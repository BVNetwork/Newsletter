using System;
using System.Diagnostics;
using System.Net.Mail;
using BVNetwork.EPiSendMail.DataAccess;
using EPiServer.Web;

namespace BVNetwork.EPiSendMail.Library
{
	/// <summary>
	/// This class does the actual mail sending, using the standard .NET Framework
	/// </summary>
    public class MailSenderNetSmtp : MailSenderBase, IMailSenderVerification
	{
		private static log4net.ILog _log = log4net.LogManager.GetLogger(typeof(MailSenderNetSmtp));

        /// <summary>
		/// Verifies the environment, called before the send process 
		/// starts to verify that important settings and infrastructure
		/// is in place.
		/// </summary>
		/// <returns>An EnvironmentVerification object, holding all the verification tests that has been run</returns>
		public virtual EnvironmentVerification VerifyEnvironment()
		{
			EnvironmentVerification env = new EnvironmentVerification();

            SmtpClient client = EPiMailEngine.GetConfiguredSmtpClient();

            // Need an smtp server specified
            if (string.IsNullOrEmpty(client.Host) && string.IsNullOrEmpty(client.PickupDirectoryLocation))
            {
                env.VerificationItems.Add(VerificationType.Error, "Missing Smtp settings in web.config. You need to configure smtp network host or a pickup directory.");
            }

			return env;
		}

	    /// <summary>
	    /// Send mail to mailreceivers using System.Net.Mail 
	    /// </summary>
	    /// <param name="mailInfo"></param>
	    /// <param name="recepients">receivers</param>
	    /// <param name="from">from-address for mail</param>
	    /// <param name="testMode">No mails are sent, generating report for user</param>
	    /// <returns>A html formatted report of the send process</returns>
	    public override SendMailLog SendEmail(MailInformation mailInfo, JobWorkItems recepients, bool testMode)
		{
			_log.Debug("Starting Send");
			// for logging
			SendMailLog log = new SendMailLog();
			log.SendStart = DateTime.Now;

			// HttpContext.Current.Server.ScriptTimeout = 7200;

			// Need someone to send to
			if (recepients.Items.Count == 0)
			{
				_log.Error("Trying to send newsletter with an empty JobWorkCollection. Please check the collection before attemting to send mail.");
				throw new ArgumentNullException("recepients", "Recipient collection is empty, there is no recipients to send to.");
			}

			// And, we need a sender address
			if (string.IsNullOrEmpty(mailInfo.From))
			{
				_log.Error("Missing from address. SMTP servers do not allow sending without a sender.");
				throw new ArgumentNullException("mailInfo", "Missing from address. SMTP servers do not allow sending without a sender.");
			}

			// Loop through receivers collection, send email for each
            foreach (JobWorkItem recipient in recepients)
            {
                _log.Debug(string.Format("Job {0}, Email: {1}, Status: {2}",
                           recipient.JobId.ToString(), recipient.EmailAddress, recipient.Status.ToString()));

                MailMessage mail = new MailMessage(mailInfo.From, recipient.EmailAddress);
                mail.Subject = mailInfo.Subject;

                mail.Body = mailInfo.BodyHtml;
                mail.IsBodyHtml = true;

                try
                {
                    if (SendMail(mail, testMode))
                    {
                        log.SuccessMessages.Add(recipient.EmailAddress);
                        
                        // Update status and save it
                        recipient.Status = JobWorkStatus.Complete;
                        // Only save if real work item (could be a test item)
                        if (recipient.JobId > 0)
                            recipient.Save();
                        
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Error sending to email: {0}", recipient.EmailAddress), ex);
                    string exceptionMsg = string.Format("Email: {0}\r\nException: {1}\r\n\r\n", recipient.EmailAddress, ex.Message);
                    log.ErrorMessages.Add(exceptionMsg);

                    // Update work item
                    recipient.Status = JobWorkStatus.Failed;
                    if (exceptionMsg.Length >= 2000)
                        exceptionMsg = exceptionMsg.Substring(0, 1999);
                    recipient.Info = exceptionMsg;
                    if (recipient.JobId > 0)
                        recipient.Save();

                }
            }	

			// Finished
			log.SendStop = DateTime.Now;

			_log.Debug("Ending Send");
			// return report 
			return log;
		}

		/// <summary>
		/// Does the actual send action for the emails, using the System.Web.Mail namespace
		/// </summary>
		/// <param name="mail"></param>
		/// <param name="onlyTestDontSendMail"></param>
		/// <returns></returns>
		internal bool SendMail(MailMessage mail, bool onlyTestDontSendMail)
		{
			
			//don't send mail if localhost is set to smtp-server (probably in development enviroment)
			// 20051219 SC: Comment above is sound, but there is no test for it
			if ( ! onlyTestDontSendMail )
			{
			    // mail. .UrlContentBase = SiteDefinition.Current.SiteUrl.ToString();
				mail.BodyEncoding = System.Text.Encoding.UTF8;

			    SmtpClient client = EPiMailEngine.GetConfiguredSmtpClient();
                client.Send(mail);

				return true;
			}							
			
			return false;
		}


	}
}
