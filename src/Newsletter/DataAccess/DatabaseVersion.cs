using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;
using EPiServer.Logging;

namespace BVNetwork.EPiSendMail.DataAccess
{
    public static class DatabaseVersion
    {
        private static readonly ILogger _log = LogManager.GetLogger();
        public static int NotInstalled = -1;
        public static int CurrentDatabaseVersion = 301;

        public static int GetInstalledDatabaseVersion()
        {
            SystemData systemData = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<SystemData>();
            int version = NotInstalled;
            try
            {
                version = systemData.GetNewsletterDatabaseVersion();
            }
            catch (Exception e)
            {
                // Nothing we can do here, we'll report -1
                _log.Warning("Unable to determine Database version for Newsletter module", e);
            }
            return version;
        }
    }
}
