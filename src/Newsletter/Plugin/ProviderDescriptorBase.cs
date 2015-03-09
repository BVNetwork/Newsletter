using System.IO;
using System.Web;

namespace BVNetwork.EPiSendMail.Plugin
{

    /// <summary>
    /// Base class for recipient and work item providers.
    /// </summary>
    public class ProviderDescriptorBase
    {
        public ProviderDescriptorBase(string name, string linkText, string userCtrlUrl)
        {
            ProviderName = name;
            Text = linkText;
            UserControlUrl = userCtrlUrl;

        }

        public string Text { get; set; }

        public string ProviderName { get; set; }

        public string UserControlUrl { get; set; }

        public bool ProviderControlExists
        {
            get
            {
                if (string.IsNullOrEmpty(UserControlUrl) == true)
                    return false;

                if (HttpContext.Current == null)
                    return false;

                string physicalPath = HttpContext.Current.Server.MapPath(UserControlUrl);
                if (File.Exists(physicalPath))
                    return true;

                return false;
            }
        }
    }
}