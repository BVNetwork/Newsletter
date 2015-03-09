using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BVNetwork.EPiSendMail.Api;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;
using EPiServer;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Shell.WebForms;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class NewsLetters : PluginWebFormsBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Load jobs
            Jobs jobs = Jobs.ListAll();
            FilterClosedJobs(jobs);
            BindJobData(jobs);

            if(jobs.Items.Count == 0)
            {
                pnlJobList.Visible = false;
                pnlNoJobs.Visible = true;
            }

            //if (Request.QueryString["pid"] != null)
            //{
            //    var repository = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IContentRepository>();
            //    string contentId = Request.QueryString["pid"];
            //    var contentLink = new ContentReference(contentId);
            //    PageData page = repository.Get<PageData>(contentLink);
            //    string newsletterEdit = Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/Plugin/Jobs/JobEdit.aspx?pid=" + page.PageLink.ID.ToString();
            //    iframeNewsletter.Attributes["src"] = newsletterEdit;
            //}

        }

        protected string GetNewsletterLink(Job job)
        {
            if (job.PageId == 0)
                return "javascript:alert('This job has no corresponding page attached. Please contact your web administrator.')";
            return job.PageId.ToString();
        }

        private void FilterClosedJobs(Jobs jobs)
        {
            // Filter away closed
            for (int i = jobs.Items.Count - 1; i >= 0; i--)
            {
                Job job = jobs.Items[i];

                if (job.Status == DataAccess.JobStatus.Editing
                    || job.Status == DataAccess.JobStatus.Sending)
                {
                    // Keep it
                }
                else
                {
                    // Remove it
                    jobs.Items.Remove(job);
                }
            }
        }

        private void BindJobData(Jobs jobs)
        {
            rptNewsLettersInProgress.DataSource = jobs.Items.OrderByDescending(j => j.Id);
            rptNewsLettersInProgress.DataBind();
        }



        protected void lnkShowAll_Click(object sender, EventArgs e)
        {
            // Load jobs
            Jobs jobs = Jobs.ListAll();
            BindJobData(jobs);
        }

        public string GetRecipientListTypeString(object listType)
        {
            return GetRecipientListTypeString((RecipientListType)listType);
        }

        public string GetRecipientListTypeString(RecipientListType listType)
        {
            return NewsLetterUtil.GetEnumLanguageName(listType);
        }

        protected string GetNewsLetterImageForStatus(object status)
        {
            return GetNewsLetterImageForStatus((DataAccess.JobStatus)status);
        }
        protected string GetNewsLetterImageForStatus(DataAccess.JobStatus status)
        {
            return Configuration.NewsLetterConfiguration.GetModuleBaseDir() + "/content/images/" + status.ToString() + ".gif";
        }

        protected string GetNewsLetterColorForStatus(DataAccess.JobStatus status)
        {
            switch (status)
            {
                case DataAccess.JobStatus.Closed:
                    return "gray";
                case DataAccess.JobStatus.Editing:
                    return "black";
                case DataAccess.JobStatus.Sending:
                    return "green";
                default:
                    return "";
            }
        }

        protected string GetPageEditUrl(Job job)
        {
            if(job.PageId != 0)
            {
                return PageEditing.GetEditUrl(new ContentReference(job.PageId));
            }

            return string.Empty;
        }

        protected int GetNumberOfWorkItems(Job job)
        {
            int total = job.GetWorkItemCountForStatus(JobWorkStatus.Complete) +
                        job.GetWorkItemCountForStatus(JobWorkStatus.Failed) +
                        job.GetWorkItemCountForStatus(JobWorkStatus.NotStarted) +
                        job.GetWorkItemCountForStatus(JobWorkStatus.Sending);
            return total;
        }


    }
}