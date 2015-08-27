using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using EPiServer.Shell.Navigation;
using EPiServer.Security;
using EPiServer;

namespace BVNetwork.EPiSendMail.Plugin
{
    [MenuProvider]
    public class NewsLetterMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();
            var newsletterRoles = new List<string>()
            {
                "NewsletterAdmins","NewsletterEditors","CmsAdmins"
            };
            SectionMenuItem sectionMenuItem = new SectionMenuItem("Newsletter", "/global/newsletter");
            sectionMenuItem.IsAvailable = x => newsletterRoles.Any(y => x.HttpContext.User.IsInRole(y));
            menuItems.Add(sectionMenuItem);
            
            
            // During installation, we'll show another menu
            int version = DatabaseVersion.GetInstalledDatabaseVersion();
            if (version == DatabaseVersion.NotInstalled)
            {
                // Link to database installer page
                UrlMenuItem urlMenuItem = new UrlMenuItem("Newsletter Installation", "/global/newsletter/install",
                    NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/admin/newsletterinstall.aspx");
                urlMenuItem.IsAvailable = ((RequestContext request) => true);
                urlMenuItem.SortIndex = 100;
                menuItems.Add(urlMenuItem);
            }
            else
            {
                UrlMenuItem urlMenuItem = new UrlMenuItem("Newsletters", "/global/newsletter/newsletters", NewsLetterConfiguration.GetModuleBaseDir("/plugin/newsletters.aspx"));
                urlMenuItem.IsAvailable = ((RequestContext request) => true);
                urlMenuItem.SortIndex = 100;
                menuItems.Add(urlMenuItem);

                urlMenuItem = new UrlMenuItem("Lists", "/global/newsletter/lists", NewsLetterConfiguration.GetModuleBaseDir("/plugin/lists.aspx"));
                urlMenuItem.IsAvailable = ((RequestContext request) => true);
                urlMenuItem.SortIndex = 200;
                menuItems.Add(urlMenuItem);
                
                //TODO: Add this menu item, when admin settings are ready to be moved from appsettings to dds
                //UrlMenuItem urlAdminMenuItem = new UrlMenuItem("Admin", "/global/newsletter/admin",
                //    NewsLetterConfiguration.GetModuleBaseDir() + "/plugin/Admin/NewsLetterAdmin.aspx");
                //urlAdminMenuItem.IsAvailable = ((RequestContext request) => true);
                //urlAdminMenuItem.SortIndex = 200;
            }

            return menuItems.ToArray();
        }
    }




}