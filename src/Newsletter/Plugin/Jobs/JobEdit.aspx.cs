using System;
using EPiServer;
using EPiServer.Core;

namespace BVNetwork.EPiSendMail.Plugin
{

	public partial class NewsLetterJobEdit : JobUiPageBase
	{
	    protected void Page_Load(object sender, System.EventArgs e)
		{
            if (Request.QueryString["pid"] != null)
            {
               var repository = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IContentRepository>();
                string contentId = Request.QueryString["pid"];
                var contentLink = new ContentReference(contentId);
                CurrentContent = repository.Get<PageData>(contentLink);
            }
		}

	}
}
