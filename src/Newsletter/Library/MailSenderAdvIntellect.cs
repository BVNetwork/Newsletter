using System;
using System.IO;
using System.Net.Mail;
using aspNetEmail;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using EPiServer.Logging;
using EPiServer.Web;

namespace BVNetwork.EPiSendMail.Library
{
    /// <summary>
    /// This class does the actual mail sending, using the standard .NET Framework
    /// </summary>
    public class MailSenderAdvIntellect : MailSenderBase, IMailSenderVerification
    {
        private static readonly ILogger _log = LogManager.GetLogger();
        private const string REL_LICENSE_PATH = @"modules\bvn\sendmail\license\aspNetEmail.xml.lic";

        public MailSenderAdvIntellect()
        {
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

            // Verify all the settings

            // Get the registered filter for working with recipients
            RecipientsUtility recipUtil = new RecipientsUtility();

            SmtpClient client = EPiMailEngine.GetConfiguredSmtpClient();

            // Need an smtp server specified
            if (string.IsNullOrEmpty(client.Host) == true)
            {
                env.VerificationItems.Add(VerificationType.Error, "Missing Smtp host setting in web.config. The aspnetEmail component requires a SMTP server to be specified.");
            }

            // Verify license file
            if (DoesLicenseFileExist() == false)
                env.VerificationItems.Add(VerificationType.Error, "Can't find aspNetEmail License file: " + GetLocalLicenseFilePath());

            return env;
        }

        /// <summary>
        /// Send mail to mailreceivers using System.Web.Mail 
        /// </summary>
        /// <param name="mailInfo"></param>
        /// <param name="recipients"></param>
        /// <param name="testMode">No mails are sent, generating report for user</param>
        /// <returns>A SendMailLog object with information about the status of the job.</returns>
        public override SendMailLog SendEmail(MailInformation mailInfo, JobWorkItems recipients, bool testMode)
        {
            _log.Debug("900.10.11.1 Starting Send");
            // for logging
            SendMailLog log = new SendMailLog();
            log.StartJob();
            // Recipients util
            RecipientsUtility recipUtil = new RecipientsUtility();

            // We try to verify as many settings, variables etc. before
            // starting the sending loop, as we don't want to generate
            // lots of exceptions in a loop.

            // Need someone to send to
            if (recipients.Items.Count == 0)
            {
                _log.Error("900.10.11.2 Trying to send mail with an empty JobWorkItems collection. Please check the collection before attemting to send mail.");
                throw new ArgumentNullException("Recipient collection is empty, there is no recipients to send to.", "recepients");
            }

            // And, we need a sender address
            if (mailInfo.From == null || mailInfo.From == string.Empty)
            {
                _log.Error("900.10.11.3 Missing from address. SMTP servers do not allow sending without a sender.");
                throw new ArgumentNullException("Missing from address. SMTP servers do not allow sending without a sender.", "mailInfo.From");
            }

            // Load the license. This needs to be in the
            EmailMessage.LoadLicenseString(GetLicenseFileContents());

            // We'll reuse the mail object, so we create it outside of the loop
            EmailMessage mail = CreateMessage(mailInfo);

            // Set port and authentication details if found
            InitializeMailSettings(ref mail);

            // Send a warm-up message outside the loop. This
            // will help us catch any misconfigurations and other
            // things that prevents us from sending emails. By
            // doing this outside the loop, we won't kill the
            // application or log with uneccesary error messages.
            // TODO: Implement this

            // Loop through receivers collection, send email for each
            foreach (JobWorkItem workItem in recipients)
            {
                _log.Debug(string.Format("900.10.11.5 Job {0}, Email: {1}, Status: {2}",
                           workItem.JobId.ToString(), workItem.EmailAddress, workItem.Status.ToString()));

                // Only attempt sending those that have been stamped as ready for sending
                // unless we're in a test case.
                if (workItem.Status == JobWorkStatus.Sending || testMode == true)
                {

                    try
                    {
                        // At this point we assume the address is formatted correctly
                        // and checked, but aspNetEmail checks the to address on set, may fail
                        mail.To = workItem.EmailAddress;

                        // Perform actual send, if testmail, then this will
                        // return false. We should not update the status on
                        // test sends
                        bool result = SendMail(mail, testMode);
                        if (result == true)
                        {
                            log.SuccessMessages.Add(workItem.EmailAddress);

                            // Update status and save it
                            // TODO: Improve performance by doing this in batch
                            workItem.Status = JobWorkStatus.Complete;
                            // Only save if real work item (could be a test item)
                            if (workItem.JobId > 0)
                                workItem.Save();

                        }
                        // else
                        // Could not send, it has been disabled by
                        // settings or parameters

                    }
                    catch (Exception ex)
                    {
                        _log.Error(string.Format("900.10.11.6 Error sending to email: {0}", workItem.EmailAddress), ex);
                        string exceptionMsg = string.Format("Email: {0}\r\nException: {1}\r\n\r\n", workItem.EmailAddress, ex.Message);
                        log.ErrorMessages.Add(exceptionMsg);

                        // Update work item
                        workItem.Status = JobWorkStatus.Failed;
                        if (exceptionMsg.Length >= 2000)
                            exceptionMsg = exceptionMsg.Substring(0, 1999);
                        workItem.Info = exceptionMsg;
                        if (workItem.JobId > 0)
                            workItem.Save();
                    }
                }
                else
                {
                    _log.Debug(string.Format("900.10.11.7 Skipping Recipient, wrong status. Job {0}, Email: {1}, Status: {2}",
                              workItem.JobId.ToString(), workItem.EmailAddress, workItem.Status.ToString()));
                }
            }

            // Finished
            log.StopJob();

            // Warn user if logging is enabled
            if (Configuration.NewsLetterConfiguration.ExtendedLogFile != null)
                log.WarningMessages.Add("Logging has been enabled. Only use during troubleshooting.");

            _log.Debug("900.10.11.8 Ending Send");
            // return report 
            return log;
        }

        /// <summary>
        /// Initializes the mail settings.
        /// </summary>
        /// <param name="mail">The mail.</param>
        protected void InitializeMailSettings(ref EmailMessage mail)
        {
            SmtpClient client = EPiMailEngine.GetConfiguredSmtpClient();

            // Server
            mail.Server = client.Host;

            // Port
            mail.Port = client.Port;

            //// User/password may both be empty -- in that case just ignore everything... 
            //if (string.IsNullOrEmpty(userName) == false ||
            //    string.IsNullOrEmpty(password) == false)
            //{
            //    // If one has been defined, the other must also be defined
            //    if (string.IsNullOrEmpty(userName))
            //        throw new EPiServerException("Undefined username");
            //    if (string.IsNullOrEmpty(password))
            //        throw new EPiServerException("Undefined password");

            //    mail.Username = userName;
            //    mail.Password = password;
            // }
        }

        /// <summary>
        /// Creates the aspNET EmailMessage object based on mail information
        /// we've alread got. Construct the message from html, and then spesify
        /// that the images should be attached inline.
        /// </summary>
        /// <param name="mailInfo">Mail info.</param>
        /// <returns></returns>
        private EmailMessage CreateMessage(MailInformation mailInfo)
        {
            if (_log.IsDebugEnabled())
                _log.Debug("Begin CreateMessage. Base Url: {0}", mailInfo.BaseUrl);

            EmailMessage email = new EmailMessage();
            HtmlUtility utility = new HtmlUtility(email);

            // Swallow exceptions if configured
            if (Configuration.NewsLetterConfiguration.DisableExceptionsInMailRendering == true)
                email.ThrowException = false;

            // Troubleshooting needs logging
            if (Configuration.NewsLetterConfiguration.ExtendedLogFile != null)
            {
                // Be careful with logging, it will generate large amounts of log data
                email.LogInMemory = false;
                email.LogPath = EPiServer.Global.BaseDirectory + Configuration.NewsLetterConfiguration.ExtendedLogFile;
                email.LogDebugStatements = true;
                email.LogBody = true;
                email.Logging = true;
            }

            // set all relative links contained in the html to this url 
            string baseUrl = SiteDefinition.Current.SiteUrl.ToString();

            if (string.IsNullOrEmpty(mailInfo.BaseUrl) == false)
                baseUrl = mailInfo.BaseUrl;

            // Resolves Hrefs to their absolute value, this means
            // no urls in the html will be relative (which can be a pain
            // depending on the markup
            utility.ResolveHrefs = true;

            // Clean html, remove tags and content we do not want or need
            utility.HtmlRemovalOptions = HtmlRemovalOptions.AppletTag |
                                        HtmlRemovalOptions.EmbedTag |
                                        HtmlRemovalOptions.NoScriptTag |
                                        HtmlRemovalOptions.ObjectTag |
                                        HtmlRemovalOptions.ParamTag |
                                        HtmlRemovalOptions.Scripts |
                                        HtmlRemovalOptions.ViewState |
                                        HtmlRemovalOptions.EventArgument |
                                        HtmlRemovalOptions.EventTarget;

            // Load the html mail
            utility.LoadString(mailInfo.BodyHtml, baseUrl);

            // Set the UrlContent base 
            utility.SetUrlContentBase = false;

            // Set the basetag in the html 
            utility.SetHtmlBaseTag = true;

            // Embed the images into the email message, using the
            // content id as a src reference. Change this if you do
            // not want images to be part of the message
            utility.EmbedImageOption = EmbedImageOption.ContentId;

            // If you have problems loading images, disable exceptions
            // if (utility.ParentMessage != null)
            //    utility.ParentMessage.ThrowException = false;
            
            //render the Html so it is properly formatted for email 
            utility.Render();

            //render an EmailMessage with appropriate text and html parts 
            email = utility.ToEmailMessage();

            //load remaining properties from web.config 
            email.LoadFromConfig();

            // Try to get these encodings correct
            email.ContentTransferEncoding = MailEncoding.QuotedPrintable;

            // Using utf-8 would have been nice, but utf-8 is not widely
            // adopted by email clients it seems.
            // email.CharSet = "utf-8";

            // Using iso 8859-1 hotmail will show Norwegian characters
            // even if the user runs with english settings
            // Hotmail needs this
            email.CharSet = "iso-8859-1";

            // Override with our own values
            email.Subject = mailInfo.Subject;
            email.From = mailInfo.From;
            email.TextBodyPart = mailInfo.BodyText;

            if (_log.IsDebugEnabled())
                _log.Debug("Rendered Mail Message: {0}", email.Subject);

            return email;
        }

        /// <summary>
        /// Does the actual send action for the emails, using the System.Web.Mail namespace
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="onlyTestDontSendMail"></param>
        /// <returns></returns>
        internal bool SendMail(EmailMessage mail, bool onlyTestDontSendMail)
        {
            //don't send mail if localhost is set to smtp-server (probably in development enviroment)
            // 20051219 SC: Comment above is sound, but there is no test for it
            if (onlyTestDontSendMail == false)
            {
                // It returns true if successfull 
                return mail.Send();
            }
            return false;
        }

        /// <summary>
        /// Gets the license file contents.
        /// </summary>
        /// <returns>The xml content of the aspNetEmail license file</returns>
        private string GetLicenseFileContents()
        {
            // No HttpContext available
            string localFile = GetLocalLicenseFilePath();
            StreamReader reader = File.OpenText(localFile);
            string fileContent = reader.ReadToEnd();
            return fileContent;

        }

        /// <summary>
        /// Checks if the license file exist. This does not check
        /// if the file is valid (not expired).
        /// </summary>
        /// <returns>true if the license exists on disk, false if not</returns>
        private bool DoesLicenseFileExist()
        {
            // No HttpContext available
            string localFile = GetLocalLicenseFilePath();
            return File.Exists(localFile);
        }

        /// <summary>
        /// Gets the local license file path. Should not start with
        /// a back slash as it will be prepended along with the rest
        /// of the local path.
        /// </summary>
        /// <remarks>
        /// Default value is:
        /// "bvn\sendmail\license\aspNetEmail.xml.lic"
        /// </remarks>
        /// <returns>The root relative path (not URL) to the license xml file.</returns>
        private string GetLocalLicenseFilePath()
        {
            string localFile = null;
            // Should be of type:
            // bvn\sendmail\license\aspNetEmail.xml.lic (default)
            string nonStandardLicensePath = NewsLetterConfiguration.GetAppSettingsConfigValueEx<string>("EPsAspNetEmailRelativeLicensePath", null);
            if (string.IsNullOrEmpty(nonStandardLicensePath) == false)
            {
                localFile = EPiServer.Global.BaseDirectory + nonStandardLicensePath;
            }
            else
            {
                localFile = EPiServer.Global.BaseDirectory + REL_LICENSE_PATH;
            }

            return localFile;
        }

    }
}
