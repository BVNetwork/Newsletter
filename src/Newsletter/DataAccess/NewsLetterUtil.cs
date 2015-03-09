using System;
using System.Web.Configuration;

namespace BVNetwork.EPiSendMail.DataAccess
{
    public static class NewsLetterUtil
    {
        /// <summary>
        /// Checks the email address, changes it to lower case and trims it.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>If not null, a lower case trimmed string. If given null, returns null.</returns>
        public static string CleanEmailAddress(string emailAddress)
        {
            if (emailAddress != null)
            {
                emailAddress = emailAddress.Trim();
                emailAddress = emailAddress.ToLowerInvariant();
            }
            return emailAddress;
        }

        public static string GetEnumLanguageName(Enum enumvar)
        {
            return EPiServer.Framework.Localization.LocalizationService.Current.GetString("/bvnetwork/sendmail/enum/" 
                + enumvar.GetType().Name.ToLower() 
                    + "/" + enumvar.ToString().ToLower());
        }

        /// <summary>
        /// Gets the batch size we're using to pull work items from the 
        /// database. This setting is read from the EPnSendMailSendBatchSize
        /// setting in web.config
        /// </summary>
        /// <remarks>
        /// If no EPnSendMailSendBatchSize setting is found in the
        /// web.config file, the default value of 50 is used
        /// </remarks>
        /// <returns>The number of Work Items to ask for when sending newsletters</returns>
        public static int GetSendBatchSize()
        {
            const int defaultBatchSize = 50;
            return GetConfigValueInt("EPnSendMailSendBatchSize", defaultBatchSize);
        }

        /// <summary>
        /// Gets the value of an appSettings key as an int. If not defined
        /// or not parsable, the defaultValue parameter will be returned.
        /// </summary>
        /// <param name="key">The key in the appSettings section whose value to return.</param>
        /// <param name="defaultValue">The default value to returned if the setting is null or not parsable.</param>
        /// <returns>The appSettings value if parsable, defaultValue if not</returns>
        private static int GetConfigValueInt(string key, int defaultValue)
        {
            string stringValue = WebConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(stringValue))
                return defaultValue;

            int retValue;
            bool parsed = false;
            parsed = int.TryParse(stringValue, out retValue);
            if (parsed)
                return retValue;
            
            // Could not parse, return default value
            return defaultValue;
        }
    }
}
