using System.Web.Http;
using BVNetwork.EPiSendMail.Api.Models;
using BVNetwork.EPiSendMail.Plugin.WorkItemProviders;

namespace BVNetwork.EPiSendMail.Api
{
    [Authorize(Roles = "NewsletterEditors, CmsAdmins")]
    public class RecipientsController : ApiController
    {
        [HttpPost]
        public RecipientStatus AddRecipientsFromEPiServerGroupname(int jobId, string groupName)
        {
            EPiServerGroupProvider recipients = new EPiServerGroupProvider();

            RecipientStatus status = recipients.AddEPiServerGroupRecipients(jobId, groupName);

            return status;
        }
    }
}
