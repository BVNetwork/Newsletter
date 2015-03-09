using System;
using System.Collections.Generic;
using System.Web.Security;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin.RecipientItemProviders
{
    public partial class EPiServerGroupProvider : System.Web.UI.UserControl, IRecipientItemProvider
    {
        private RecipientList _list;
        private IRecipientListUi _listUi;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            LoadEPiGroups();
        }

        private void LoadEPiGroups()
        {
            if (dropListEPiGroups.Items.Count == 0)
            {
                // Fill it, but only first time
                dropListEPiGroups.DataSource = Roles.GetAllRoles();
                dropListEPiGroups.DataBind();
            }
        }

        protected void cmdAddEPiGroupEmailAddresses_Click(object sender, EventArgs e)
        {
            List<string> addresses = new List<string>();

            string selectedRoleName = dropListEPiGroups.SelectedValue;
            string[] usersInRole = Roles.GetUsersInRole(selectedRoleName);

            foreach (string userName in usersInRole)
            {
                MembershipUser user = Membership.GetUser(userName);
                if (user != null && string.IsNullOrEmpty(user.Email) == false)
                    addresses.Add(user.Email);
            }

            if (addresses.Count == 0)
            {
                _listUi.ShowInfo("Could not find any email addresses ");
                return;
            }            

            // Add the items
            List<string> duplicateAddresses;
            List<string> invalidAddresses;

            System.Diagnostics.Stopwatch tmr = System.Diagnostics.Stopwatch.StartNew();
            int count = _list.ImportEmailAddresses(addresses.ToArray(), out invalidAddresses, out duplicateAddresses);
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

            _listUi.ShowInfo(string.Format(message,
                                        count.ToString(),
                                        selectedRoleName,
                                        duplicateAddresses.Count.ToString(),
                                        invalidAddresses.Count.ToString(),
                                        invalidMessage
                                ));
        }

        #region IRecipientItemProvider Members

        public void Initialize(BVNetwork.EPiSendMail.DataAccess.RecipientList list, IRecipientListUi listUi)
        {
            _list = list;
            _listUi = listUi;
        }

        #endregion
    }
}