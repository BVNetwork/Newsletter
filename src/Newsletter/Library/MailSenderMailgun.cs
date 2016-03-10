using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using BVNetwork.EPiSendMail.Contracts;
using BVNetwork.EPiSendMail.DataAccess;
using EPiServer.Core;
using EPiServer.Logging;
using PreMailer.Net;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
// ReSharper disable PossibleMultipleEnumeration

namespace BVNetwork.EPiSendMail.Library
{
	/// <summary>
	/// This class does the actual mail sending, using the standard .NET Framework
	/// </summary>
    public class MailSenderMailgun : MailSenderBatchBase, IMailSenderVerification
	{
		private const string MAILGUN_CAMPAIGN_PROPERTYNAME = "MailgunCampaignName";
		private const string MAILGUN_TAG_PROPERTYNAME = "MailgunTagName";
        private static readonly ILogger _log = LogManager.GetLogger();

        public class MailgunSettings
		{
			public string ApiKey { get; set; }
			public string Domain { get; set; }
			public string PublicKey { get; set; }
		    public string ProxyAddress { get; set; }
		    public int ProxyPort { get; set; }
		}

		/// <summary>
		/// Verifies the environment, called before the send process 
		/// starts to verify that important settings and infrastructure
		/// is in place.
		/// </summary>
		/// <returns>An EnvironmentVerification object, holding all the verification tests that has been run</returns>
		public virtual EnvironmentVerification VerifyEnvironment()
		{
			EnvironmentVerification env = new EnvironmentVerification();

			MailgunSettings settings = GetSettings();
			if (string.IsNullOrEmpty(settings.ApiKey))
			{
				env.VerificationItems.Add(VerificationType.Error, "Missing Mailgun.ApiKey setting in configuration.");
			}

			if (string.IsNullOrEmpty(settings.Domain))
			{
				env.VerificationItems.Add(VerificationType.Error, "Missing Mailgun.Domain setting in configuration.");
			}

			return env;
		}


		/// <summary>
		/// Send mail to mailreceivers using Mailgun REST API
		/// </summary>
		/// <param name="mailInfo"></param>
		/// <param name="recepients">receivers</param>
		/// <param name="testMode">No mails are sent, generating report for user</param>
		/// <returns>A html formatted report of the send process</returns>
		public override SendMailLog SendEmail(MailInformation mailInfo, JobWorkItems recepients, bool testMode)
		{
			_log.Debug("Starting Mailgun Send");

			// Inline all css
			InlineResult cssInline = PreMailer.Net.PreMailer.MoveCssInline(mailInfo.BodyHtml);
			mailInfo.BodyHtml = cssInline.Html;
			
            // Base will send
		    SendMailLog log = base.SendEmail(mailInfo, recepients, testMode);

		    // Log any messages, debug is only detected
            // if we have an HttpContext.
            if (IsInDebugMode())
            {
                log.WarningMessages.Add("Premailer CSS warning messages are only shown in debug mode. Primarily for developers.");
                log.WarningMessages.AddRange(cssInline.Warnings.ToArray());
            }

			_log.Debug("Ending Mailgun Send");
			// return report 
			return log;
		}

	    public override bool SendMailBatch(MailInformation mail, IEnumerable<JobWorkItem> recipients, bool onlyTestDontSendMail)
	    {
	        MailgunSettings settings = GetSettings();

			// ReSharper disable PossibleMultipleEnumeration
			if(recipients == null || recipients.Count() == 0)
				throw new ArgumentException("No workitems", "recipients");
			
			if(recipients.Count() > 1000)
				throw new ArgumentOutOfRangeException("recipients", "Mailgun supports maximum 1000 recipients per batch send.");

			RestClient client = new RestClient();
	        client.BaseUrl = new Uri("https://api.mailgun.net/v2");
			client.Authenticator = new HttpBasicAuthenticator("api", settings.ApiKey);

            if(string.IsNullOrEmpty(settings.ProxyAddress) == false)
            {
                client.Proxy = new WebProxy(settings.ProxyAddress, settings.ProxyPort); // Makes it easy to debug as Fiddler will show us the requests
            }

			RestRequest request = new RestRequest();
			request.AlwaysMultipartFormData = true;
			request.AddParameter("domain", settings.Domain, ParameterType.UrlSegment);
			request.Resource = "{domain}/messages";
			request.AddParameter("from", mail.From);
			request.AddParameter("subject", mail.Subject);
			request.AddParameter("text", mail.BodyText);
			request.AddParameter("html", mail.BodyHtml);
			
			if(mail.EnableTracking)
			{
				request.AddParameter("o:tracking", mail.EnableTracking);
				request.AddParameter("o:tracking-clicks", mail.EnableTracking);
				request.AddParameter("o:tracking-opens", mail.EnableTracking);
			}

			foreach (KeyValuePair<string, object> customProperty in mail.CustomProperties)
			{
				request.AddParameter(customProperty.Key, customProperty.Value.ToString());
			}

			// Add custom data about job
			if (mail.CustomProperties.ContainsKey("v:newsletter-data") == false)
			{
				request.AddParameter("v:newsletter-data", 
									 string.Format("{{\"id\": \"{0}\", \"page\": \"{1}\" }}", 
													recipients.First().JobId, mail.PageLink.ID));
			}

			// Add all recipients
			StringBuilder recipVariables = new StringBuilder();
			bool first = true;
			foreach (JobWorkItem recipient in recipients)
			{
				request.AddParameter("to", recipient.EmailAddress);

				if (first == false)
				{
					recipVariables.Append(",");
				}
				first = false;
				recipVariables.AppendFormat("\"{0}\" : {{\"id\": \"{1}\" }}", recipient.EmailAddress, recipient.GetHashCode());
			}
			request.AddParameter("recipient-variables", "{" + recipVariables.ToString() + "}");

			//if(onlyTestDontSendMail)
			//{
			//    request.AddParameter("o:testmode", true);
			//}

			request.Method = Method.POST;
			var response = client.Execute(request);

			Dictionary<string, string> resultParams = null;
			try
			{
				resultParams = new JsonDeserializer().Deserialize<Dictionary<string, string>>(response);
			}
			catch (Exception e)
			{
				_log.Warning("Unable to parse Mailgun response.", e);
			}

			if(response.StatusCode == HttpStatusCode.OK)
			{
				_log.Debug("Mailgun responded with: {0} - {1}", response.StatusCode, response.StatusDescription);
				if(string.IsNullOrEmpty(response.ErrorMessage) == false)
				{
					_log.Error("Response Error: {0}", response.ErrorMessage);
				}
				_log.Debug(response.Content);

				// json looks like:
				//{
				//  "message": "Queued. Thank you.",
				//  "id": "<20140630134433.55355.54227@sandboxb59eb07ea01544958e1a00200a8abc27de.mailgun.org>"
				//}
				
				// Update all recipients with information
				if(resultParams != null)
				{
					string info = resultParams["id"];
					foreach (JobWorkItem recipient in recipients)
					{
						recipient.Info = info;
					}
					
				}
			}
			else
			{
				_log.Debug("Mailgun responded with: {0} - {1}", response.StatusCode, response.StatusDescription);
				string errorMessage = response.StatusDescription;
				if(resultParams != null)
					errorMessage = resultParams["message"];

				if (string.IsNullOrEmpty(response.ErrorMessage) == false)
				{
					_log.Error("Response Error: {0}", response.ErrorMessage);
				}
				_log.Debug(response.Content);

				throw new HttpException((int)response.StatusCode, errorMessage);
			}
			
			return true;
			// ReSharper restore PossibleMultipleEnumeration
		}

		/// <summary>
		/// Populates needed mail information before sending. Will also check
		/// for Mailgun specific properties
		/// </summary>
		/// <remarks>
		/// Note! Mailgun validate campaign codes, and if the campaign does
		/// not exist, it will be ignored (the email will still be sent)
		/// Note! Tags are limited to a count of 200
		/// </remarks>
		/// <param name="mailPage"></param>
		/// <returns></returns>
		public override MailInformation GetMailInformation(PageData mailPage)
		{
			MailInformation mailInformation = base.GetMailInformation(mailPage);

			IPopulateCustomProperties customPropertiesProvider = mailPage as IPopulateCustomProperties;
			if (customPropertiesProvider == null)
			{
                // The base class will add custom properties if the page type
                // implements that - if NOT, we'll try to add them ourselves
                // by looking for special property names relevant to Mailgun
                var campaign = mailPage[MAILGUN_CAMPAIGN_PROPERTYNAME];
			    if (campaign != null)
			    {
			        mailInformation.Utm.Campaign = campaign.ToString();

			    }

                // Since the Utm Campaign can be set independently, we check if it
                // is set, and use it as a Mailgun campaign too
                if(string.IsNullOrEmpty(mailInformation.Utm.Campaign) == false)
                {
                    mailInformation.CustomProperties.Add("o:campaign", mailInformation.Utm.Campaign);
                }


                if (mailPage[MAILGUN_TAG_PROPERTYNAME] != null)
				{
					mailInformation.CustomProperties.Add("o:tag", mailPage[MAILGUN_TAG_PROPERTYNAME]);
				}
			}
			return mailInformation;
		}

		public override int GetBatchSize()
		{
			// Mailgun lets us send 1000 each time. We'll default to 999
			return 999;
		}

		protected MailgunSettings GetSettings()
		{
			MailgunSettings settings = new MailgunSettings();
			settings.ApiKey = Configuration.NewsLetterConfiguration.GetAppSettingsConfigValueEx<string>("Mailgun.ApiKey", null);
			settings.Domain = Configuration.NewsLetterConfiguration.GetAppSettingsConfigValueEx<string>("Mailgun.Domain", null);
			settings.PublicKey = Configuration.NewsLetterConfiguration.GetAppSettingsConfigValueEx<string>("Mailgun.PublicKey", null);

            // Optional
            settings.ProxyAddress = Configuration.NewsLetterConfiguration.GetAppSettingsConfigValueEx<string>("Mailgun.ProxyAddress", null);
            settings.ProxyPort = Configuration.NewsLetterConfiguration.GetAppSettingsConfigValueInt("Mailgun.ProxyPort", 0);

			return settings;
		}



	    public List<string> ValidateRecipientList(RecipientList list, RecipientList blocked)
	    {
	        List<string> invalidAddresses = new List<string>();
	        const int maxSize = 3900;
            // Run checks in batches, Mailgun does not like us 
            // washing the lists like this. Max size to check is 8000 chars
            List<EmailAddress> batch = new List<EmailAddress>();
	        int size = 0;
	        foreach (EmailAddress address in list.EmailAddresses)
	        {
	            // Check size (including separator)
                if ((size + address.Email.Length)> maxSize)
	            {
                    // Wash and clear the ones we've got so far
	                invalidAddresses.AddRange(ValidateRecipientList(batch, blocked));
	                size = 0;
                    batch.Clear();
	            }
                batch.Add(address);
                // Increase size used (including separator)
	            size += address.Email.Length + 1;
	        }

            // Finish up with the remaining
            invalidAddresses.AddRange(ValidateRecipientList(batch, blocked));

	        return invalidAddresses;
	    }

        public List<string> ValidateRecipientList(List<EmailAddress> recipientAddresses, RecipientList blocked)
		{
            _log.Debug("Validating {0} emails using block list {1} ({2})", recipientAddresses.Count, blocked.Name, blocked.Id);

            if(recipientAddresses.Count == 0)
                return new List<string>();

		    MailgunSettings settings = GetSettings();
		    RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
			client.Authenticator = new HttpBasicAuthenticator("api", settings.PublicKey);
            
            if (string.IsNullOrEmpty(settings.ProxyAddress) == false)
            {
                client.Proxy = new WebProxy(settings.ProxyAddress, settings.ProxyPort); // Makes it easy to debug as Fiddler will show us the requests
            }
            
            RestRequest request = new RestRequest();

            // We're sending a lot of data, which should 
            // be a post, but Mailgun does not allow that
            // request.Method = Method.POST;

			request.Resource = "/address/parse";
		    
            // Validate strict
            request.AddParameter("syntax_only", false);

		    string addresses = "";

            foreach (EmailAddress emailAddress in recipientAddresses)
		    {
		        addresses += emailAddress.Email + ",";
		    }

            _log.Debug("Length of address field sent to Mailgun: {0}", addresses.Length);

            if(addresses.Length > 8000)
            {
                throw new ApplicationException("Mailgun only accepts address fields with length of 8000 characters.");
            }

            request.AddParameter("addresses", addresses.TrimEnd(','));

		    var response = client.Execute(request);
            _log.Debug("Mailgun responded with status: {0} - {1}", (int)response.StatusCode, response.StatusDescription);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                /*
                {
                    "parsed": [
                        "Alice <alice@example.com>",
                        "bob@example.com"
                    ],
                    "unparseable": [
                        "example.com"
                    ]
                }
                */
                Dictionary<string, List<string>> resultParams = null;
                try
                {
                    resultParams = new JsonDeserializer().Deserialize<Dictionary<string, List<string>>>(response);
                }
                catch (Exception e)
                {
                    _log.Warning("Unable to parse Mailgun response.", e);
                }

                // Update all recipients with information
                if (resultParams != null)
                {   
                    List<string> invalidAddresses = resultParams["unparseable"];
                    foreach (string address in invalidAddresses)
                    {
                        EmailAddress emailAddress = recipientAddresses.Find(a => a.Email.Equals(address, StringComparison.InvariantCultureIgnoreCase));
                        if (emailAddress != null)
                        {
                            emailAddress.Comment = "Mailgun reported address as invalid.";
                            emailAddress.Save();
                        }

                        EmailAddress blockedAddress = blocked.CreateEmailAddress(address);
                        blockedAddress.Comment = "Mailgun reported address as invalid.";
                        blockedAddress.Save();
                    }

                    return invalidAddresses;
                }
            }
            else
            {
                // Attempt to log error from Mailgun
                if(string.IsNullOrEmpty(response.ErrorMessage) == false)
                    _log.Warning(response.ErrorMessage);

                if(string.IsNullOrEmpty(response.Content) == false)
                    _log.Debug(response.Content);

                throw new ApplicationException("Cannot validate email addresses using Mailgun: " + response.ErrorMessage);
            }
		    return null;
		}
	}

}
