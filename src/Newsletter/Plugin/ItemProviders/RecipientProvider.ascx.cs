using System;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Plugin.ItemProviders;

namespace BVNetwork.EPiSendMail.Plugin.ItemProviders
{
    public partial class RecipientProvider : System.Web.UI.UserControl, IEmailImporterProvider
    {
        public string ApiUrl = null;

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

        public void Initialize(IEmailImporter importer, IShowFeedback feedbackUi)
        {
            // Depending on what we're adding addresses to, we need to call different controllers
            string apiUrl = NewsLetterConfiguration.GetModuleBaseDir() + "/api/recipients/";
            int id = 0;

            if (importer is Job)
            {
                apiUrl = apiUrl + "AddRecipientsToJobFromList";
                id = ((Job)importer).Id;
                apiUrl = apiUrl + "?jobId=" + id.ToString();
            }
            else
            {
                // We're adding from a list to another listg
                apiUrl = apiUrl + "AddRecipientsToListFromList";
                id = ((RecipientList)importer).Id;
                apiUrl = apiUrl + "?destinationListId=" + id.ToString();
            }

            ApiUrl = apiUrl;
        }


    }
}