using System.Web.Mvc;
using $rootnamespace$.Models.Pages;
using EPiServer.Web.Mvc;

namespace $rootnamespace$.Controllers
{
    public class NewsletterWithSidebarPageController : PageController<NewsletterWithSidebarPage>
    {
        public ActionResult Index(NewsletterWithSidebarPage currentPage)
        {
            return View(currentPage);
        }

    }
}