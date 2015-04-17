using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class RemoveWorkItems : JobUiProviderBase
    {

        public override Control ProviderContainer
        {
            get {
                return pnlProviderContainer;
            }
        }

        public override Control UiContainer
        {
            get
            {
                return pnlProviderUiContainer;
            }
        }

        protected override void InitializeWorkItemDescriptors()
        {

            // Build the provider list, this will be configurable later

            RecipientListProviderDescriptor recipientListProvider =
                new RecipientListProviderDescriptor("RecipientList",
                                               "Select one of your Block Lists",
                                               BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/workitemproviders/RecipientRemoveprovider.ascx");

            AddProviderWithChecks(WorkItemProviders, recipientListProvider);

            RecipientListProviderDescriptor textProvider =
                new RecipientListProviderDescriptor("TextImportRemove",
                                               "Remove from manually entered email addresses",
                                               BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/workitemproviders/TextRemoveProvider.ascx");
            AddProviderWithChecks(WorkItemProviders, textProvider);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Databind providers
            pnlProviders.DataBind();
        }

        protected void cmdWorkItems_Click(object sender, CommandEventArgs e)
        {
            base.ProviderCommandClick(sender, e);
        }
    }
}