using System;
using System.Net;
using System.Text;
using System.Web;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.Contracts;
using BVNetwork.EPiSendMail.DataAccess;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace BVNetwork.EPiSendMail.Library
{
	/// <summary>
	/// This class does the actual mail sending, using the standard .NET Framework
	/// </summary>
	public abstract class MailSenderBase
	{
        private static readonly ILogger _log = LogManager.GetLogger();
        protected Injected<IContentRepository> ContentRepository { get; set; }

		/// <summary>
		/// Creates the mail info object with only meta information like subject
		/// sender etc. Does not add mail content to the object.
		/// </summary>
		/// <param name="pagelink">The EPiServer page to use to extract meta information.</param>
		/// <returns></returns>
		public virtual MailInformation GetMailMetaData(PageReference pagelink)
		{
			return GetMailMetaData(GetPageWithChecks(pagelink));
		}

		/// <summary>
		/// Creates the mail info object with only meta information like subject
		/// sender etc. Does not add mail content to the object.
		/// </summary>
		public virtual MailInformation GetMailMetaData(PageData mailPage)
		{
			// Get content from page
			MailInformation mailInfo = new MailInformation();

			mailInfo.PageLink = mailPage.PageLink;
			mailInfo.Subject = GetMailSubject(mailPage);
			if (mailPage["BaseUrl"] != null)
				mailInfo.BaseUrl = mailPage["BaseUrl"].ToString();
			else
				mailInfo.BaseUrl = GetSiteUrl(mailPage);
			
			// The pagename can be used for other things later on
			mailInfo.PageName = mailInfo.PageName;
			
			// Sender address
			mailInfo.From = GetMailSender(mailPage);

            // Campaign data
		    mailInfo.Utm.Campaign = GetCampaign(mailPage);

			return mailInfo;
		}

		/// <summary>
		/// Gets the mail subject.
		/// </summary>
		/// <remarks>
		/// 1. Use MailSubject property
		/// 2. Use PageName
		/// 2. Use Newsletter.DefaultMailSubject from appSetting
		/// 3. Use "Newsletter" as default
		/// </remarks>
		public virtual string GetMailSubject(PageData page)
		{
			string subject = null;
			if (page != null)
			{
				if (page["MailSubject"] != null)
					subject = page["MailSubject"].ToString();

				if (subject == null)
					subject = page.PageName;
			}

			if (subject == null)
				subject = NewsLetterConfiguration.GetAppSettingsConfigValueEx<string>("Newsletter.DefaultMailSubject", "Newsletter");

			return subject;
		}

        /// <summary>
        /// Looks for a UtmCampaign property on the page and uses that for Utm tracking
        /// </summary>
        /// <returns>The campaign name if it exists</returns>
        public virtual string GetCampaign(PageData page)
		{
			string campaign = null;
			if (page != null)
			{
				if (page["UtmCampaign"] != null)
					campaign = page["UtmCampaign"].ToString();
			}

			return campaign;
		}

		/// <summary>
		/// Gets the sender email address.
		/// </summary>
		/// <remarks>
		/// 1. Use MailSender property
		/// 2. Use Newsletter.DefaultFromAddress from appSettings
		/// 3. Construct noreply@SiteUrl
		/// </remarks>
		/// <param name="page">The page to extract information from.</param>
		/// <returns></returns>
		public virtual string GetMailSender(PageData page)
		{
			string fromAddress = null;
			if (page["MailSender"] != null)
				fromAddress = page["MailSender"].ToString();
			else
				fromAddress = NewsLetterConfiguration.GetAppSettingsConfigValueEx<string>("Newsletter.DefaultFromAddress", "noreply@" + SiteDefinition.Current.SiteUrl.Host);
			return fromAddress;
		}

		/// <summary>
		/// Creates a mail info object based on information on a page.
		/// </summary>
		/// <param name="pagelink">Pagelink.</param>
		/// <returns></returns>
		public virtual MailInformation GetMailInformation(PageReference pagelink)
		{
			return GetMailInformation(GetPageWithChecks(pagelink));
		}

		/// <summary>
		/// Creates a mail info object based on information on a page.
		/// </summary>
		/// <returns></returns>
		public virtual MailInformation GetMailInformation(PageData mailPage)
		{
			// Get Meta Information
			MailInformation mailInfo = GetMailMetaData(mailPage);

			// Get the two body versions
			string mailBody = GetPageHtml(mailPage);

			string hostUrl = GetSiteUrl(mailPage);

			mailInfo.BodyHtml = RewriteUrls(mailBody, hostUrl);

			mailInfo.BodyText = QuickCleanMailText(GetPageText(mailPage));

			// Let page add custom properties
			IPopulateCustomProperties customPropertiesProvider = mailPage as IPopulateCustomProperties;
			if (customPropertiesProvider != null)
			{
				// Let page populate the custom properties collection
				customPropertiesProvider.AddCustomProperties(mailInfo.CustomProperties);
			}

			return mailInfo;

		}

		/// <summary>
		/// Make all urls fully qualified, we only care about href and src attributes here.
		/// </summary>
		/// <remarks>
		/// If you need more advanced rewriting, override this method in the
		/// sender implementation
		/// </remarks>
		/// <param name="mailBody"></param>
		/// <returns></returns>
		public virtual string RewriteUrls(string mailBody, string hostUrl)
		{
			string safeHost = hostUrl.TrimEnd('/');

			mailBody = mailBody.Replace("src=\"//", "src=\"/");
			mailBody = mailBody.Replace("src=\"/", "src=\"" + safeHost + "/");
			mailBody = mailBody.Replace("href=\"/", "href=\"" + safeHost + "/");
			return mailBody;
		}

		/// <summary>
		/// Send mail to mailreceivers using System.Web.Mail 
		/// </summary>
		/// <param name="mailInfo">A MailInformation object with the content to send</param>
		/// <param name="recepients">receivers</param>
		/// <param name="testMode">No mails are sent, generating report for user</param>
		/// <returns>A report of the send process</returns>
		public abstract SendMailLog SendEmail(MailInformation mailInfo, JobWorkItems recepients, bool testMode);
		
		/// <summary>
		/// Get mail content in HTML format.
		/// </summary>
		/// <param name="pageRef">Pagelink to mailpage</param>
		/// <returns>HTML</returns>
		public virtual string GetPageHtml(PageReference pageRef)
		{
			if (_log.IsDebugEnabled())
				_log.Debug("Beginning to generate Page Html for {0}.", pageRef.ToString());

			PageData pageData = GetPage(pageRef);
			return GetPageHtml(pageData);
		}

		public virtual string GetPageHtml(PageData pageData)
		{
			string url;
			string html;
			string hostUrl;
			bool hasHttpContext = HttpContext.Current != null;

			if (_log.IsDebugEnabled())
				_log.Debug("Beginning to generate Page Html for {0}. Has HttpContext: {1}", pageData.PageLink.ToString(), hasHttpContext);

			// Standard way of getting a url
			url = UrlResolver.Current.GetUrl(pageData.PageLink, ((ILocalizable)pageData).Language.Name);
			_log.Debug("UrlResolver says link to newsletter is: {0}", url);

			hostUrl = GetSiteUrl(pageData);

			if (_log.IsDebugEnabled())
				_log.Debug("Link to newsletter page: {0} (Hosturl: {1})", url, hostUrl);

			if (url.StartsWith("/"))
			{
				// We have a relative url
				url = hostUrl.TrimEnd('/') + url;
				_log.Debug("Constructed full url from relative: {0} ", url);
			}
			else
			{
				// Make sure we're using the correct host
				UriBuilder correctHost = new UriBuilder(hostUrl);
				UriBuilder uriBuilder = new UriBuilder(url);
				uriBuilder.Host = correctHost.Host;
				uriBuilder.Scheme = correctHost.Scheme;
				uriBuilder.Port = correctHost.Port;
				if (uriBuilder.Port == 80)
				{
					// Little known hack to remove port from ToString()
					uriBuilder.Port = -1;
				}
				url = uriBuilder.ToString();
				_log.Debug("Constructed full url from two hosts: {0} ", url);
			}

			html = DownloadHtml(url);

			_log.Debug("Downloaded {0} bytes of html from {1}", html.Length, url);

			return html;
		}

		protected string DownloadHtml(string url)
		{
			string html;
			WebClient wr = new WebClient();
			wr.Credentials = CredentialCache.DefaultCredentials;
			byte[] htmlData = wr.DownloadData(url);

			html = Encoding.UTF8.GetString(htmlData);
			htmlData = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, htmlData);
			html = Encoding.Unicode.GetString(htmlData);
			return html;
		}

		protected PageData GetPage(PageReference pageRef)
		{
			IContentRepository contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

			ContentReference contentLink = pageRef.ToReferenceWithoutVersion();
			PageData pageData = contentRepository.Get<PageData>(contentLink);
			if (pageData == null)
			{
				throw new NullReferenceException("Cannot Load Page: " + pageRef.ToString());
			}
			
			return pageData;
		}

		protected string QuickCleanMailText(string bodytext)
		{			
			bodytext  = bodytext.Replace("<br>", " \n");
			bodytext  = bodytext.Replace("<BR>", " \n");
			bodytext  = bodytext.Replace("<p>", "");
			bodytext  = bodytext.Replace("</p>", " \n");
			bodytext  = bodytext.Replace("<P>", "");
			bodytext  = bodytext.Replace("</P>", " \n");
			bodytext  = bodytext.Replace("<div>", "");
			bodytext  = bodytext.Replace("</div>", "");
			bodytext  = bodytext.Replace("<DIV>", "");
			bodytext  = bodytext.Replace("</DIV>", "");

			return bodytext;
		}
		

		/// <summary>
		/// Get text content in HTML format.
		/// </summary>
		/// <param name="pageRef">Reference to mail page</param>
		/// <returns></returns>
		public virtual string GetPageText(PageReference pageRef)
		{			
			PageData mailpage =   DataFactory.Instance.GetPage(pageRef);
			return GetPageText(mailpage);
		}

		/// <summary>
		/// If this is an enterprise solution, we need to be sure we have the correct site url. 
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		public virtual string GetSiteUrl(PageData page)
		{
			string hostUrl;
			
            // Config has highest priority
		    hostUrl = Configuration.NewsLetterConfiguration.GetAppSettingsConfigValueEx<string>("Newsletter.BaseUrl", null);
		    if (string.IsNullOrEmpty(hostUrl))
		    {
		        if (page["BaseUrl"] != null)
		        {
		            // The host url could be different than what we get from the system
		            // if you have a dedicated edit server for an example.
		            hostUrl = page["BaseUrl"].ToString();
		        }
		        else
		        {
#if CMS9
                    // Always look up based on page
                    SiteDefinitionResolver repo = ServiceLocator.Current.GetInstance<SiteDefinitionResolver>();
		            SiteDefinition siteDefinition = repo.GetDefinitionForContent(page.ContentLink, fallbackToWildcardMapped: true, fallbackToEmpty: false);
#else
                    ISiteDefinitionResolver repo = ServiceLocator.Current.GetInstance<ISiteDefinitionResolver>();
                    SiteDefinition siteDefinition = repo.GetByContent(page.ContentLink, fallbackToWildcard: true, fallbackToEmpty: false);
#endif
                    if (siteDefinition == null || siteDefinition.SiteUrl == null)
		            {
		                // Still haven't found it, can't go on
		                throw new ApplicationException("Cannot find a SiteDefinition with a valid SiteUrl for page: " +
		                                               page.ContentLink.ToString());
		            }

		            if (_log.IsDebugEnabled())
		            {
		                _log.Debug("Looked up Site Definition: {0}, Url: {1}", siteDefinition.Name,
		                    siteDefinition.SiteUrl);
		                foreach (HostDefinition host in siteDefinition.Hosts)
		                {
		                    _log.Debug("Available Site Definition Host: {0} (language: {1})", host.Name, host.Language);
		                }
		            }

		            hostUrl = siteDefinition.SiteUrl.ToString();
		        }
		    }

		    return hostUrl;
		}
		
		public virtual string GetPageText(PageData page)
		{
			string mailBody = string.Empty;
			
			if (page != null && page["MainBodyText"] != null)
				mailBody = page["MainBodyText"].ToString(); 		

			return mailBody;
		}

		protected PageData GetPageWithChecks(PageReference pagelink)
		{
			if (pagelink == PageReference.EmptyReference)
				throw new ArgumentException("PageReference to mailpage is empty, no content to send.", "pagelink");

			// Load page
			PageData mailPage = null;
			if (ContentRepository.Service.TryGet<PageData>(pagelink, out mailPage) == false)
				throw new NullReferenceException("Cannot load newsletter page (" + pagelink.ToString() + ")");

			return mailPage;
		}


		/// <summary>
		/// Returns the number of emails to send for a given newsletter and a given task
		/// </summary>
		/// <returns></returns>
		public virtual int GetBatchSize()
		{
			// IMPORTANT! Do not call the NewsletterConfiguration.SendBatchSize property
			// as it calls this one.
			return NewsLetterUtil.GetSendBatchSize();
		}


	    protected bool IsInDebugMode()
	    {
            // If no http context, we assume debug is not on
	        if (HttpContext.Current != null)
	            return HttpContext.Current.IsDebuggingEnabled;

	        // Alternative way to detect debug mode
            //CompilationSection compilationSection = System.Configuration.ConfigurationManager.GetSection(@"system.web/compilation") as CompilationSection;
            //if (compilationSection != null) 
            //    return compilationSection.Debug;
	        return false;
	    }
	}
}
