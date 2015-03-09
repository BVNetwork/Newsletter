using System.Web;
using BVNetwork.EPiSendMail.Contracts;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace BVNetwork.EPiSendMail.UIExtensions
{
    [ServiceConfiguration(typeof(EPiServer.Shell.ViewConfiguration))]
    public class NewsLetterDetailsView : ViewConfiguration<NewsletterBase>
    {
        public NewsLetterDetailsView()
        {
            Key = "NewsletterDetailsView";
            Name = "Newsletter Details";
            Description = "Newsletter Details View";
            ControllerType = "epi-cms/widget/IFrameController";
            ViewType = VirtualPathUtility.ToAbsolute(Configuration.NewsLetterConfiguration.GetModuleBaseDir("/Plugin/Jobs/JobEdit.aspx"));
            IconClass = "newsletterView";
        }
    }
}
