using System;
using System.Data;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;

namespace BVNetwork.EPiSendMail.DataAccess
{
    public class JobWorkItem
    {
        private int _jobId;
        private string _emailAddress;
        private JobWorkStatus _status;
        private string _info;

        private static WorkItemData GetWorker()
        {
            return EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<WorkItemData>();
        }

        public JobWorkItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the JobWorkItem class.
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="emailAddress"></param>
        /// <param name="status"></param>
        /// <param name="info"></param>
        public JobWorkItem(int jobId, string emailAddress, JobWorkStatus status, string info)
        {
            _jobId = jobId;
            _status = status;
            // Let property setter format it
            Info = info;
            EmailAddress = emailAddress;
        }

        /// <summary>
        /// Initializes a new instance of the JobWorkItem class.
        /// </summary>
        /// <param name="emailAddress"></param>
        public JobWorkItem(string emailAddress)
        {
            _jobId = 0;
            _status = JobWorkStatus.NotStarted;
            // Let property setter format it
            Info = null;
            EmailAddress = emailAddress;
        }

        /// <summary>
        /// Initializes a new instance of the JobWorkItem class.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="status"></param>
        public JobWorkItem(string emailAddress, JobWorkStatus status)
        {
            _jobId = 0;
            _status = status;
            // Let property setter format it
            Info = null;
            EmailAddress = emailAddress;
        }




        /// <summary>
        /// Initializes a new instance of the JobWorkItem class from a Datatable.Row
        /// </summary>
        internal JobWorkItem(DataRow row)
        {
            _jobId = (int)(row["fkJobId"]);
            _emailAddress = row["EmailAddress"] != null ? row["EmailAddress"].ToString() : null;
            _status = (JobWorkStatus)row["Status"];
            _info = row["Info"] != null ? row["Info"].ToString() : null;
        }

        public string Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
                if (_info != null)
                    _info = _info.Trim();
            }
        }
        public JobWorkStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public string EmailAddress
        {
            get
            {
                return _emailAddress;
            }
            set
            {
                _emailAddress = NewsLetterUtil.CleanEmailAddress(value);
            }
        }

        public int JobId
        {
            get
            {
                return _jobId;
            }
            set
            {
                _jobId = value;
            }
        }

        /// <summary>
        /// Saves this work item.
        /// </summary>
        public virtual void Save()
        {
            if (_jobId == 0)
                throw new NullReferenceException("This work item has no job association, and cannot be saved.");
            if (_emailAddress == null)
                throw new NullReferenceException("Cannot save work item with empty email address.");

            // Save item
            WorkItemData dataUtil = GetWorker();
            dataUtil.WorkItemEdit(_jobId, _emailAddress, _status, _info);

        }

        /// <summary>
        /// Loads the specified work item based on a job id and an email address.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>A JobWorkItem if job and email found, null it not</returns>
        public static JobWorkItem Load(int jobId, string emailAddress)
        {
            WorkItemData dataUtil = GetWorker() ;
            // Get it, making sure we pass a washed address
            DataRow workRow = dataUtil.WorkItemGet(jobId, NewsLetterUtil.CleanEmailAddress(emailAddress));
            if (workRow != null)
                return new JobWorkItem(workRow);
            else
                return null;
        }

        /// <summary>
        /// Deletes this work item.
        /// </summary>
        public void Delete()
        {
            if (_jobId == 0)
                throw new NullReferenceException("This work item has no job association, and cannot be deleted.");
            if (_emailAddress == null)
                throw new NullReferenceException("Cannot delete work item with empty email address.");

            WorkItemData dataUtil = GetWorker();
            dataUtil.WorkItemDelete(_jobId, _emailAddress);
        }

        /// <summary>
        /// Deletes the specified work item.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <param name="emailAddress">The email address.</param>
        public static void Delete(int jobId, string emailAddress)
        {
            WorkItemData dataUtil = GetWorker();
            dataUtil.WorkItemDelete(jobId, emailAddress);
        }

        public string GetLogData()
        {
            return string.Format("Job {0}, Email: {1}, Status: {2}", JobId.ToString(), EmailAddress, Status.ToString());
        }
    }
}
