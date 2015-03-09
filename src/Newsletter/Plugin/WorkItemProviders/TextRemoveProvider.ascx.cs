using System;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin.WorkItemProviders
{
    public partial class TextRemoveProvider : System.Web.UI.UserControl, IWorkItemProvider
    {
        private Job _job;
        private IJobUi _jobUi;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Do not do databind here - the container will do this for us
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
        }

        protected void cmdRemoveCsvEmailAddresses_Click(object sender, EventArgs e)
        {
            string addresses = txtRemoveEmailWorkItems.Text;
            if (string.IsNullOrEmpty(addresses))
            {
                _jobUi.ShowError("Please enter email addresses to remove.");
                return;
            }
            
            JobWorkItems itemsToAttemptDelete = Job.ParseEmailAddressesToWorkItems(addresses);
            foreach (JobWorkItem item in itemsToAttemptDelete)
            {
                item.JobId = _job.Id;
                item.Delete();
            }

            // Construct log message
            string message = "Removed email addresses";
            _jobUi.ShowInfo(string.Format(message)); 
        }

        #region IWorkItemProvider Members

        public void Initialize(BVNetwork.EPiSendMail.DataAccess.Job job, IJobUi jobUi)
        {
            _job = job;
            _jobUi = jobUi;
        }

        #endregion
    }
}