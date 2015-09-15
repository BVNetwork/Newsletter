using System;
using System.Collections.Generic;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace BVNetwork.EPiSendMail.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class InitializeNewsletterEvents : IInitializableModule
    {

        public void Initialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {
            IContentEvents contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.CreatedContent += ContentEventsOnCreatedContent;
            contentEvents.DeletedContent += ContentEventsOnDeletedContent;
            contentEvents.SavedContent += ContentEventsOnSavedContent;
        }

        public void Preload(string[] parameters)
        {

        }

        public void Uninitialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {

        }

        private void ContentEventsOnCreatedContent(object sender, ContentEventArgs contentEventArgs)
        {
            PageData page = contentEventArgs.Content as PageData;
            if (page == null)
            {
                return;
            }

            INewsletterBase correctBase = page as INewsletterBase;
            if (correctBase != null)
            {
                Job job = Job.LoadByPageId(page.PageLink.ID);
                if (job == null)
                {
                    string name = page.PageName;
                    string desc = string.Empty;
                    // Create and Save
                    Job newJob = new Job(page.PageLink.ID, name, desc);
                    newJob.Save();
                }

            }
        }

        private void ContentEventsOnDeletedContent(object sender, DeleteContentEventArgs deleteContentEventArgs)
        {
            PageData page = deleteContentEventArgs.Content as PageData;
            if (page == null)
            {
                return;
            }

            INewsletterBase correctBase = page as INewsletterBase;
            if (correctBase != null)
            {
                Job job = Job.LoadByPageId(page.PageLink.ID);
                if (job != null)
                {
                    // Page has been deleted, now delete job and all it's work items
                    job.Delete();
                }
            }
        }

        private void ContentEventsOnSavedContent(object sender, ContentEventArgs contentEventArgs)
        {
            PageData page = contentEventArgs.Content as PageData;
            if (page == null)
            {
                return;
            }

            INewsletterBase correctBase = page as INewsletterBase;
            if (correctBase != null)
            {
                Job job = Job.LoadByPageId(page.PageLink.ID);
                if (job != null)
                {
                    // Sync page name and job name
                    job.Name = page.PageName;
                    job.Save();
                }
            }

        }


    }
}