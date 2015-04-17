using System;
using System.Collections;
using System.Collections.Generic;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Plugin;
using BVNetwork.EPiSendMail.Plugin.ItemProviders;
using BVNetwork.EPiSendMail.Plugin.WorkItemProviders;
using EPiServer.Framework.Localization;
using Mediachase.BusinessFoundation.Core;
using Mediachase.BusinessFoundation.Data.Business;
using Mediachase.Commerce.Security;

namespace BVNetwork.EPiSendMail.CommerceProvider.Plugin.ItemProviders
{
    public partial class CommerceUsersProvider : System.Web.UI.UserControl, IEmailImporterProvider
    {
        private IEmailImporter _job;
        private IShowFeedback _jobUi;
        private const string MetaClassName = "Contact";

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            LoadCommerceProfileViewsGroups();
        }

        private void LoadCommerceProfileViewsGroups()
        {
            ArrayList arrayList = new ArrayList();
            if (dropListUserViews.Items.Count == 0)
            {
                ListViewProfile[] systemProfiles = ListViewProfile.GetSystemProfiles(MetaClassName, "EntityList");
                List<KeyValuePair<string, string>> allCustomerViews = new List<KeyValuePair<string, string>>();
                foreach (ListViewProfile listViewProfile in systemProfiles)
                {
                    arrayList.Add((object) listViewProfile.Id);
                    string name = listViewProfile.Name;
                    if (name.StartsWith("{"))
                    {
                        name = name.Replace("{", "");
                        name = name.Replace("}", "");
                        name = name.Replace(":", "_");
                        name = LocalizationService.Current.GetString("/bvnetwork/sendmail/plugin/commerce/" + name);
                    }
                    allCustomerViews.Add(new KeyValuePair<string, string>(listViewProfile.Id, "  " + name));
                }
                ListViewProfile[] userProfiles = ListViewProfile.GetProfiles(MetaClassName, "EntityList",
                    SecurityContext.Current.CurrentContactId);
                foreach (ListViewProfile listViewProfile in userProfiles)
                {
                    if (!arrayList.Contains((object) listViewProfile.Id))
                        allCustomerViews.Add(new KeyValuePair<string, string>(listViewProfile.Id,
                            "  " + listViewProfile.Name));
                }
                dropListUserViews.DataSource = allCustomerViews;
                dropListUserViews.DataTextField = "Value";
                dropListUserViews.DataValueField = "Key";
                dropListUserViews.DataBind();
            }
        }

        protected void cmdCustomerViewEmailAddresses_Click(object sender, EventArgs e)
        {
            ListViewProfile selectedProfile = ListViewProfile.Load(MetaClassName, dropListUserViews.SelectedValue, "EntityList");

            EntityObject[] entityObjectArray = BusinessManager.List(MetaClassName, selectedProfile.Filters.ToArray());

            List<string> addresses = new List<string>();
            foreach (var entityObject in entityObjectArray)
            {
                if (entityObject.Properties["Email"] != null)
                {
                    string email = entityObject.Properties["Email"].Value.ToString();
                    addresses.Add(email);
                }
            }
            if (addresses.Count == 0)
            {
                _jobUi.ShowInfo("Could not find any email addresses ");
                return;
            }

            // Add the items
            List<string> duplicateAddresses;
            List<string> invalidAddresses;

            System.Diagnostics.Stopwatch tmr = System.Diagnostics.Stopwatch.StartNew();
            int count = _job.ImportEmailAddresses(addresses.ToArray(), out invalidAddresses, out duplicateAddresses);
            tmr.Stop();

            string invalidMessage = "";
            if (invalidAddresses.Count > 0)
            {
                // Show invalid addresses
                invalidMessage = "<blockquote>\n";
                invalidMessage += string.Join(", ", invalidAddresses.ToArray());
                invalidMessage += "</blockquote>\n";
            }

            // Construct log message
            string message = "Imported {0} email addresses from \"{1}\". <br />\n" +
                             "Number of duplicates: {2}<br />\n" +
                             "Number of invalid addresses: {3} <br />\n" +
                             "{4}";

            _jobUi.ShowInfo(string.Format(message,
                                        count,
                                        dropListUserViews.SelectedItem.Text,
                                        duplicateAddresses.Count,
                                        invalidAddresses.Count,
                                        invalidMessage
                                ));

        }

        public void Initialize(IEmailImporter importer, IShowFeedback feedbackUi)
        {
            _job = importer;
            _jobUi = feedbackUi;
            
        }
    }
}