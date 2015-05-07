using System;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Plugin.ItemProviders;

namespace BVNetwork.EPiSendMail.Plugin.WorkItemProviders
{
    public partial class TextRemoveProvider : System.Web.UI.UserControl, IEmailImporterProvider
    {
        private IEmailImporter _importer;
        private IShowFeedback _feedbackCtrl;

        private Job _job;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Do not do databind here - the container will do this for us
        }

        protected void cmdRemoveCsvEmailAddresses_Click(object sender, EventArgs e)
        {
            string addresses = txtRemoveEmailWorkItems.Text;
            if (string.IsNullOrEmpty(addresses))
            {
                _feedbackCtrl.ShowError("Please enter email addresses to remove.");
                return;
            }
            
            JobWorkItems itemsToAttemptDelete = Job.ParseEmailAddressesToWorkItems(addresses);
            foreach (JobWorkItem item in itemsToAttemptDelete)
            {
                item.JobId = _job.Id;
                item.Delete();
            }

            _feedbackCtrl.ShowInfo("Removed email addresses"); 
        }

        public void Initialize(IEmailImporter importer, IShowFeedback feedbackUi)
        {
            _importer = importer;
            _job = (Job) importer;
            _feedbackCtrl = feedbackUi;
        }
    }
}