using System;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Library;
using EPiServer.Framework.Web.Mvc.Html;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class JobEditControl : JobUiUserControlBase
    {
        // Logger
        private static log4net.ILog _log = log4net.LogManager.GetLogger(typeof(JobEditControl));

        private const string TEST_SUBJECT_POSTFIX = " (TEST)";

        protected StatusMessage ucStatusMessage;
        protected EnvironmentVerification ucEnvironmentVerification;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Tell base where to show messages
            MessageControl = (StatusMessage)this.ucStatusMessage;
            cmdSendTest.Click += new EventHandler(SendTest_ClickHandler);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.ClientScript.IsClientScriptIncludeRegistered("jquery") == false)
            {
                Page.ClientScript.RegisterClientScriptInclude(Page.GetType(), "jquery", NewsLetterConfiguration.GetModuleBaseDir() + "/content/js/jquery-2.0.3.min.js");

            }
            if (Page.ClientScript.IsClientScriptIncludeRegistered("bootstrap") == false)
            {
                Page.ClientScript.RegisterClientScriptInclude(Page.GetType(), "bootstrap", NewsLetterConfiguration.GetModuleBaseDir() + "/content/js/bootstrap.js");
            }

            if (NewsletterJob != null)
                BindJobData(NewsletterJob);

            if (!IsPostBack)
            {
                if (NewsletterJob == null)
                {
                    tblEditJob.Visible = false;
                    pnlAddWorkItems.Visible = false;
                    pnlJobStatus.Visible = false;
                    pnlMiscActions.Visible = false;
                    pnlNewNewsLetter.Visible = true;
                }
                Session.Remove("CurrentSelection");
            }

            cmdSendTest.DataBind();
        }

        public void BindJobData(Job job)
        {
            h2JobName.InnerText = job.Name;
            jobDescription.Text = job.Description;
        }

        public string MailSubject
        {
            get
            {

                MailSenderBase mailSender = GetEmailEngine().GetMailSender();
                return mailSender.GetMailSubject(CurrentPage);
            }
        }

        public string MailFrom
        {
            get
            {
                MailSenderBase mailSender = GetEmailEngine().GetMailSender();
                return mailSender.GetMailSender(CurrentPage);
            }
        }

        protected string ButtonEnabled()
        {
            return CurrentPage.IsPendingPublish ? "disabled" : String.Empty;
        }


        /// <summary>
        /// Verifies the environment. This should be done
        /// before sending the email.
        /// </summary>
        /// <returns>True means ok to send, false means something is wrong</returns>
        public bool VerifyEnvironment(EPiMailEngine engine)
        {

            Library.EnvironmentVerification envVer = engine.VerifyEnvironment();
            if (envVer.HasErrors() || envVer.HasWarnings())
            {

                ucEnvironmentVerification.VerificationItems = envVer.VerificationItems;
                ucEnvironmentVerification.Visible = true;

                // Stop here if errors
                if (envVer.HasErrors())
                    return false;
            }
            return true;
        }

        protected EPiMailEngine GetEmailEngine()
        {
            EPiMailEngine engine = new EPiMailEngine();
            return engine;
        }


        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="workItems">The work items.</param>
        public void SendMail(EPiMailEngine engine, JobWorkItems workItems, bool isTestSend)
        {
            if (_log.IsDebugEnabled)
                _log.Debug("Starting send process. Testmode: " + isTestSend.ToString());

            // First we verify the environment
            if (VerifyEnvironment(engine) == false)
                return;

            // Collection of recipients from job
            if (workItems == null)
                throw new NullReferenceException("workItems cannot be null when sending newsletter");

            if (workItems.Items.Count == 0)
            {
                ShowError("No recipients defined. Please add email addresses to textbox above Test Send button.");
                return;
            }

            // Need default values
            // 1. Use MailSender property
            // 2. Use EPsSendMailFromAddress from web.config
            // 3. Construct newsletter@<sitename>
            string fromAddress = MailFrom;

            // 1. Use MailSubject property
            // 2. Use EPsSendMailSubject from web.config
            // 3. Use "Newsletter" as default
            string subject = MailSubject;
            if (isTestSend)
                subject += TEST_SUBJECT_POSTFIX;

            if (_log.IsDebugEnabled)
                _log.Debug(string.Format("Start sending newsletter based on WorkItems. Subject: '{0}', From: '{1}', Count '{2}', Test Mode: '{3}'",
                    subject, fromAddress, workItems.Items.Count.ToString(), isTestSend.ToString()));

            // Send the message
            string sendStatus;
            // We set testsend as false in the method below, as the test sending UI has changed, but the
            // logic in the engine interpret this as not sending anything. Test here means change the
            // email subject
            SendMailLog log = engine.SendNewsletter(subject, fromAddress, CurrentPage.ContentLink , workItems, false);
            sendStatus = log.GetClearTextLogString(true);

            lblSendResult.Text = sendStatus;
            pnlSendResult.Visible = true;

            if (_log.IsDebugEnabled)
                _log.Debug("Send process finished. Testmode: " + isTestSend.ToString());
            
        }

        void SendTest_ClickHandler(object sender, EventArgs e)
        {
            string sendTo = txtSendTestTo.Text.Trim();
            if (string.IsNullOrEmpty(sendTo))
                ShowError("Please add one or more email addresses to send the test to.");
            else
            {
                /// TODO: Store personalization data from the user profile instad
                // Save email addresses for next time
                // PersonalizedData.Current["EPiSendMailSavedTestAddresses"] = sendTo;

                // Parse items, set ready for sending
                JobWorkItems items = Job.ParseEmailAddressesToWorkItems(sendTo, JobWorkStatus.Sending);
                EPiMailEngine engine = GetEmailEngine();
                SendMail(engine, items, true);
            }
                
        }


    }
}