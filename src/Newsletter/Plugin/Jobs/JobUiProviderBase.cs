using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.Plugin.ItemProviders;
using BVNetwork.EPiSendMail.Plugin.WorkItemProviders;

namespace BVNetwork.EPiSendMail.Plugin
{
    public abstract class JobUiProviderBase : JobUiUserControlBase
    {
        const string PROVIDER_SUFFIX = "Provider";
        private readonly List<RecipientListProviderDescriptor> _workItemProviders = new List<RecipientListProviderDescriptor>();
        private readonly Dictionary<string, Control> _providerCtrls = new Dictionary<string, Control>();

        public List<RecipientListProviderDescriptor> WorkItemProviders
        {
            get
            {
                return _workItemProviders;
            }
        }

        public Dictionary<string, Control> ProviderControls
        {
            get
            {
                return _providerCtrls;
            }
        }

        public abstract Control ProviderContainer
        {
            get;
        }

        public abstract Control UiContainer
        {
            get;
        }

        protected void LoadProviderControls()
        {
            // Add controls from descriptors
            foreach (RecipientListProviderDescriptor provider in WorkItemProviders)
            {
                Control ctrl = null;
                ctrl = Page.LoadControl(provider.UserControlUrl);
                ctrl.ID = provider.ProviderName + PROVIDER_SUFFIX;
                ctrl.Visible = false;
                _providerCtrls.Add(provider.ProviderName, ctrl);
            }
        }

        protected abstract void InitializeWorkItemDescriptors();

        protected void AddProvidersToUiContainer()
        {
            // Hide the other controls
            foreach (KeyValuePair<string, Control> keyValue in ProviderControls)
            {
                ProviderContainer.Controls.Add(keyValue.Value);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Let inheritor describe the providers for us
            InitializeWorkItemDescriptors();

            // Load user controls, adding them to the page
            LoadProviderControls();

            // Add controls to its container
            AddProvidersToUiContainer();
           
        }

        protected void AddProviderWithChecks(List<RecipientListProviderDescriptor> providers, RecipientListProviderDescriptor provider)
        {
            if (provider.ProviderControlExists == true)
                providers.Add(provider);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
             // We need to make sure we load the provider for
            // postbacks too, to make sure they are able to
            // catch any events
            ShowWorkItemProvider();
           
        }

        protected void ShowWorkItemProvider()
        {
            if (CurrentProvider == null)
            {
                UiContainer.Visible = false;
                return;
            }

            Control ctrl = null;
            if (!_providerCtrls.ContainsKey(CurrentProvider))
                return;
            ctrl = _providerCtrls[CurrentProvider];
            ctrl.Visible = true;
            // Initialize provider
            ((IEmailImporterProvider)ctrl).Initialize(NewsletterJob, this);
            // Databind it, to have it load it's values
            ctrl.DataBind();

            UiContainer.Visible = true;

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
                return (string)Session["CurrentSelection"];//(string)ViewState["CurrentSelection"];
            }
            set
            {
                Session["CurrentSelection"] = value;
                //ViewState["CurrentSelection"] = value;
            }
        }

        protected void ProviderCommandClick(object sender, CommandEventArgs e)
        {
            if (e.CommandName == CurrentProvider)
                return;

            CurrentProvider = e.CommandName;

            // Bind here
            ShowWorkItemProvider();
        }
 
    }
}
