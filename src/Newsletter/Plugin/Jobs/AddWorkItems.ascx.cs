using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Plugin.ItemProviders;
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

            List<RecipientListProviderDescriptor> recipientProviders = NewsLetterConfiguration.GetRecipientListProviderDescriptors();

            foreach (RecipientListProviderDescriptor descriptor in recipientProviders)
            {
                AddProviderWithChecks(WorkItemProviders, descriptor);
            }

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