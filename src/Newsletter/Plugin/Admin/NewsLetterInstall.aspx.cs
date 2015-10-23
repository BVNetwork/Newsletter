using System;
using BVNetwork.EPiSendMail.DataAccess;

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