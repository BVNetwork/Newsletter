using System;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class WorkItemsEdit : JobUiPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindData();
        }

        public void BindData()
        {
            if (NewsletterJob != null)
            {
                Job job = NewsletterJob;

                int notSent = job.GetWorkItemCountForStatus(JobWorkStatus.NotStarted);
                int failed = job.GetWorkItemCountForStatus(JobWorkStatus.Failed);
                int sending = job.GetWorkItemCountForStatus(JobWorkStatus.Sending);
                int sent = job.GetWorkItemCountForStatus(JobWorkStatus.Complete);
                // _leftSending = notSent + sending;

                lblJobName.Text = job.Name;
                lblDescription.Text = job.Description;
                lblWorkItemCount.Text = (notSent + failed + sending + sent).ToString();
            }
        }

        public void BindWorkItemData(JobWorkItems items)
        {
            grdWorkItems.DataSource = items;
            grdWorkItems.DataBind();
            lblGridCount.Text = items.Items.Count.ToString();
            pnlGridCount.Visible = true;
        }

        protected void grdWorkItemsItemCreatedHandler(object sender, DataGridItemEventArgs e)
        {
        }
        protected void grdWorkItemsItemDataBoundHandler(object sender, DataGridItemEventArgs e)
        {
        }
        protected void grdWorkItemsItemCommandHandler(object sender, DataGridCommandEventArgs e)
        {
            if (System.String.Compare(e.CommandName, "delete", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Delete it
                string email = e.Item.Cells[1].Text;
                JobWorkItem.Delete(NewsletterJob.Id, email);

                // Rebind Data
                //JobWorkItem removedItem = NewsletterJob.WorkItems.Find(email);
                //if (removedItem != null)
                //    NewsletterJob.WorkItems.Remove(removedItem);
                if (string.IsNullOrEmpty(txtSearchFor.Text.Trim()))
                {
                    BindWorkItemData(NewsletterJob.GetWorkItems());
                    BindData();
                }
                else
                    cmdSearchFor_Click(null, null);
            }
            
        }

        protected void cmdSearchFor_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearchFor.Text.Trim()))
                return;

            JobWorkItems items = JobWorkItems.Search(NewsletterJob.Id, "%" + txtSearchFor.Text + "%");
            BindWorkItemData(items);
        }

        protected void lnkShowAll_Click(object sender, EventArgs e)
        {
            // Bind all
            BindWorkItemData(NewsletterJob.GetWorkItems());
        }

        /// <summary>
        /// Handles the Click event of the lnkShowError control. Shows all work items
        /// with the status of error
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void lnkShowError_Click(object sender, EventArgs e)
        {
            JobWorkItems items = JobWorkItems.ListAll(NewsletterJob, JobWorkStatus.Failed);
            BindWorkItemData(items);
        }

        protected void lnkShowNotSent_Click(object sender, EventArgs e)
        {
            JobWorkItems items = JobWorkItems.ListAll(NewsletterJob, JobWorkStatus.NotStarted);
            BindWorkItemData(items);
        }

        protected void lnkShowSent_Click(object sender, EventArgs e)
        {
            JobWorkItems items = JobWorkItems.ListAll(NewsletterJob, JobWorkStatus.Complete);
            BindWorkItemData(items);
        }

        protected void lnkShowSending_Click(object sender, EventArgs e)
        {
            JobWorkItems items = JobWorkItems.ListAll(NewsletterJob, JobWorkStatus.Sending);
            BindWorkItemData(items);
        }
        
        
    }
}
