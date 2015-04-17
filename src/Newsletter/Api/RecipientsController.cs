using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using BVNetwork.EPiSendMail.Api.Models;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Plugin.WorkItemProviders;

namespace BVNetwork.EPiSendMail.Api
{
    [Authorize(Roles = "NewsletterEditors, CmsAdmins")]
    public class RecipientsController : ApiBaseController
    {
        [HttpPost]
        public HttpResponseMessage AddRecipientsToJobFromEPiServerGroupname(int id, string groupName)
        {
            Job job = Job.Load(id);
            if(job != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, 
                    GetJsonResult<RecipientStatus>(AddEPiServerGroupRecipients(job, groupName)));
            }
            
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Job not found");
        }

        [HttpPost]
        public HttpResponseMessage AddRecipientsToListFromEPiServerGroupname(int id, string groupName)
        {
            RecipientList list = RecipientList.Load(id);
            if(list != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, 
                    GetJsonResult<RecipientStatus>(AddEPiServerGroupRecipients(list, groupName)));
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "List not found");
        }

        [HttpPost]
        public HttpResponseMessage AddRecipientsToListFromList(int sourceListId, int destinationListId)
        {
            RecipientList destList = RecipientList.Load(destinationListId);
            RecipientList sourceList = RecipientList.Load(sourceListId);
            if (destList != null && sourceList != null)
            {
                int addedAddresses = destList.AddRecipientItemsFromRecipientList(sourceList.Id);
                RecipientStatus status = new RecipientStatus {ImportedEmails = addedAddresses};

                return Request.CreateResponse(HttpStatusCode.OK,
                    GetJsonResult<RecipientStatus>(status));
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Source or Destination list not found");
        }

        [HttpPost]
        public HttpResponseMessage AddRecipientsToJobFromList(int sourceListId, int jobId)
        {
            Job job = Job.Load(jobId);
            if (job == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Job not found");
            }
            RecipientList sourceList = RecipientList.Load(sourceListId);
            if (sourceList == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Source list not found");
            }

            int addedAddresses = job.AddWorkItemsFromRecipientList(sourceList.Id);
            RecipientStatus status = new RecipientStatus {ImportedEmails = addedAddresses};

            return Request.CreateResponse(HttpStatusCode.OK, GetJsonResult<RecipientStatus>(status));
        }


        protected RecipientStatus AddEPiServerGroupRecipients(IEmailImporter importer, string groupName)
        {
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
            int count = importer.ImportEmailAddresses(addresses.ToArray(), out invalidAddresses, out duplicateAddresses);
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

    }
}
