using System.Web.Mvc;
using EPiServer.Web.Mvc;
using BVNetwork.EPiSendMail.Api;
using $rootnamespace$.Models.ViewModels;
using $rootnamespace$.Models.Pages;

namespace $rootnamespace$.Controllers
{
    public class NewsletterUnsubscribePageController : PageController<NewsletterUnsubscribePage>
    {
        public ActionResult Unsubscribe(NewsletterUnsubscribePage currentPage, string email)
        {
            PageViewModel<NewsletterUnsubscribePage> model = new PageViewModel<NewsletterUnsubscribePage>(currentPage);

            // Attempt to unsubscribe
            SubscriptionApi api = new SubscriptionApi();
            SubscriptionResult result;
            if(currentPage.BlockListId == 0 )
            {
                result = api.Unsubscribe(email);
            }
            else
            {
                result = api.UnsubscribeUsingBlocklist(email, currentPage.BlockListId);
            }

            ViewData.Add("SubscriptionResult", result);
            ViewData.Add("email", email);

            return View("index", model);
        }

        public ActionResult Index(NewsletterUnsubscribePage currentPage)
        {
            PageViewModel<NewsletterUnsubscribePage> model = new PageViewModel<NewsletterUnsubscribePage>(currentPage);
            return View(model);
        }

    }
}