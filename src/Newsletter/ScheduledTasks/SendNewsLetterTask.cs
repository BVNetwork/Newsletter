using System;
using System.Diagnostics;
using System.Threading;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Library;
using EPiServer.BaseLibrary.Scheduling;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Logging;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;

namespace BVNetwork.EPiSendMail.ScheduledTasks
{
    /// <summary>
    /// An EPiServer scheduled task plug-in for sending newsletters.
    /// </summary>
    [ScheduledPlugIn(
        DisplayName="Send Newsletter  Task", 
        Description="Sends newsletters that has been scheduled for sending.")]
    public class SendNewsLetterTask : EPiServer.Scheduler.ScheduledJobBase
    {
        // Logger
        private static readonly ILogger _log = LogManager.GetLogger();
        // static lock
        private static object TaskLock = null;

        //private bool _isStopped = false;

        // Static constructor
        static SendNewsLetterTask()
        {
        	TaskLock = new object();
        }

        public SendNewsLetterTask()
        {
            this.IsStoppable = false;
        }

        public override void Stop()
        {
            base.Stop();
            // _isStopped = true;
        }

        public override string Execute()
        {
            DebugWrite("SendNewsLetterTask::Execute Start");
            this.OnStatusChanged("Checking for newsletters to send.");
            string retVal = null;
            try
            {
                retVal = ExecuteImpl();
            }
            catch (Exception ex)
            {
                retVal = ex.ToString();
            }

            retVal = "Server: " + Environment.MachineName + "<br/>\n" + retVal;

            DebugWrite("SendNewsLetterTask::Execute End");
            if (string.IsNullOrEmpty(retVal) == false)
                return retVal;
            else
                return "Nothing to return. This is unexpected";
        }

        protected string ExecuteImpl()
        {
            DebugWrite("Executing SendNewsLetterTask");

            string logMsg;
            string sendLog;
            Stopwatch tmr = Stopwatch.StartNew();


            // Check if we're running
            if (!Monitor.TryEnter(TaskLock))
            {
                OnStatusChanged("Job is already running, exiting.");
                string msg = "Job is already running.";
                DebugWrite(msg);
                return msg;
            }

            try
            {
                // 1. Get items to send
                Job job = GetJobWithWorkItemsToProcess();
                if (job != null)
                {
                    // 2. Send
                    OnStatusChanged("Sending: " + job.Name);
                    sendLog = SendNewsletter(job);
                }
                else
                {
                    sendLog = "No jobs with status Sending was found.";
                    OnStatusChanged(sendLog);
                    DebugWrite(sendLog);
                }

            }
            catch (Exception ex)
            {
                // Log it:
                if (_log.IsErrorEnabled())
                    _log.Error("SendNewsLetterTask failed (on " + Environment.MachineName + ")", ex);

                // Handle exception, or rethrow to signal that the job failed   
                throw;
            }
            finally 
            {
                // Make sure we release the lock, or no one else 
                // will be able to send afterwards
                Monitor.Exit(TaskLock);
            }

            tmr.Stop();
            logMsg = "Execution of SendNewsLetterTask has finished in {0}ms <br />\n{1}";
            logMsg = string.Format(logMsg, tmr.ElapsedMilliseconds.ToString(), sendLog);

            DebugWrite(logMsg);

            return logMsg;
        }

        /// <summary>
        /// Sends the newsletter.
        /// </summary>
        /// <returns></returns>
        private string SendNewsletter(Job job)
        {
            JobWorkItems workItemsForProcessing = job.GetWorkItems();

            if (workItemsForProcessing.Items.Count == 0)
            {
                DebugWrite("Work Items collection is empty. Nothing to send.");
                return "Work Items collection is empty. Nothing to send.";
            }

            // Get information about what we're about to send
            EPiMailEngine mailEngine = new EPiMailEngine();
            MailSenderBase msbase = mailEngine.GetMailSender();
            MailInformation mailInfo = msbase.GetMailMetaData(new PageReference(job.PageId));

            // const int sleepTestInterval = 0;
            DebugWrite(string.Format("Start sending newsletter. Job name: '{0}', Subject: '{1}', From: '{2}'",
                   job.Name, mailInfo.Subject, mailInfo.From));

            // Send the message

            // For testing, it can be nice to control the time it takes to send
            // each batch
//#if DEBUG
//            if (sleepTestInterval > 0)
//                System.Threading.Thread.Sleep(sleepTestInterval);
//#endif
            // Send with status
            
            SendMailLog log;
            log = mailEngine.SendNewsletter(mailInfo.Subject,
                                  mailInfo.From, /* Who we send from */
                                  mailInfo.PageLink, /* The page to send */
                                  workItemsForProcessing, /* Who we send to */
                                  false /* Not Test Mode */);
//#if DEBUG
//            if (sleepTestInterval > 0)
//                System.Threading.Thread.Sleep(sleepTestInterval);
//#endif
            return log.GetClearTextLogString(true /* Use br instead of \n */);
        }


        /// <summary>
        /// Gets a job with the status Sending, which have work items
        /// to process.
        /// </summary>
        /// <returns></returns>
        private Job GetJobWithWorkItemsToProcess()
        {
            // Get all jobs in sending state
            DataAccess.Jobs jobsForSending = DataAccess.Jobs.ListAll(JobStatus.Sending);

            foreach (Job job in jobsForSending)
            {
                // Load work items to process
                job.LoadWorkItemsForProcessing();
                
                if (job.GetWorkItems().Items.Count > 0)
                {
                    // We found a job with work items
                    // return job and work items
                    return job;
                }
                else
                {
                    // This job has no work items left, we'll
                    // set the status to complete
                    job.Status = JobStatus.Closed;
                    job.Save();
                }
            }

            // If we get here, there were nothing to do
            return null;
        }

        private void DebugWrite(string text)
        {
            if (_log.IsDebugEnabled())
                _log.Debug(text);
        }

        public static ScheduledJob GetJobDefinition()
        {
            ScheduledJobRepository repository =
                ServiceLocator.Current.GetInstance<ScheduledJobRepository>();
            ScheduledJob job = repository.Get("Execute", typeof (SendNewsLetterTask).FullName,
                typeof (SendNewsLetterTask).Assembly.GetName().Name);
            return job;
        }

    }
}
