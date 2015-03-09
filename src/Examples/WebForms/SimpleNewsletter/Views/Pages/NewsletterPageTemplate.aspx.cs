using System;
using BVNetwork.EPiSendMail.bvn.SendMail.Models.Pages;
using EPiServer;

namespace BVNetwork.EPiSendMail.Views.Pages
{
    public partial class NewsletterPageTemplate : TemplatePage<NewsletterPage>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

           
            DataBind();
        }
    }
}