using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.Plugin.RecipientItemProviders;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class AddRecipients : RecipientListUiUserControlBase
    {
        const string PROVIDER_SUFFIX = "Provider";
        private Dictionary<string, Control> _providerCtrls = new Dictionary<string, Control>();
        private List<RecipientListProviderDescriptor> _recipientProviders = new List<RecipientListProviderDescriptor>(); 

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _recipientProviders.Add(new RecipientListProviderDescriptor(
                "RecipientList",
                "Add from one of your Recipient Lists",
                BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/recipientitemproviders/recipientprovider.ascx"
            ));

            _recipientProviders.Add(new RecipientListProviderDescriptor(
                "TextImport",
                "Import from text",
                BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/recipientitemproviders/TextImportProvider.ascx"
            ));

            _recipientProviders.Add(new RecipientListProviderDescriptor(
            "EPiServerGroup",
            "Add email addresses from an EPiServer Group",
            BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/recipientitemproviders/EPiServerGroupProvider.ascx"
            ));

            _recipientProviders.Add(new RecipientListProviderDescriptor(
                "CommerceUsers",
                "Add email addresses from an EPiServer Group",
                BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/recipientitemproviders/CommerceUsersProvider.ascx"
            ));

            foreach (RecipientListProviderDescriptor descriptor in _recipientProviders)
            {
                if (descriptor.ProviderControlExists)
                {
                    Control ctrl = Page.LoadControl(descriptor.UserControlUrl);
                    ctrl.Visible = false;
                    ctrl.ID = descriptor.ProviderName + PROVIDER_SUFFIX;
                    _providerCtrls.Add(descriptor.ProviderName, ctrl);
                    pnlProviderContainer.Controls.Add(ctrl);
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Add the ones we could find
            lstRecipientProviders2.DataSource = _recipientProviders.Where(p => p.ProviderControlExists);
            lstRecipientProviders2.DataBind();

            ShowRecipientItemProvider();
        }

        public void ShowRecipientItemProvider()
        {
            if (CurrentProvider == null)
            {
                pnlProviderUiContainer.Visible = false;
                return;
            }

            Control ctrl = null;
            ctrl = _providerCtrls[CurrentProvider];
            ctrl.Visible = true;

            pnlProviderUiContainer.Visible = true;

            // Initialize provider
            ((IRecipientItemProvider)ctrl).Initialize(RecipientList, RecipientListBase);
            
            // Databind it, to have it load it's values
            ctrl.DataBind();

            // Hide the other controls
            foreach (KeyValuePair<string, Control> keyValue in _providerCtrls)
            {
                if (ctrl != keyValue.Value)
                {
                    keyValue.Value.Visible = false;
                }
            }

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

        protected void cmdImportRecipientItems_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == CurrentProvider)
                return;

            // Store for next time
            CurrentProvider = e.CommandName;

            // Bind here
            ShowRecipientItemProvider();
        }
    }
}