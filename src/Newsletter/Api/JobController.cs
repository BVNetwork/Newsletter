using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BVNetwork.EPiSendMail.Api.Models;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Plugin.WorkItemProviders;
using BVNetwork.EPiSendMail.ScheduledTasks;
using EPiServer.DataAbstraction;
using JobStatus = BVNetwork.EPiSendMail.Api.Models.JobStatus;

namespace BVNetwork.EPiSendMail.Api
{

    [Authorize(Roles = "NewsletterEditors, CmsAdmins")]
    public class JobController : ApiController
    {
        [HttpGet]
        public List<Job> List()
        {
            Jobs jobs = DataAccess.Jobs.ListAll();
            return jobs.Items;
        }

        [HttpGet]
        public Job Get(int jobId)
        {
            Job job = DataAccess.Job.Load(jobId);
            if (job == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return job;
        }


        [HttpGet]
        public Models.JobStatus GetJobStatus(int jobId)
        {
            Job job = DataAccess.Job.Load(jobId);
            if (job == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            ScheduledJob jobDefinition = SendNewsLetterTask.GetJobDefinition();

            Models.JobStatus status = new JobStatus();
            status.Job = job.Id;
            status.Name = job.Name;
            status.EmailsSent = job.GetWorkItemCountForStatus(JobWorkStatus.Complete);
            status.EmailsFailed = job.GetWorkItemCountForStatus(JobWorkStatus.Failed);
            status.EmailsNotSent = job.GetWorkItemCountForStatus(JobWorkStatus.NotStarted);
            status.EmailsInQueue = job.GetWorkItemCountForStatus(JobWorkStatus.Sending);

            if(NewsletterConfigurationSection.Instance.IgnoreServiceStatus == false)
            {
                status.SchedulerIsOnline = ScheduledJob.IsServiceOnline;
            }
            else
            {
                // In a load balanced environment, you might not want
                // to report that the scheduler is not enabled, as users
                // could get confused (another server will be responsible)
                status.SchedulerIsOnline = true;
            }
            

            status.Status = job.Status.ToString();
            if (jobDefinition != null)
            {
                status.ScheduledTaskIsEnabled = jobDefinition.IsEnabled;
                status.ScheduledTaskNextRun = jobDefinition.NextExecution;
                status.ScheduledTaskIsRunning = jobDefinition.IsRunning;
            }
            else
            {
                status.ScheduledTaskIsEnabled = false;
                status.ScheduledTaskNextRun = DateTime.MinValue;
                status.ScheduledTaskIsRunning = false;
            }
            status.BatchSize = NewsLetterConfiguration.SendBatchSize;
            status.TimeStamp = DateTime.Now;

            return status;
        }

        [HttpPost]
        public bool Send(int jobId)
        {
            Job job = DataAccess.Job.Load(jobId);
            if (job == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            if (job.Status != DataAccess.JobStatus.Sending)
            {
                job.Status = DataAccess.JobStatus.Sending;
                job.Save();
            }

            return true;

        }

        [HttpDelete]
        public bool Delete(int jobId)
        {
            Job job = DataAccess.Job.Load(jobId);
            if (job == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            job.Delete();

            return true;

        }
    }
}
