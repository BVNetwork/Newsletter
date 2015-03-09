using System;
using System.Web.UI.WebControls;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class MiscActions : JobUiUserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (NewsletterJob != null)
            {
                lnkWorkItems.NavigateUrl =
                        BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() +
                        "/plugin/jobs/workitemsedit.aspx?jobid=" + NewsletterJob.Id.ToString();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
        }

        protected void cmdActionClickHandler(object sender, CommandEventArgs e)
        {
            string cmd = e.CommandName;
            switch (cmd)
            {
                case "RemoveAllWorkItems":
                    base.NewsletterJob.DeleteAllWorkItems();
                    ShowInfo("Removed all recipients from the job");
                    break;
                case "ResetWorkItemStatus":
                    // Set status to not started
                    base.NewsletterJob.SetStatusForWorkItems(EPiSendMail.DataAccess.JobWorkStatus.NotStarted);
                    
                    // Set job status to editing
                    base.NewsletterJob.Status = BVNetwork.EPiSendMail.DataAccess.JobStatus.Editing;
                    base.NewsletterJob.Save();

                    ShowInfo("All newsletter recipients has been reset to the Not Sent status");
                    break;
            }
            
        }
    }
}