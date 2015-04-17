using System;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Plugin.ItemProviders;

namespace BVNetwork.EPiSendMail.Plugin.WorkItemProviders
{
    public partial class RecipientRemoveProvider : System.Web.UI.UserControl, IEmailImporterProvider
    {
        private Job _job;
        private IShowFeedback _jobUi;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Do not do databind here - the container will do this for us
        }

        protected override void OnDataBinding(EventArgs e)
        {

            base.OnDataBinding(e);
            if (!ShowAll)
            {

                rptRemoveFromRecipientLists.DataSource = RecipientLists.ListOneType("BlockList");
                rptRemoveFromRecipientLists.DataBind();
            }
            else
            {
                rptRemoveFromRecipientLists.DataSource = RecipientLists.ListAll();
                rptRemoveFromRecipientLists.DataBind();
            }
        }

        /// <summary>
        /// Remove a reicipent list from the job.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdSelect_Click(object sender, CommandEventArgs e)
        {
            string recipListIdString = (string)e.CommandArgument;
            int recipListId = int.Parse(recipListIdString);
            RecipientList list = RecipientList.Load(recipListId);

            // Add the items
            int count = _job.FilterOnRecipients(recipListId);
            _jobUi.ShowInfo("Removed " + count.ToString() + " Recipients from Recipient List " + list.Name);

        }

        /// <summary>
        /// Default only block lists is showed, but user can click on show all link and 
        /// all the recipients lists will be showed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdShowAll_Click(object sender, EventArgs e)
        {
            // We need to set this before the databind, as it will
            // be evaluated during the databind
            ShowAll = true;
            rptRemoveFromRecipientLists.DataSource = RecipientLists.ListAll();
            rptRemoveFromRecipientLists.DataBind();
        }

        protected bool ShowAll
        {
            get
            {
                return ViewState["ShowAll"] == null ? false : (bool)ViewState["ShowAll"];
            }
            set
            {
                ViewState["ShowAll"] = value;
            }
        }

        public void Initialize(IEmailImporter importer, IShowFeedback feedbackUi)
        {
            _job = (Job)importer;
            _jobUi = feedbackUi;
        }
    }
}