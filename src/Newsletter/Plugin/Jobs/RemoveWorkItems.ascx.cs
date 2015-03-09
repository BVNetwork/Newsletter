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

            WorkItemProviderDescriptor recipientListProvider =
                new WorkItemProviderDescriptor("RecipientList",
                                               "Select one of your Block Lists",
                                               BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/workitemproviders/RecipientRemoveprovider.ascx");

            AddProviderWithChecks(WorkItemProviders, recipientListProvider);

            WorkItemProviderDescriptor textProvider =
                new WorkItemProviderDescriptor("TextImportRemove",
                                               "Remove from manually entered email addresses",
                                               BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/workitemproviders/TextRemoveProvider.ascx");
            AddProviderWithChecks(WorkItemProviders, textProvider);

            //WorkItemProviderDescriptor sidGroupProvider =
            //    new WorkItemProviderDescriptor("EPiServerGroup",
            //                           "Add email addresses from an EPiServer Group",
            //                           BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/workitemproviders/EPiServerGroupProvider.ascx");
            //AddProviderWithChecks(_workItemProviders, sidGroupProvider);

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