using System;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using BVNetwork.EPiSendMail.DataAccess;
using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace BVNetwork.EPiSendMail.Plugin
{
    public class JobUiUserControlBase : EPiServer.UserControlBase, IJobUi
    {
        private Job _job;
        private int _jobId = -1;
        private StatusMessage _messageControl;

        public override PageData CurrentPage
        {
            get {
                if (CurrentContent != null)
                {
                    if (CurrentContent.ContentLink != ContentReference.StartPage)
                        return CurrentContent as PageData;
                    string pageId = Request.QueryString["pId"];
                    if (!string.IsNullOrEmpty(pageId))
                    {
                        return Locate.ContentRepository().Get<PageData>(new ContentReference(int.Parse(pageId)));
                    }
                }
                return null;

            }
            set { }
        }

        public StatusMessage MessageControl
        {
            get
            {
                if (_messageControl == null)
                    SetupData();
                return _messageControl;
            }
            set
            {
                _messageControl = value;
            }
        }

        public Job NewsletterJob
        {
            get
            {
                // First try to find root
                if (_job == null)
                    SetupData();

                // Load it by page ID
                if (_job == null)
                {
                    if (JobId > 0)
                        _job = Job.LoadByPageId(JobId);
                }

                // still null? See if we have a page loaded (not the start page)
                if (_job == null && CurrentContent != null &&
                    CurrentContent.ContentLink.CompareToIgnoreWorkID(ContentReference.StartPage) == false)
                {
                    // We're on a page, look up the newsletter based on the page id
                    // Will return null if not found
                    _job = Job.LoadByPageId(CurrentContent.ContentLink.ID);
                }

                // Still no job - it might have been deletes, so we create it again
                // to not break the UI. It will be empty though.
                if(_job == null)
                {
                    if(CurrentPage == null)
                        throw new ApplicationException("Unable to create a new Job for a newsletter page");

                    // Create and Save
                    _job = new Job(CurrentPage.PageLink.ID, CurrentPage.PageName, "");
                    _job.Save();
                }

                return _job;
            }
        }

        /// <summary>
        /// This is - for some reason - not the job id, but the page id.
        /// Cannot change it without breaking code.
        /// </summary>
        public int JobId
        {
            get
            {
                if (_jobId == -1)
                    SetupData();

                if (_jobId == -1)
                    _jobId = GetJobIdFromQuery();

                return _jobId;
            }
        }

        protected int GetJobIdFromQuery()
        {
            string id = Request.QueryString["pId"];

            if (string.IsNullOrEmpty(id))
                id = "0";
            else
                return int.Parse(id);
            return 0;
        }


        /// <summary>
        /// Shows an error message on the page.
        /// </summary>
        /// <param name="message">Error message.</param>
        public void ShowError(string message)
        {
            MessageControl.ErrorMessage = message;
        }

        /// <summary>
        /// Shows an info message on the page.
        /// </summary>
        /// <param name="infoMessage">The info message.</param>
        public void ShowInfo(string message)
        {
            MessageControl.InfoMessage = message;
        }



        private void SetupData()
        {
            // Avoid searching if already found
            if (_messageControl != null)
                return;

            // Search for it
            Control parentCtrl = Parent;
            while (parentCtrl != null)
            {
                JobEditControl rootCtrl = parentCtrl as JobEditControl;
                if (rootCtrl == null)
                    parentCtrl = parentCtrl.Parent;
                else
                {
                    _messageControl = rootCtrl.MessageControl;
                    _job = rootCtrl.NewsletterJob;
                    _jobId = rootCtrl.JobId;
                    break;
                }
            }

            // Still haven't found it?
            // Try the page
            if (_job == null)
            {
                JobUiPageBase pageUi = this.Page as JobUiPageBase;
                if (pageUi != null)
                {
                    // Found it
                    _messageControl = pageUi.MessageControl;
                    _job = pageUi.NewsletterJob;
                    _jobId = pageUi.JobId;
                }
            }

        }


        public virtual T Get<T>(ContentReference contentLink) where T : IContentData
        {
            return this.Get<T>(contentLink, LanguageSelector.AutoDetect(true));
        }

        public virtual T Get<T>(ContentReference contentLink, ILanguageSelector selector) where T : IContentData
        {
            T obj = Locate.ContentRepository().Get<T>(contentLink, selector);
            if ((object)obj == null)
                return default(T);
            AccessLevel access = contentLink.CompareToIgnoreWorkID(this.CurrentContentLink) ? AccessLevel.Read : AccessLevel.Read;
            ISecurable securable = (object)obj as ISecurable;
            if (securable != null && !securable.GetSecurityDescriptor().HasAccess(PrincipalInfo.CurrentPrincipal, access))
            {
                if (PrincipalInfo.CurrentPrincipal.Identity.IsAuthenticated)
                    throw new AccessDeniedException();
                DefaultAccessDeniedHandler.AccessDenied((object)this);
            }
            return obj;
        }

        private IContent _content;
        private ContentReference _currentLink;
        public virtual IContent CurrentContent
        {
            get
            {
                return this._content ?? (this._content = this.Get<IContent>(this.CurrentContentLink));
            }
            set
            {
                this._content = value;
            }
        }


        public virtual ContentReference CurrentContentLink
        {
            get
            {
                if (ContentReference.IsNullOrEmpty(this._currentLink))
                {
                    this._currentLink = ServiceLocator.Current.GetInstance<ContentRouteHelper>().ContentLink;
                    if (ContentReference.IsNullOrEmpty(this._currentLink))
                    {
                        RouteData routeData = ServiceLocator.Current.GetInstance<ClassicLinkRoute>().GetRouteData(HttpContextExtensions.ContextBaseOrNull(HttpContext.Current));
                        if (routeData != null)
                            this._currentLink = routeData.DataTokens[RoutingConstants.NodeKey] as ContentReference;
                    }
                    if (ContentReference.IsNullOrEmpty(this._currentLink))
                    {
                        this._currentLink = (ContentReference)ContentReference.StartPage;
                        if (ContentReference.IsNullOrEmpty(this._currentLink))
                            this._currentLink = (ContentReference)ContentReference.RootPage;
                    }
                }
                return this._currentLink;
            }
            set
            {
                this._currentLink = value;
            }
        }

    }
}
