using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVNetwork.EPiSendMail.Configuration;
using EPiServer.Core;
using EPiServer.Shell.WebForms;

namespace BVNetwork.EPiSendMail.Plugin
{
    public class PluginWebFormsBase : WebFormsBase
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (Page.User.IsInRole("CmsAdmins") == false
                && Page.User.IsInRole("NewsletterEditors") == false
                && Page.User.IsInRole("NewsletterAdmins"))
            {
                throw new AccessDeniedException();
            }

            MasterPageFile = EPiServer.UriSupport.ResolveUrlFromUIBySettings("MasterPages/EPiServerUI.Master");

            if (Page.ClientScript.IsClientScriptIncludeRegistered("bootstrap") == false)
            {
                Page.ClientScript.RegisterClientScriptInclude(Page.GetType(), "bootstrap", NewsLetterConfiguration.GetModuleBaseDir("/content/js/bootstrap.js"));
            }

            if (Page.ClientScript.IsClientScriptIncludeRegistered("jquery") == false)
            {
                Page.ClientScript.RegisterClientScriptInclude(Page.GetType(), "jquery", NewsLetterConfiguration.GetModuleBaseDir("/content/js/jquery-2.0.3.min.js"));

            }

            if (Page.ClientScript.IsClientScriptIncludeRegistered("jsRender") == false)
            {
                Page.ClientScript.RegisterClientScriptInclude(Page.GetType(), "jsRender", NewsLetterConfiguration.GetModuleBaseDir("/content/js/jsrender.min.js"));
                
            }


        }
    }
}
