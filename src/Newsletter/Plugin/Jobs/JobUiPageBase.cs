
using BVNetwork.EPiSendMail.DataAccess;
using EPiServer.Shell.WebForms;

namespace BVNetwork.EPiSendMail.Plugin
{
    public class JobUiPageBase : ContentWebFormsBase, IJobUi
    {
        private Job _job;
        private int _jobId = -1;
        private StatusMessage _messageControl;

        public StatusMessage MessageControl
        {
            get
            {
                return _messageControl;
            }
            set
            {
                _messageControl = value;
            }
        }

        public Job NewsletterJob
        {
            get
            {
                if (_job == null)
                {
                    if (JobId > 0)
                        _job = BVNetwork.EPiSendMail.DataAccess.Job.Load(JobId);
                }
                return _job;
            }
        }

        public int JobId
        {
            get
            {
                if (_jobId == -1)
                    _jobId = GetJobIdFromQuery();

                return _jobId;
            }
        }

        protected int GetJobIdFromQuery()
        {
            string id = Request.QueryString["jobId"];

            if (string.IsNullOrEmpty(id))
                id = "0";

            return int.Parse(id);
        }

        /// <summary>
        /// Shows an error message on the page.
        /// </summary>
        /// <param name="message">Error message.</param>
        public void ShowError(string message)
        {
            MessageControl.ErrorMessage = message;
        }

        /// <summary>
        /// Shows an info message on the page.
        /// </summary>
        /// <param name="infoMessage">The info message.</param>
        public void ShowInfo(string message)
        {
            MessageControl.InfoMessage = message;
        }


    }
}
