using System;
using System.Linq;
using EPiServer.Data.Dynamic;

namespace BVNetwork.EPiSendMail.Plugin.Admin
{
    public class AdminSettingsRepository
    {

        private static AdminSettingsRepository _instance;

        public AdminSettingsRepository()
        {
        }

        public static AdminSettingsRepository Instance
        {
            get { return _instance ?? (_instance = new AdminSettingsRepository()); }
        }

        private static DynamicDataStore Store
        {
            get { return typeof(AdminSettingsModel).GetStore(); }
        }

        public bool SaveSettings(AdminSettingsModel settings)
        {
            try
            {
                var currentSettings = LoadSettings();
                Store.Save(settings, currentSettings.Id);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public AdminSettingsModel LoadSettings()
        {
            var currentSettings = Store.Items<AdminSettingsModel>().FirstOrDefault();
            if (currentSettings == null)
            {
                currentSettings = new AdminSettingsModel();
                Store.Save(currentSettings);
            }
            return currentSettings;
        }
    }
}