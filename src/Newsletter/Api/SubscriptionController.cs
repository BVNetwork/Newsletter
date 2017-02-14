using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Library;
using EPiServer.Logging;
using Newtonsoft.Json.Linq;

namespace BVNetwork.EPiSendMail.Api
{
    public class SubscriptionController : ApiBaseController
    {
        private static readonly ILogger _log = LogManager.GetLogger();

        /// <summary>
        /// Adds an email address to the one public recipient list, or the one named 
        /// "Default" if there are more than one.
        /// </summary>
        /// <remarks>
        /// We do not care if the address already exists, due to
        /// security conserns, we won't tell if it do exists, just add it
        /// </remarks>
        /// <param name="email">The email address to add. It will be validated.</param>
        /// <returns>A success or fail message.</returns>
        [HttpPost]
        public JObject Subscribe(string email)
        {
            SubscriptionApi api = new SubscriptionApi();
            SubscriptionResult result = api.Subscribe(email);

            if (result == SubscriptionResult.Success)
                return GetSubscriptionResult(true);
            else
            {
                _log.Information("Unable to subcribe '{0}' to a public list. Result: {1}", email, result);
                return GetSubscriptionResult(false);
            }
        }

        /// <summary>
        /// Adds an email address to the specificed recipient list
        /// </summary>
        /// <remarks>
        /// We do not care if the address already exists, due to
        /// security conserns, we won't tell if it do exists, just add it
        /// </remarks>
        /// <param name="email">The email address to add. It will be validated.</param>
        /// <param name="recipientList">The ID of the recipient list</param>
        /// <returns>A success or fail message.</returns>
        [HttpPost]
        public JObject Subscribe(string email, int recipientList)
        {

            SubscriptionApi api = new SubscriptionApi();
            SubscriptionResult result = api.Subscribe(email, recipientList);

            if (result == SubscriptionResult.Success)
                return GetSubscriptionResult(true);
            else
            {
                _log.Information("Unable to subcribe '{0}' to a list {1}. Result: {2}", email, recipientList, result);
                return GetSubscriptionResult(false);
            }
        }

        [HttpPost]
        public JObject Unsubscribe(string email)
        {
            SubscriptionApi api = new SubscriptionApi();
            SubscriptionResult result = api.Unsubscribe(email);

            if (result == SubscriptionResult.Success)
                return GetSubscriptionResult(true);
            else
            {
                _log.Information("Unable to unsubcribe '{0}' from a public list. Result: {1}", email, result);
                return GetSubscriptionResult(false);
            }
        }

        [HttpPost]
        public JObject Unsubscribe(string email, int recipientList)
        {
            SubscriptionApi api = new SubscriptionApi();
            SubscriptionResult result = api.Unsubscribe(email, recipientList);

            if (result == SubscriptionResult.Success)
                return GetSubscriptionResult(true);
            else
            {
                _log.Information("Unable to unsubcribe '{0}' from list '{1}'. Result: {2}", email, recipientList, result);
                return GetSubscriptionResult(false);
            }
        }

        [Authorize(Roles = "NewsletterEditors, CmsAdmins")]
        [HttpGet]
        public List<string> ValidateRecipientList(int recipientList, int blockList)
        {
            _log.Debug("Validating recipient list {0}, adding to block list {1}", recipientList, blockList);

            RecipientList list = RecipientList.Load(recipientList);
            RecipientList blocked = RecipientList.Load(blockList);

            MailSenderMailgun sender = new MailSenderMailgun();

            List<string> result = null;
            try
            {
                result = sender.ValidateRecipientList(list, blocked);
            }
            catch (Exception e)
            {
                _log.Error("Error validating recipent list", e);
                throw new HttpResponseException(
                    Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Cannot validate emails: " + e.Message));
            }

            if(result == null)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occured during parsing, please inspect the logs."));
            }
            return result;
        }

        protected JObject GetSubscriptionResult(bool result)
        {
            return JObject.FromObject(new
            {
                subscriptionResult = result
            });
        }

    }
}
