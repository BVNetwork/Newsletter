using System;
using System.Collections;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Library;
using EPiServer;

namespace BVNetwork.EPiSendMail.Units
{

	public abstract class Unsubscribe : UserControlBase
	{
		protected System.Web.UI.WebControls.TextBox txtEmail;
		protected System.Web.UI.WebControls.CheckBoxList chkNewsLetterLists;
		protected System.Web.UI.WebControls.Panel pnlError;
		protected System.Web.UI.WebControls.Literal lblError;
		protected System.Web.UI.WebControls.Literal lblMessage;
		protected System.Web.UI.WebControls.Panel pnlMessage;
		protected System.Web.UI.WebControls.Button cmdUnsubscribe;
		protected Repeater rptResult;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				if (Request["subscribeemail"] != null)
				{
					txtEmail.Text = Request["subscribeemail"];	
				}

                //Gets all the public lists and show them in a checkbox list.
                RecipientLists lists = RecipientLists.ListOneType("PublicList");
                foreach (RecipientList list in lists)
                {
                    ListItem itm = new ListItem(list.Name, list.Id.ToString());
                    itm.Selected = false;
                    chkNewsLetterLists.Items.Add(itm);
                }
			}

		}
		public void AddErrorMessage(string text)
		{
			lblError.Text += text + "<br />";
			pnlError.Visible = true;
		}

		public void AddMessage(string text)
		{
			lblMessage.Text += text + "<br />";
			pnlMessage.Visible = true;
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cmdUnsubscribe.Click += new System.EventHandler(this.cmdUnsubscribe_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void cmdUnsubscribe_Click(object sender, System.EventArgs e)
		{
			string emailAddress = txtEmail.Text;

			if (emailAddress == null || emailAddress == string.Empty)
			{
				AddErrorMessage(Translate("/bvnetwork/sendmail/unsubscribe/errornoemail"));
				return;
			}

			emailAddress = emailAddress.Trim();
			EPiMailEngine engine = new EPiMailEngine();
			ArrayList resultArray = new ArrayList();
			
            EmailAddresses importedItems = new EmailAddresses();

            foreach (ListItem itm in this.chkNewsLetterLists.Items)
            {
                if (itm.Selected)
                {
                    //Load the selected recipient list
                    RecipientList list = RecipientList.Load(Convert.ToInt32(itm.Value));

                    SubscriptionStatus status = new SubscriptionStatus();
                    status.RecipientListName = list.Name;
                    //load user email address
                    EmailAddress emailItem = EmailAddress.Load(list.Id, emailAddress);
                    if (emailItem != null)
                    {
                        //Delete the user from the list, and show a confirm message
                        emailItem.Delete();
                        status.SubscriptionResult = true;
                        status.Message = Translate("/bvnetwork/sendmail/unsubscribe/recipientremoved");
                    }
                    else
                    {
                        status.SubscriptionResult = false;
                        status.Message = Translate("/bvnetwork/sendmail/unsubscribe/nosubscriptionfound");

                    }
                    //add the result to the array list.
                    resultArray.Add(status);
                }
            }
			// Done adding, now show the result
			rptResult.DataSource = resultArray; 
			rptResult.DataBind();

		}

	}
}
