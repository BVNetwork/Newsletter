using System;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin
{

    public partial class NewRecipientList : RecipientListUiPageBase
	{
    

        protected void Page_Load(object sender, System.EventArgs e)
		{
    
		}


        private void DoTheBinding()
        {
            //Retrives all Recipient List Types and bind them to dropdown list
            Array recipientListTypesValues = Enum.GetValues(typeof(RecipientListType));
            for (int i = 0; i < recipientListTypesValues.Length; i++)
            {
                dropListRecipientTypes.Items.Add(new ListItem(
                        NewsLetterUtil.GetEnumLanguageName((RecipientListType)recipientListTypesValues.GetValue(i)), //languagespecific name
                                ((int)recipientListTypesValues.GetValue(i)).ToString())); 
            }
            dropListRecipientTypes.DataBind();
        }

        protected void cmdSaveNewRecipientList_ClickHandler(object sender, EventArgs e)
        {
            string name = txtRecipientListName.Text;
            string desc = txtRecipientListDesc.Text;

            if (string.IsNullOrEmpty(name))
                ShowError("Name cannot be empty");

            // Create and Save
            RecipientList newList = new RecipientList((RecipientListType)Int32.Parse(dropListRecipientTypes.SelectedValue), name, desc);
            newList.Save();

            //Redirect to the new recipient list page
            if (newList.Id != 0)
                Response.Redirect(BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/recipientlists/listedit.aspx?recipientlistid=" + newList.Id);
            else
                ShowError("Something went wrong saving new recipient list");
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
            if (!IsPostBack)
            {
                DoTheBinding();
            }
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);


            // Tell base where to show messages
            //base.MessageControl = this.ucStatusMessage;
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
