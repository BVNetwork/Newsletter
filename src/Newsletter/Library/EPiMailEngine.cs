using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using EPiServer.Core;

namespace BVNetwork.EPiSendMail.Library
{
    /// <summary>
    /// The engine responsible for retrieving userdata and
    /// sending mail. 
    /// </summary>
    public class EPiMailEngine
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(EPiMailEngine));

        /// <summary>
        /// Gets preview HTML to be shown on the web.
        /// </summary>
        /// <param name="pageLink">Page link to the mail page to show.</param>
        /// <returns>The fully html formatted text</returns>
        public string GetPreviewHtml(ContentReference pageLink)
        {
            MailInformation mailInfo;
            MailSenderNetSmtp sender = new MailSenderNetSmtp();
            mailInfo = sender.GetMailInformation(pageLink as PageReference);

            return mailInfo.BodyHtml;
        }


        /// <summary>
        /// Verifies the environment, and checks that the
        /// neccessary configuration settings is correct.
        /// </summary>
        /// <returns></returns>
        public EnvironmentVerification VerifyEnvironment()
        {
            // Construct correct sender object based on config setting
            IMailSenderVerification sender = GetMailSender() as IMailSenderVerification;

            if (sender != null) return 
                sender.VerifyEnvironment();
            return new EnvironmentVerification();
        }

        protected MailInformation GetMailInformation(MailSenderBase sender, ContentReference pageRef, string from, string subject)
        {
            // Get content to send
            MailInformation mailInfo = sender.GetMailInformation(pageRef as PageReference);
            if (!string.IsNullOrEmpty(subject))
                mailInfo.Subject = subject; // Can be something other than the pagename
            mailInfo.From = from;
            return mailInfo;
        }

        /// <summary>
        /// Send mail to mailreceivers
        /// </summary>
        /// <param name="subject">subject for mail</param>
        /// <param name="pageRef">reference to mailpage</param>
        /// <param name="from">from-address for mail</param>
        /// <param name="onlyTestDontSendMail">No mails are sent, generating report for user</param>
        /// <param name="job">The job to send as newsletter</param>
        /// <returns></returns>
        public SendMailLog SendNewsletter(string subject, string from, ContentReference pageRef, JobWorkItems workItems, bool onlyTestDontSendMail)
        {
            // Construct correct sender object based on config setting
            MailSenderBase sender = GetMailSender();

            // Construct mail object with default values and content
            MailInformation mailInfo = GetMailInformation(sender, pageRef, from, subject);

            // Send it
            SendMailLog log;
            log = sender.SendEmail(mailInfo, workItems, onlyTestDontSendMail);

            // Add additional information to the log
            log.Subject = subject;

            return log;
        }

        /// <summary>
        /// Creates a mailsender based on a configuration setting
        /// </summary>
        /// <returns></returns>
        public MailSenderBase GetMailSender()
        {
            string typeName = GetMailSenderTypeName();

            if (_log.IsDebugEnabled)
                _log.Debug("Creating sender object: " + typeName);

            Type senderType = Type.GetType(typeName);

            if (senderType == null)
            {
                throw new ConfigurationErrorsException("Mailsender type for " + typeName + " cannot be found.");
            }

            object senderImpl = Activator.CreateInstance(senderType);
            return senderImpl as MailSenderBase;
        }

        /// <summary>
        /// Gets the type name of the mail sender type, from
        /// which you can create instances of.
        /// </summary>
        /// <returns></returns>
        public string GetMailSenderTypeName()
        {
            const string defSender = "MailSenderNetSmtp";

            // For backwards compatibility, we need to use the appSettings value first
            string typeName = NewsLetterConfiguration.MailSenderTypename;

            if(string.IsNullOrEmpty(typeName))
            {
                // Get from new config section
                typeName = NewsletterConfigurationSection.Instance.SenderType;
            }

            if (typeName == null)
            {
                // Last resort - use SMTP as default as sensible default
                typeName = this.GetType().Namespace + "." + defSender + ", " + this.GetType().Assembly.FullName;
            }

            return typeName;
        }

        /// <summary>
        /// Initializes the SMTP client that will be used by all mail functions.
        /// Call this function to get a smtp client that is setup according to the current configuration.
        /// The default SmtpServer can be defined in web.config.
        /// </summary>
        /// <returns>An instance of a smtp client object</returns>
        public static SmtpClient GetConfiguredSmtpClient()
        {
            SmtpClient client = new SmtpClient {Timeout = 5 * 1000};
            return client;
        }

        /// <summary>
        /// Initializes the SMTP client that will be used by all mail functions.
        /// Call this function to get a smtp client that is setup using the values passed as arguments.
        /// Pass a null value (or -1 for smtpPort) to fallback to the configured value for a parameter.
        /// </summary>
        /// <param name="smtpServer">hostname of the SMTP server</param>
        /// <param name="smtpPort">the port</param>
        /// <param name="userName">the username used for authentication</param>
        /// <param name="password">the password used for authentication</param>
        /// <returns>An instance of a smtp client object</returns>
        /// <remarks>If a value other than null is passed to userName then a password must be given as well.</remarks>
        public static SmtpClient GetConfiguredSmtpClient(string smtpServer, int smtpPort, string userName, string password)
        {
            SmtpClient smtp = new SmtpClient
            {
                Port = smtpPort, 
                Host = smtpServer
            };

            // User/password may both be empty -- in that case just ignore everything... 
            if (userName.Length != 0 || password.Length != 0)
            {
                // If one has been defined, the other must also be defined
                if (userName.Length == 0)
                    throw new EPiServerException("Undefined username");
                if (password.Length == 0)
                    throw new EPiServerException("Undefined password");

                smtp.Credentials = new NetworkCredential(userName, password);
            }

            return smtp;
        }
    }
}
