using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.Plugin.RecipientItemProviders;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class RemoveRecipients : RecipientListUiUserControlBase
    {
        private bool _addedCtrlForThisRequest = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            // We need to make sure we load the provider for
            // postbacks too, to make sure they are able to
            // catch any events
            ShowRecipientItemProvider();

        }

        public void ShowRecipientItemProvider()
        {
            if (CurrentProvider == null || _addedCtrlForThisRequest == true)
            {
                pnlProviderContainer.Visible = false;
                return;
            }

            Control ctrl = null;
            string baseDir = BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir();
            switch (CurrentProvider)
            {
            	case "RecipientList":
                    ctrl = Page.LoadControl(baseDir + "/plugin/recipientitemproviders/recipientprovider.ascx");
            		break;
                case "TextImport":
                    ctrl = Page.LoadControl(baseDir + "/plugin/recipientitemproviders/TextImportProvider.ascx");
                    break;
            }


            pnlProviderContainer.Visible = true;
            // Initialize provider
            ((IRecipientItemProvider)ctrl).Initialize(RecipientList, RecipientListBase);

            pnlProviderContainer.Controls.Add(ctrl);
            // Databind it, to have it load it's values
            ctrl.DataBind();

            _addedCtrlForThisRequest = true;
        }

        public string CurrentProvider
        {
            get
            {
                return (string)ViewState["CurrentSelection"];
            }
            set
            {
                ViewState["CurrentSelection"] = value;
            }
        }

        protected void cmdRemoveRecipients_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == CurrentProvider)
                return;

            CurrentProvider = e.CommandName;

            // Bind here
            ShowRecipientItemProvider();
        }
    }
}