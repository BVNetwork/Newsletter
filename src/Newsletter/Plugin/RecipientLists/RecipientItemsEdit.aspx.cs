using System;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class RecipientItemsEdit : RecipientListUiPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            BindData();
        }

        public void BindData()
        {
            if (RecipientList != null)
            {
                lblName.Text = RecipientList.Name;
                lblDescription.Text = RecipientList.Description;
                lblItemCount.Text = RecipientList.EmailAddressCount.ToString();
            }
        }

        public void BindWorkItemData(EmailAddresses items)
        {
            grdItems.DataSource = items;
            grdItems.DataBind();
        }

        protected void grdWorkItemsItemCommandHandler(object sender, DataGridCommandEventArgs e)
        {
            if (string.Compare(e.CommandName, "delete", true /* Ignore Case */) == 0)
            {
                // Delete it
                string email = e.Item.Cells[0].Text;
                // Remove from collection
                RecipientList.RemoveEmailAddresses(email);

                // Rebind Data
                if (string.IsNullOrEmpty(txtSearchFor.Text.Trim()))
                {
                    BindData();
                    BindWorkItemData(this.RecipientList.EmailAddresses);
                }
                else
                    cmdSearchFor_Click(null, null);
            }
            
        }

        protected void cmdSearchFor_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearchFor.Text.Trim()))
                return;
            EmailAddresses items = RecipientList.Search(RecipientList.Id, "%" + txtSearchFor.Text + "%");
            BindWorkItemData(items);

            //JobWorkItems items = JobWorkItems.Search(NewsletterJob.Id, "%" + txtSearchFor.Text + "%");
            //BindWorkItemData(items);
        }

        protected void lnkShowAll_Click(object sender, EventArgs e)
        {
            // Bind all
            BindWorkItemData(this.RecipientList.EmailAddresses);
        }

    }
}
