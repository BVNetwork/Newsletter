using System;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin.WorkItemProviders
{
    public partial class RecipientProvider : System.Web.UI.UserControl, IWorkItemProvider
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
            rptAddFromRecipientLists.DataSource = RecipientLists.ListAll();
            rptAddFromRecipientLists.DataBind();
        }

        protected void cmdSelect_Click(object sender, CommandEventArgs e)
        {
            string recipListIdString = (string)e.CommandArgument;
            int recipListId = int.Parse(recipListIdString);
            RecipientList list = RecipientList.Load(recipListId);
            
            // Add the items
            int count = _job.AddWorkItemsFromRecipientList(recipListId);
            _jobUi.ShowInfo("Added " + count.ToString() + " Recipients from Recipient List " + list.Name);

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