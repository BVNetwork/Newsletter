using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.Models;

namespace BVNetwork.EPiSendMail.Controllers
{
    public class NewsletterController : Controller
    {
        public ActionResult Index()
        {
            return new RedirectResult(NewsLetterConfiguration.GetModuleBaseDir("plugin/newsletters.aspx"));

            //NewsletterListModel model = new NewsletterListModel();
            //model.Title = "Newsletters";
            //return View(NewsLetterConfiguration.GetModuleBaseDir() + "/Views/Newsletter/Index.cshtml", model);

            //ContentResult result = new ContentResult();
            //result.Content = "Home";

            //return result;
        }
    }
}
