using System;
using System.Web.Security;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;


namespace BVNetwork.EPiSendMail.Plugin.ItemProviders
{
    public partial class EPiServerGroupProvider : JobUiUserControlBase, IEmailImporterProvider
    {
        public string ApiUrl = null;

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            LoadEPiGroups();
        }

        private void LoadEPiGroups()
        {
            if (Roles.Enabled)
            {
                if (dropListEPiGroups.Items.Count == 0)
                {
                    // Fill it, but only first time 
                    dropListEPiGroups.DataSource = Roles.GetAllRoles();
                    dropListEPiGroups.DataBind();
                }
            }
        }

        public void Initialize(IEmailImporter importer, IShowFeedback feedbackUi)
        {
            // Depending on what we're adding addresses to, we need to call different controllers
            string apiUrl = NewsLetterConfiguration.GetModuleBaseDir() + "/api/recipients/";
            int id = 0;

            if (importer is Job)
            {
                apiUrl = apiUrl + "AddRecipientsToJobFromEPiServerGroupname";
                id = ((Job) importer).Id;
            }
            else
            {
                apiUrl = apiUrl + "AddRecipientsToListFromEPiServerGroupname";
                id = ((RecipientList)importer).Id;
            }
            apiUrl = apiUrl + "?id=" + id.ToString();

            ApiUrl = apiUrl;
        }
    }

}