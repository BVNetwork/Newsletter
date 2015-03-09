using System;
using System.Drawing;
using BVNetwork.EPiSendMail.Api;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Web.Mvc.Html;
using EPiServer.Shell.WebForms;

namespace BVNetwork.EPiSendMail.Plugin.Admin

{
    public partial class NewsLetterInstall : PluginWebFormsBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verify database is correct
            int version = DatabaseVersion.GetInstalledDatabaseVersion();
            if(version == DatabaseVersion.NotInstalled)
            {
                // Show error and upgrade options
                return;
            }

        }

    }
}