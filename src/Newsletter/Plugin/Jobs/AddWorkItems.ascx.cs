using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.Configuration;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class AddWorkItems : JobUiProviderBase
    {
        public override Control ProviderContainer
        {
            get
            {
                return pnlProviderContainer;
            }
        }

        public override Control UiContainer
        {
            get { 
                return pnlProviderUiContainer; 
            }
        }

        protected override void InitializeWorkItemDescriptors()
        {
            LocalizationService localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();

            NameValueCollection config = NewsLetterConfiguration.GetRecipientListProviders();
            if (config != null)
            {
                foreach (string item in config)
                {
                    WorkItemProviderDescriptor provider = new WorkItemProviderDescriptor(item,
                                       localizationService.GetString("/newsletterRecipientProvider/" + item, item),
                                       config[item]);
                    AddProviderWithChecks(WorkItemProviders, provider);
                }
            }

            // Build the provider list, this will be configurable later
            WorkItemProviderDescriptor recipientListProvider =
                new WorkItemProviderDescriptor("RecipientList",
                                               "Select one of your Recipient Lists",
                                               BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/workitemproviders/recipientprovider.ascx");

            AddProviderWithChecks(WorkItemProviders, recipientListProvider);

            WorkItemProviderDescriptor textProvider =
                new WorkItemProviderDescriptor("TextImport",
                                               "Import from text",
                                               BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/workitemproviders/TextImportProvider.ascx");
            AddProviderWithChecks(WorkItemProviders, textProvider);

            WorkItemProviderDescriptor sidGroupProvider =
                new WorkItemProviderDescriptor("EPiServerGroup",
                                       "Add email addresses from an EPiServer Group",
                                       BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/workitemproviders/EPiServerGroupProvider.ascx");
            AddProviderWithChecks(WorkItemProviders, sidGroupProvider);

            WorkItemProviderDescriptor commerceUsersProvider =
               new WorkItemProviderDescriptor("CommerceUsers",
                                      "Add users from Commerce",
                                      BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/workitemproviders/CommerceUsersProvider.ascx");
            AddProviderWithChecks(WorkItemProviders, commerceUsersProvider);

        }

        protected void Page_Load(object sender, EventArgs e)
        {

            pnlImportProviders.DataBind();
            
        }

        protected void cmdImportWorkItems_Click(object sender, CommandEventArgs e)
        {
            base.ProviderCommandClick(sender, e);
        }
    }
}