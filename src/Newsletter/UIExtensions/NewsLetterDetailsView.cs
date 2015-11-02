using System.Web;
using BVNetwork.EPiSendMail.Configuration;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace BVNetwork.EPiSendMail.UIExtensions
{
    [ServiceConfiguration(typeof(ViewConfiguration))]
    public class NewsLetterDetailsView : ViewConfiguration<INewsletterBase>
    {
        public NewsLetterDetailsView()
        {
            Key = "NewsletterDetailsView";
            LanguagePath = "/bvnetwork/sendmail/view";
            ControllerType = "epi-cms/widget/IFrameController";
            ViewType = VirtualPathUtility.ToAbsolute(NewsLetterConfiguration.GetModuleBaseDir("/Plugin/Jobs/JobEdit.aspx"));
            IconClass = "newsletterView";
        }
    }
}
