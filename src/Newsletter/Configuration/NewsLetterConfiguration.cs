using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Library;
using BVNetwork.EPiSendMail.Plugin;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace BVNetwork.EPiSendMail.Configuration
{
    /// <summary>
    /// Static properties and methods with different configuration values
    /// </summary>
    public static class NewsLetterConfiguration
    {
        private const int DEFAULT_EDITPANEL_TAB = 4;
        private const bool DEFAULT_MODULE_IS_ENABLED = true;
        private const string KEY_EXT_LOG_FILE = "EPsSendMailExtendedLogFile";
        private const string KEY_DISABLE_EXCEPTIONS_IN_RENDER = "EPfSendMailDisableRenderExceptions";
        private const string MODULE_BASE_DIR = "/modules/epicode.newsletter";
        private static bool _isEnabled = DEFAULT_MODULE_IS_ENABLED;
        private static bool _isEnabledRead = false;

        /// <summary>
        /// Gets the path to an extended log file. Should be root relative
        /// and the physical path to a file on the file system. The value is
        /// read from the EPsSendMailExtendedLogFile appSetting in web.config.
        /// </summary>
        /// <remarks>
        /// Only enable logging when testing, as the log file will grow rapidly
        /// during mail sending.
        /// 
        /// Returns null if no log file has been configured in the web.config file.
        /// No logging will be done if the config value is null.
        /// 
        /// The web site process account must have write access to the file.
        /// </remarks>
        /// <example>
        /// The following example setting from web.config will log to a file called
        /// newsletterExtended.log in the root of your web site.
        /// &lt;appSettings&gt;
        ///     &lt;add key="EPsSendMailExtendedLogFile" value="newsletterExtended.log" /&gt;
        /// &lt;/appSettings&gt;
        /// </example>
        /// <value>The path to the extended log file.</value>
        public static string ExtendedLogFile
        {
            get 
            {
                return GetAppSettingsConfigValueEx<string>(KEY_EXT_LOG_FILE, null); 
            }
        }

        /// <summary>
        /// Gets a value indicating whether to disable exceptions in mail rendering. If your
        /// markup has errors, the renderer might throw exceptions. Set this to true to swallow
        /// the exceptions.
        /// </summary>
        /// <remarks>
        /// Please note that this can lead to unwanted side effects as errors in markup or rendering
        /// go unnoticed. Make sure you test your newsletter thoroughly before setting this.
        /// 
        /// Use this along with the ExtendedLogFile setting to have exceptions with more details be
        /// logged to a file.
        /// </remarks>
        /// <example>
        /// This example shows how to disable exceptions in mail rendering:
        /// &lt;appSettings&gt;
        ///     &lt;add key="EPfSendMailDisableRenderExceptions" value="True" /&gt;
        /// &lt;/appSettings&gt;
        /// </example>
        /// <value>
        /// 	<c>true</c> if exceptions should be disabled in mail rendering; otherwise, <c>false</c>.
        /// </value>
        public static bool DisableExceptionsInMailRendering
        {
            get {
                return GetAppSettingsConfigValueEx<bool>(KEY_DISABLE_EXCEPTIONS_IN_RENDER, false); 
            }
        }

        public static string MailSenderTypename {
            get
            {
                string typeName = GetAppSettingsConfigValueEx<string>("Newsletter.SenderType", null);
                if (typeName != null)
                    typeName = typeName.Trim();

                return typeName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Newsletter module is enabled.
        /// </summary>
        /// <remarks>
        /// Checks the appSettings section in web.config for the EPfEnableSendMailPlugIn
        /// settings. If false, all the functionality in the module is disabled, and the
        /// plug-ins will not be visible.
        /// </remarks>
        /// <value><c>true</c> if module is enabled; otherwise, <c>false</c>. 
        /// If no settings has been defined in the config file, default is true.</value>
        public static bool ModuleIsEnabled
        {
            get 
            {
                if (_isEnabledRead == false)
                {
                    _isEnabled = GetAppSettingsConfigValueEx<bool>("EPfEnableSendMailPlugIn",
                                                          DEFAULT_MODULE_IS_ENABLED);
                    _isEnabledRead = true;
                }
                return _isEnabled; 
            }
        }

        public static string GetEditPanelLink(Job job)
        {
            if (job.PageId == 0)
                return "javascript:alert('This job has no corresponding page attached. Please contact your web administrator.')";

            UrlResolver urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            PageData page = repository.Get<PageData>(new ContentReference(job.PageId));
            if (page == null)
            {
                return "javascript:alert('The page for this job has been deleted. Please contact your web administrator.')";
            }

            var iLocalizable = page as ILocalizable;
            string language = "en"; // last default resort
            if (iLocalizable.Language != null)
            {
                language = iLocalizable.Language.Name;
            } 
            var editUrl = UrlResolver.Current.GetUrl(page.ContentLink, 
                                                     language, 
                                                     new VirtualPathArguments() { ContextMode = ContextMode.Edit });

            return editUrl;
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
        public static int SendBatchSize
        {
            get
            {
                // This could be dependent on mail sender
                MailSenderBase mailSender = new EPiMailEngine().GetMailSender();
                if(mailSender != null)
                {
                    return mailSender.GetBatchSize();
                }
                // Fall back to configuration
                return NewsLetterUtil.GetSendBatchSize();
            }
        }

        public static string GetModuleBaseDir()
        {
            return MODULE_BASE_DIR;
        }

        public static string GetModuleBaseDir(string relativePath)
        {
            if(relativePath.StartsWith("/"))
                return MODULE_BASE_DIR + relativePath;
            return MODULE_BASE_DIR + "/" + relativePath;
        }


        /// <summary>
        /// Gets the fully qualified base URL for the newsletter.
        /// </summary>
        /// <remarks>
        /// Will not end with the a slash.
        /// </remarks>
        /// <returns></returns>
        public static string GetFullyQualifiedBaseUrl()
        {
            string url = SiteDefinition.Current.SiteUrl.ToString();
            url = url.TrimEnd(new[] {'/'});
            return url;
        }

        /// <summary>
        /// Gets the value of an appSettings key as an int. If not defined
        /// or not parsable, the defaultValue parameter will be returned.
        /// </summary>
        /// <param name="key">The key in the appSettings section whose value to return.</param>
        /// <param name="defaultValue">The default value to returned if the setting is null or not parsable.</param>
        /// <returns>The appSettings value if parsable, defaultValue if not</returns>
        public static int GetAppSettingsConfigValueInt(string key, int defaultValue)
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

        /// <summary>
        /// Gets the value of an appSettings key as an bool. If not defined
        /// or not parsable, the defaultValue parameter will be returned.
        /// </summary>
        /// <param name="key">The key in the appSettings section whose value to return.</param>
        /// <param name="defaultValue">The default value to returned if the setting is null or not parsable.</param>
        /// <returns>The appSettings value if parsable, defaultValue if not</returns>
        public static bool GetAppSettingsConfigValueBool(string key, bool defaultValue)
        {
            string stringValue = WebConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(stringValue))
                return defaultValue;

            bool retValue;
            bool parsed = false;
            parsed = bool.TryParse(stringValue, out retValue);
            if (parsed)
                return retValue;

            // Could not parse, return default value
            return defaultValue;
        }

        public static T GetAppSettingsConfigValueEx<T>(string key, T defaultValue)
        {
            object val = GetAppSettingsConfigValue(key);
            if (string.IsNullOrEmpty(val.ToString()))
                return defaultValue;
            else
            {
                return (T)val;
            }
        }

        /// <summary>
        /// Gets a config value from the appSettings section, compatible
        /// with the EPiServer 4 version that converted the value based
        /// on the 3rd characted in the key,
        /// </summary>
        /// <remarks>
        /// Prefixes: EPf is bool, EPn is int, EPs is string, any other is string. 
        /// If no value exists for the key, string.Empty is returned.
        /// </remarks>
        /// <param name="key">The key of the setting to retrieve.</param>
        /// <returns>The value from the appSettings section in web.config, converted to the correct type</returns>
        public static object GetAppSettingsConfigValue(string key)
        {
            if (key.Length < 4)
            {
                throw new ApplicationException("Too short key: \"" + key + "\", must be at least four characters.");
            }

            string settingsValue = WebConfigurationManager.AppSettings[key];
            if (settingsValue != null)
            {
                if (((key[0] == 'E') || (key[0] == 'e'))
                    &&
                    ((key[1] == 'P') || (key[1] == 'p')))
                {
                    switch (key[2])
                    {
                        // Bool
                        case 'f':
                            bool val;
                            // If parseable, return it
                            if (bool.TryParse(settingsValue, out val))
                                return val;
                            // Cant parse it - return false
                            return false;
                        // Integer
                        case 'n':
                            int intVal;
                            if (int.TryParse(settingsValue, out intVal))
                                return intVal;
                            // Cant parse - default value is 0
                            return 0;
                    }
                }
            }
            // Return if not null, empty if null
            return settingsValue ?? string.Empty;
        }

        
        /// <summary>
        /// Default in the newslettermodul there are three recipient lists. If a project need specific recipientlists that is not
        /// needed to the core project, these usercontrols can be added as a name value collection in web.config.
        /// 
        /// <newsletterRecipientProviders>
        ///    <add key="EPiServerCategoryProvider" value="/sendmail/plugin/workitemproviders/EPiServerCategoryProvider.ascx" />
        /// 
        /// Remember to update the lang file with a description.
        /// 
        /// <?xml version="1.0" encoding="utf-8" ?>
        /// <languages>
        ///   <language name="Norwegian" id="no">
        ///     <newsletterRecipientProvider>
        ///       <EPiServerCategoryProvider>Recipient list for EPiServer categories.</EPiServerCategoryProvider>
        /// 
        /// 
        /// </summary>
        /// <returns>A NameValueCollection of extensions and types if found in web.config, null if not.</returns>
        public static NameValueCollection GetRecipientListProviders()
        {
            NameValueCollection config = ConfigurationManager.GetSection("newsletterRecipientProviders") as NameValueCollection;
            return config;
        }


        /// <summary>
        /// Gets the recipient list provider descriptors.
        /// </summary>
        /// <returns>A list of all the configured recipient list providers</returns>
        public static List<RecipientListProviderDescriptor> GetRecipientListProviderDescriptors()
        {
            List<RecipientListProviderDescriptor> providerList;
            if(NewsletterConfigurationSection.Instance.RecipientListProviders.Count > 0)
            {
                providerList = new List<RecipientListProviderDescriptor>();
                foreach (RecipientListProvider provider in NewsletterConfigurationSection.Instance.RecipientListProviders)
                {
                    providerList.Add(new RecipientListProviderDescriptor(
                        provider.Name, provider.DisplayName, provider.Url));
                }
            }
            else
            {
                // Nothing configured, 
                providerList = GetDefaultRecipientListProviders();
            }
            return providerList;
        }

        private static List<RecipientListProviderDescriptor> GetDefaultRecipientListProviders()
        {

            List<RecipientListProviderDescriptor> providerList = new List<RecipientListProviderDescriptor>();
            providerList.Add(new RecipientListProviderDescriptor(
                "RecipientList",
                "Add from one of your Recipient Lists",
                BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() +
                "/plugin/itemproviders/recipientprovider.ascx"
                ));

            providerList.Add(new RecipientListProviderDescriptor(
                "TextImport",
                "Import from text",
                BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() +
                "/plugin/itemproviders/TextImportProvider.ascx"
                ));

            providerList.Add(new RecipientListProviderDescriptor(
                "EPiServerGroup",
                "Add email addresses from an EPiServer Group",
                BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() +
                "/plugin/itemproviders/EPiServerGroupProvider.ascx"
                ));

            providerList.Add(new RecipientListProviderDescriptor(
                "CommerceUsers",
                "Add email addresses from Commerce Contacts",
                BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() +
                "/plugin/itemproviders/CommerceUsersProvider.ascx"
                ));

            return providerList;
        }


    }
}
