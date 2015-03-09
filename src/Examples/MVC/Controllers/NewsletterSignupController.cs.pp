using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using $rootnamespace$.Models.Blocks;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Mvc;

namespace $rootnamespace$.Controllers
{
    public class NewsletterSignupController : BlockController<NewsletterSignupBlock>
    {
        public override ActionResult Index(NewsletterSignupBlock currentBlock)
        {
            return PartialView(currentBlock);
        }
    }
}
