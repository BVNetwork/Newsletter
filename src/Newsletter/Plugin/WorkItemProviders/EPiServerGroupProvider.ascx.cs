using System;
using System.Collections.Generic;
using System.Web.Security;
using BVNetwork.EPiSendMail.Api.Models;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Library;

namespace BVNetwork.EPiSendMail.Plugin.WorkItemProviders
{
    public partial class EPiServerGroupProvider : JobUiUserControlBase, IWorkItemProvider
    {

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

        public RecipientStatus AddEPiServerGroupRecipients(int jobId,string groupName)
        {
            Job _job = Job.Load(jobId);
            RecipientStatus status = new RecipientStatus();

            List<string> addresses = new List<string>();
            string[] usersInRole = Roles.GetUsersInRole(groupName);

            foreach (string userName in usersInRole)
            {
                MembershipUser user = Membership.GetUser(userName);
                if (user != null && string.IsNullOrEmpty(user.Email) == false)
                    addresses.Add(user.Email);
            }

            if (addresses.Count == 0)
            {
                status.Status = "Could not find any email addresses for users in the EPiServer group.";
                return status;
            }

            // Add the items
            List<string> duplicateAddresses;
            List<string> invalidAddresses;

            System.Diagnostics.Stopwatch tmr = System.Diagnostics.Stopwatch.StartNew();
            int count = _job.ImportEmailAddresses(addresses.ToArray(), out invalidAddresses, out duplicateAddresses);
            tmr.Stop();

            if (invalidAddresses.Count > 0)
            {
                status.InvalidMessage = string.Join(", ", invalidAddresses.ToArray());
            }

            status.ImportedEmails = count;
            status.DuplicatedEmails = duplicateAddresses.Count;
            status.InvalidEmails = invalidAddresses.Count;
            status.TimeToImport = tmr.ElapsedMilliseconds;
            status.Status = "Import ok";
            
            return status;
        }


        public void Initialize(Job job, IJobUi jobUi)
        {
            // No need for Job and IJobUI when doing ajax calls.
            //throw new NotImplementedException();
        }
    }

}