using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using $rootnamespace$.Models.Pages;
using EPiServer.Web.Mvc;

namespace $rootnamespace$.Controllers
{
    public class NewsletterPageController : PageController<NewsletterPage>
    {
        public ActionResult Index(NewsletterPage currentPage)
        {
            return View(currentPage);
        }

    }
}
