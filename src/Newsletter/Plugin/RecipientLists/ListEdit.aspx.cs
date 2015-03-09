using System;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin
{

	public partial class RecipientListEdit : RecipientListUiPageBase
    {
        protected StatusMessage ucStatusMessage;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            // Stylesheet
			if (!IsPostBack)
			{
				DataBind();
			}
		}

	    protected override void OnPreRender(EventArgs e)
	    {
	        base.OnPreRender(e);

            if (RecipientList != null)
                BindJobData(RecipientList);
            else
            {
                // Hide panels that should now be visible when
                // creating a new list
            }

	    }


	    public void BindJobData(RecipientList list)
        {
            h2ListName.InnerText = list.Name;
            lblListDescription.Text = list.Description;
            lblCountOfEmails.Text = list.EmailAddressCount.ToString();

            lnkEditItems.HRef = "RecipientItemsEdit.aspx?recipientlistid=" + this.RecipientList.Id.ToString();

            //if (NewsletterJob.PageId > 0)
            //    prPageToSend.PageLink = new EPiServer.Core.PageReference(job.PageId);
            
        }

        /// <summary>
        /// Handles the Click event of the lnkRemoveList control.
        /// </summary>
        /// <remarks>
        /// Delete the list, and redirects to a "deleted information" page
        /// </remarks>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void lnkRemoveList_Click(object sender, EventArgs e)
        {
            this.RecipientList.Delete();
            Response.Redirect("RecipientListDeleted.aspx");
        }

        protected void lnkRemoveContents_Click(object sender, EventArgs e)
        {
            if (RecipientList != null)
            {
                RecipientList.DeleteEmailAddressItems();
                // Rebind data
                BindJobData(RecipientList);
            }
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
            // Tell base where to show messages
            base.MessageControl = this.ucStatusMessage;
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

	}
}
