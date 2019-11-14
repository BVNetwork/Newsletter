using System;
using System.Collections.Generic;
using System.Linq;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Library;
using SendGrid;
using SendGrid.Helpers.Mail;
using EmailAddress = SendGrid.Helpers.Mail.EmailAddress;


namespace BVNetwork.EPiSendMail.SendGrid
{
    public class MailSenderSendGrid : MailSenderBatchBase, IMailSenderVerification
    {
        private const string ApiKeyName = "Newsletter.SendGrid.ApiKey";

        public class SendGridSettings
        {
            public string ApiKey { get; set; }
        }

        /// <summary>
        /// Sends the mail batch using the SendGrid API
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <param name="recipients">The recipients.</param>
        /// <param name="onlyTestDontSendMail">if set to <c>true</c> [only test dont send mail].</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool SendMailBatch(MailInformation mail, IEnumerable<JobWorkItem> recipients, bool onlyTestDontSendMail)
        {
            var settings = GetSettings();

            if (recipients == null || recipients.Any() == false)
                throw new ArgumentException("No workitems", nameof(recipients));

            if (recipients.Count() > 1000)
                throw new ArgumentOutOfRangeException(nameof(recipients), "SendGrid supports maximum 1000 recipients per batch send.");

            var msg = new SendGridMessage
            {
                From = new EmailAddress(mail.From),
                Subject = mail.Subject,
                HtmlContent = mail.BodyHtml,
                PlainTextContent = mail.BodyText
            };

            var personalizations = new List<Personalization>();
            foreach (var jobWorkItem in recipients)
            {
                var personalization = new Personalization();
                personalization.Tos = new List<EmailAddress>
                {
                    new EmailAddress(jobWorkItem.EmailAddress)
                };
                personalization.Substitutions = new Dictionary<string, string>
                {
                    {"%recipient%", jobWorkItem.EmailAddress}
                };

                personalizations.Add(personalization);  
            }

            msg.Personalizations = personalizations;

            if (mail.EnableTracking)
            {
                // true indicates that links in plain text portions of the email 
                // should also be overwritten for link tracking purposes. 
                msg.SetClickTracking(true,true);
                msg.SetOpenTracking(true);
            }

            if(mail.CustomProperties.ContainsKey("SendGridCategory"))
            {
                string category = mail.CustomProperties["SendGridCategory"] as string;
                if (string.IsNullOrEmpty(category) == false)
                    msg.Categories.Add(category);
            }
            
            var client = new SendGridClient(settings.ApiKey);

            client.SendEmailAsync(msg).Wait();

            return true;
        }

        public EnvironmentVerification VerifyEnvironment()
        {
            EnvironmentVerification env = new EnvironmentVerification();

            SendGridSettings settings = GetSettings();
            if (string.IsNullOrEmpty(settings.ApiKey))
            {
                env.VerificationItems.Add(VerificationType.Error, "Missing api key setting for SendGrid.");
            }

            return env;
        }

        protected SendGridSettings GetSettings()
        {
            return new SendGridSettings
            {
                ApiKey = NewsLetterConfiguration.GetAppSettingsConfigValueEx<string>("Newsletter.SendGrid.ApiKey", "")
            };
        }

        public override int GetBatchSize()
        {
            // SendGrid lets us send 1000 each time. We'll default to 999
            return 999;
        }
    }
}
