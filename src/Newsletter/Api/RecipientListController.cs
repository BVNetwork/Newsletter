using System.Net;
using System.Net.Http;
using System.Web.Http;
using BVNetwork.EPiSendMail.Api.Models;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Plugin.WorkItemProviders;
using Newtonsoft.Json.Linq;

namespace BVNetwork.EPiSendMail.Api
{
    [Authorize(Roles = "NewsletterEditors, CmsAdmins")]
    public class RecipientListController : ApiBaseController
    {

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            RecipientList list = RecipientList.Load(id);
            if (list == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, GetJsonResult<RecipientList>(list));
        }

        /// <summary>
        /// Deletes the specified Recipient List.
        /// </summary>
        /// <param name="id">The list id.</param>
        /// <returns>True if the delete was ok, false if not</returns>
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            RecipientList list = RecipientList.Load(id);
            if (list == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            list.Delete();
        
            return Request.CreateResponse(HttpStatusCode.OK, "Recipient list deleted");

        }
    }
}
