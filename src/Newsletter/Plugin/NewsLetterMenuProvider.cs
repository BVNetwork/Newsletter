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

                urlMenuItem = new UrlMenuItem(string.Empty, "/global/newsletter/newsletters/jobeditstandalone", NewsLetterConfiguration.GetModuleBaseDir("/plugin/jobs/jobeditstandalone.aspx"));
                urlMenuItem.IsAvailable = ((RequestContext request) => false);
                urlMenuItem.SortIndex = 110;
                menuItems.Add(urlMenuItem);

                urlMenuItem = new UrlMenuItem(string.Empty, "/global/newsletter/newsletters/workitemsedit", NewsLetterConfiguration.GetModuleBaseDir("/plugin/jobs/workitemsedit.aspx"));
                urlMenuItem.IsAvailable = ((RequestContext request) => false);
                urlMenuItem.SortIndex = 120;
                menuItems.Add(urlMenuItem);


                urlMenuItem = new UrlMenuItem("Lists", "/global/newsletter/lists", NewsLetterConfiguration.GetModuleBaseDir("/plugin/lists.aspx"));
                urlMenuItem.IsAvailable = ((RequestContext request) => true);
                urlMenuItem.SortIndex = 200;
                menuItems.Add(urlMenuItem);

                urlMenuItem = new UrlMenuItem(string.Empty, "/global/newsletter/lists/listedit", NewsLetterConfiguration.GetModuleBaseDir("/plugin/recipientlists/listedit.aspx"));
                urlMenuItem.IsAvailable = ((RequestContext request) => false);
                urlMenuItem.SortIndex = 210;
                menuItems.Add(urlMenuItem);

                urlMenuItem = new UrlMenuItem(string.Empty, "/global/newsletter/lists/newrecipientlist", NewsLetterConfiguration.GetModuleBaseDir("/plugin/recipientlists/newrecipientlist.aspx"));
                urlMenuItem.IsAvailable = ((RequestContext request) => false);
                urlMenuItem.SortIndex = 220;
                menuItems.Add(urlMenuItem);

                urlMenuItem = new UrlMenuItem(string.Empty, "/global/newsletter/lists/recipientitemsedit", NewsLetterConfiguration.GetModuleBaseDir("/plugin/recipientlists/recipientitemsedit.aspx"));
                urlMenuItem.IsAvailable = ((RequestContext request) => false);
                urlMenuItem.SortIndex = 230;
                menuItems.Add(urlMenuItem);

                urlMenuItem = new UrlMenuItem(string.Empty, "/global/newsletter/lists/recipientlistdeleted", NewsLetterConfiguration.GetModuleBaseDir("/plugin/recipientlists/recipientlistdeleted.aspx"));
                urlMenuItem.IsAvailable = ((RequestContext request) => false);
                urlMenuItem.SortIndex = 230;
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