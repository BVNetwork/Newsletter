using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;

namespace BVNetwork.EPiSendMail.DataAccess
{
    public class Job : IEmailImporter
    {
        private int _id = 0;
        private JobWorkItems _workItems = null;
        Hashtable _statusCounts = null;

        public static int StatusCountEmpty = 0;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Job()
        {
            Status = JobStatus.Editing;
            PageId = 0;
        }

        private static T GetWorker<T>()
        {
            return EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<T>();
        }

        /// <summary>
        /// Initializes a new instance of the Job class.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="pageId"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        internal Job(int id, JobStatus status, int pageId, string name, string description)
        {
            _id = id;
            Status = status;
            PageId = pageId;
            Name = name;
            Description = description;
        }

        internal Job(DataRow dataRow) : this(dataRow, null)
        {
        }

        internal Job(DataRow dataRow, DataRow[] statusRows)
        {
            _id = (int)dataRow["pkJobId"];
            Name = dataRow["Name"] != null ? dataRow["Name"].ToString() : null;
            Description = dataRow["Description"] != null ? dataRow["Description"].ToString() : null;
            PageId = dataRow["PageId"] != null ? (int)dataRow["PageId"] : 0;
            Status = dataRow["Status"] != null ? (JobStatus)dataRow["Status"] : JobStatus.Closed;

            if (statusRows != null && statusRows.Length > 0)
            {
                BuildInternalStatusCountTable(statusRows);
            }
        }

        /// <summary>
        /// Initializes a new instance of the Job class.
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public Job(int pageId, string name, string description)
        {
            Status = JobStatus.Editing;
            PageId = pageId;
            Name = name;
            Description = description;
        }
        
        
        /// <summary>
        /// The unique id of this newsletter job
        /// </summary>
        public int Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Name of the newsletter, identifying the job
        /// </summary>
        /// <value>A name to describe the job in a list of jobs</value>
        public string Name { get; set; }

        /// <summary>
        /// A short description of this newsletter
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The ID of the EPiServer page that is to be rendered as a newsletter
        /// </summary>
        /// <value>An EPiServer Page Id to a page that this job will use for constructing the email</value>
        public int PageId { get; set; }

        /// <summary>
        /// The status of the job. Query this to see if the job has been started, or closed.
        /// </summary>
        /// <value>The Status of the job. Indicates where in the life cycle the job is.</value>
        public JobStatus Status { get; set; }

        /// <summary>
        /// A list of work items that needs to be handled
        /// </summary>
        /// <remarks>
        /// When accessed for the first time, the list will be
        /// filled with all the work items for this job. 
        /// This can be time consuming if there are many items
        /// in the database.
        /// </remarks>
        public JobWorkItems GetWorkItems()
        {
            if (_workItems == null)
            {
                // Get worker items for this job
                if (_id <= 0)
                    throw new ApplicationException(
                        "Cannot retrieve work items for a job that has no id. Save the job first to get the id.");

                // Get items
                _workItems = JobWorkItems.ListAll(_id);
            }
            return _workItems;
        }


        private void BuildInternalStatusCountTable(DataRow[] statusRows)
        {
            // Set up a zero valued hashtable
            Array jobStatusValues = Enum.GetValues(typeof(JobWorkStatus));
            _statusCounts = new Hashtable(jobStatusValues.Length);
            for (int i = 0; i < jobStatusValues.Length; i++)
            {
                _statusCounts.Add(Enum.GetName(typeof(JobWorkStatus), jobStatusValues.GetValue(i)), StatusCountEmpty);
            }

            // We've got statuses, parse these
            foreach (DataRow statusRow in statusRows)
            {
                // We might have rows with no status values (no work items in the database)
                if (statusRow["Status"] != DBNull.Value)
                    _statusCounts[Enum.GetName(typeof(JobWorkStatus), statusRow["Status"])] = (int)statusRow["NumOfRows"];
            }
        }

        /// <summary>
        /// Gets the count of work items for a given status code.
        /// </summary>
        /// <param name="status">The status.</param>
        public int GetWorkItemCountForStatus(JobWorkStatus status)
        {
            //JobWorkStatus.
            if (_statusCounts == null)
            {
                // Get from database
                WorkItemData dataUtil = GetWorker<WorkItemData>();
                int count = dataUtil.WorkItemGetCountByStatus(_id, status);
                return count;
            }
            else
            {
                // Get count value from hash table
                return (int)_statusCounts[status.ToString()];
            }

        }

        protected void ResetStatusCounters()
        {
            _statusCounts = null;
        }


        /// <summary>
        /// Gets work items for processing.
        /// </summary>
        /// <remarks>
        /// All work items returned will be set to Sending status.
        /// </remarks>
        /// <param name="batchSize">The number of work items to return</param>
        /// <returns>A WorkItems collections with work items ready for processing.</returns>
        public JobWorkItems GetWorkItemsForProcessing()
        {
            int batchSize = NewsLetterConfiguration.SendBatchSize;
            return GetWorkItemsForProcessing(batchSize);
        }

        /// <summary>
        /// Gets work items for processing as a separate collection. This
        /// collection is not wired back to this Job object.
        /// </summary>
        /// <remarks>
        /// All work items returned will be set to Sending status.
        /// </remarks>
        /// <returns>A WorkItems collections with work items ready for processing.</returns>
        public JobWorkItems GetWorkItemsForProcessing(int batchSize)
        {
            WorkItemData dataUtil = GetWorker < WorkItemData>();
            DataTable workTable = dataUtil.WorkItemGetBatchForProcessing(_id, JobWorkStatus.NotStarted, JobWorkStatus.Sending, batchSize);

            JobWorkItems workItems = new JobWorkItems();
            JobWorkItems.FillFromDataTable(workTable, workItems);

            return workItems;
        }

        public void LoadWorkItemsForProcessing()
        {
            int batchSize = NewsLetterUtil.GetSendBatchSize();
            LoadWorkItemsForProcessing(batchSize);
        }

        /// <summary>
        /// Loads the work items for processing into the
        /// WorkItems property. Will clear the existing
        /// collection, and replace it with the new items
        /// ready for processing. If you want WorkItems as
        /// a separate collection, use the
        /// GetWorkItemsForProcessing method.
        /// </summary>
        /// <param name="batchSize">Number of work items to load</param>
        public void LoadWorkItemsForProcessing(int batchSize)
        {
            JobWorkItems workItems = GetWorkItemsForProcessing(batchSize);
            if (_workItems != null)
            {
                _workItems.Items.Clear();
                _workItems = null;
            }

            _workItems = workItems;

            // Counters are no longer valid - we just changed them
            ResetStatusCounters();
        }
        
        /// <summary>
        /// Deletes this job and all worker items related to the job
        /// </summary>
        public void Delete()
        {
            if (_id <= 0)
                throw new ArgumentException("Cannot delete job without id.");

            JobData dataUtil = GetWorker<JobData>();
            
            dataUtil.JobDelete(_id);

            // Remove any workitems from memory
            _workItems = null;
        }

        /// <summary>
        /// Deletes all work items.
        /// </summary>
        public void DeleteAllWorkItems()
        {
            if (_id <= 0)
                throw new ArgumentException("Cannot remove work items for a job without id. Please save the job first.");

            WorkItemData dataUtil = GetWorker<WorkItemData>();
            dataUtil.WorkItemDeleteAllForJob(_id);
            
            // Remove any workitems from memory
            _workItems = null;

            // Counters are no longer valid
            ResetStatusCounters();
        }

        /// <summary>
        /// Removes work items from this job based on the contents of a recipient list.
        /// </summary>
        /// <remarks>
        /// Deletes all work items with email addresses that also exists in a recipient list
        /// </remarks>
        /// <param name="recipientListId">The id of the recipient list to use as filter.</param>
        public int FilterOnRecipients(int recipientListId)
        {
            if (_id <= 0)
                throw new ArgumentException("Cannot filter work items for a job without id. Please save the job first.");

            WorkItemData dataUtil = GetWorker < WorkItemData>();
            int count = dataUtil.WorkItemFilterAgainstRecipientList(_id, recipientListId);
            
            // Remove any workitems from memory
            _workItems = null;

            // Counters are no longer valid
            ResetStatusCounters();

            return count;
        }

        /// <summary>
        /// Adds work items based on email addresses in a recipient list.
        /// </summary>
        /// <remarks>
        /// All new items and items that exists in both list will have their
        /// status set to NotStarted.
        /// </remarks>
        /// <param name="recipientListId">The recipient list id.</param>
        /// <returns>The number of new work items</returns>
        public int AddWorkItemsFromRecipientList(int recipientListId)
        {
            WorkItemData dataUtil = GetWorker < WorkItemData>();
            int count = dataUtil.WorkItemInsertFromRecipientList(_id, recipientListId, JobWorkStatus.NotStarted);

            // Remove any workitems from memory
            _workItems = null;

            // Counters are no longer valid
            ResetStatusCounters();

            return count;
        }

        /// <summary>
        /// Imports email addresses as work items for this job
        /// </summary>
        /// <param name="emailArray">The email addresses to import.</param>
        /// <returns>The number of email addresses imported as new work items. Duplicates are not part of this number.</returns>
        public int ImportEmailAddresses(string[] emailArray)
        {
            List<string> invalidEmailAddresses;
            return ImportEmailAddresses(emailArray, out invalidEmailAddresses);
        }

        public int ImportEmailAddresses(string emailAddressesDelimited, out List<string> invalidEmailAddresses)
        {
            // Ignored
            List<string> duplicateAddresses;
            
            // Do Import
            return ImportEmailAddresses(emailAddressesDelimited, out invalidEmailAddresses, out duplicateAddresses);
        }

        public int ImportEmailAddresses(string emailAddressesDelimited, out List<string> invalidEmailAddresses, out List<string> duplicateAddresses)
        {
            string[] emailAddressArray = emailAddressesDelimited.Split(new char[] { ';', ',', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Do Import
            return ImportEmailAddresses(emailAddressArray, out invalidEmailAddresses, out duplicateAddresses);
        }

        public int ImportEmailAddresses(string[] emailArray, out List<string> invalidEmailAddresses)
        {
            // Ignored
            List<string> duplicateAddresses;
            // Do Import
            return ImportEmailAddresses(emailArray, out invalidEmailAddresses, out duplicateAddresses);
        }

        /// <summary>
        /// Imports email addresses as work items for this job
        /// </summary>
        /// <param name="emailArray">The email addresses to import.</param>
        /// <param name="invalidEmailAddresses">A list of all available email addresses that could not be parsed as a valid address</param>
        /// <param name="duplicateAddresses">A list of duplicate email addresses added</param>
        /// <returns>The number of email addresses imported as new work items. Duplicates are not part of this number.</returns>
        public int ImportEmailAddresses(string[] emailArray, out List<string> invalidEmailAddresses, out List<string> duplicateAddresses)
        {
            JobWorkItems importedItems = new JobWorkItems();
            invalidEmailAddresses = new List<string>();
            duplicateAddresses = new List<string>();

            int numberOfNewItems = 0;
            foreach (string emailAddress in emailArray)
            {
                // TODO: This can be optimized by checking the
                // existence of these email addresses in batches

                // Clean address (this is done on save, so we need to make sure it's correct)
                string emailAddressCleaned = NewsLetterUtil.CleanEmailAddress(emailAddress);

                // Validate email address
                if (EmailSyntaxValidator.Validate(emailAddressCleaned) == false)
                {
                    // Invalid email address, skip it.
                    invalidEmailAddresses.Add(emailAddressCleaned);
                }
                else
                {
                    // Check if already imported. This is the quickest duplicate check
                    JobWorkItem workItem = importedItems.Items.FirstOrDefault(j => j.EmailAddress.Equals(emailAddressCleaned));
                    
                    if (workItem == null)
                    {
                        // Handle duplicates - try to load it first
                        workItem = JobWorkItem.Load(Id, emailAddressCleaned);
                        if (workItem == null)
                        {
                            // Create it, and save it. It is automatically
                            // added to the WorkItems collection
                            workItem = this.CreateWorkItem(emailAddressCleaned);
                            workItem.Save();
                            numberOfNewItems++;
                        }
                        else
                        {
                            // Duplicate
                            duplicateAddresses.Add(emailAddressCleaned);
                        }

                        // Add to imported collection, for quick
                        // in memory duplicate check
                        importedItems.Items.Add(workItem);
                    }
                }
            }

            // Counters are no longer valid
            ResetStatusCounters();

            return numberOfNewItems;
        }

        public static JobWorkItems ParseEmailAddressesToWorkItems(string emailAddresses)
        {
            return ParseEmailAddressesToWorkItems(emailAddresses, JobWorkStatus.NotStarted);
        }

        /// <summary>
        /// Parses the email addresses and creates in-memory work items.
        /// </summary>
        /// <remarks>
        /// During parsing, duplicates and invalid addresses are discarded.
        /// The work items has not been saved.
        /// </remarks>
        /// <param name="emailAddresses">The email addresses as a separated string to parse. Separators are ";, \n"</param>
        /// <returns>The email addresses as JobWorkItem objects</returns>
        public static JobWorkItems ParseEmailAddressesToWorkItems(string emailAddresses, JobWorkStatus defaultStatus)
        {
            JobWorkItems items = new JobWorkItems();
            string[] emailAddressArray = emailAddresses.Split(new char[] { ';', ',', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string emailAddress in emailAddressArray)
            {
                // Clean address
                string emailAddressCleaned = NewsLetterUtil.CleanEmailAddress(emailAddress);

                // Validate email address
                if (EmailSyntaxValidator.Validate(emailAddressCleaned) == true)
                {
                    // Check if already added.
                    JobWorkItem workItem = items.FirstOrDefault(j => j.EmailAddress.Equals(emailAddressCleaned));

                    if (workItem == null)
                    {
                        // Handle duplicates - try to load it first
                        workItem = new JobWorkItem(emailAddressCleaned, defaultStatus);

                        // Add to collection
                        items.Add(workItem);
                    }
                }
            }
            return items;
        }

        /// <summary>
        /// Creates a new work item with a given email address, 
        /// connected to this job, but does not add it to the WorkItems collection.
        /// The work item is not saved, it has to be explicitly saved.
        /// </summary>
        /// <remarks>
        /// There is no check for a duplicate email address for this
        /// job when creating the work item.
        /// </remarks>
        /// <returns>
        /// A new work item object with an email address connected to this job
        /// </returns>
        public JobWorkItem CreateWorkItem(string email)
        {
            // Get worker items for this job
            if (_id <= 0)
                throw new ApplicationException("Cannot create work items for a job that has no id. Save the job first to get the id.");

            JobWorkItem workItem = new JobWorkItem(_id, email, JobWorkStatus.NotStarted, null);
            return workItem;
        }

        /// <summary>
        /// Creates a new work item, connected to this job, and adds it to
        /// the WorkItems collection.
        /// The work item is not saved, it has to be explicitly saved.
        /// </summary>
        /// <returns>
        /// A new work item object connected to this job
        /// </returns>
        public JobWorkItem CreateWorkItem()
        {
            return CreateWorkItem(null);
        }

        /// <summary>
        /// Saves the job
        /// </summary>
        public void Save()
        {
            // verify paramterers before sending them to the database
            if (string.IsNullOrEmpty(Name))
                throw new ArgumentException("Name cannot be null or empty");

            if (Name.Length > 255)
                throw new ArgumentException("Name cannot be more then 255 characters");

            if (string.IsNullOrEmpty(Description) == false)
                if (Description.Length > 2000)
                    throw new ArgumentException("Description cannot be more then 2000 characters");

            JobData dataUtil = GetWorker<JobData>();

            if (Id == 0)
            {
                // New
                int newId = dataUtil.JobCreate(Name, PageId, Description);
                _id = newId;
            }
            else
            {
                // Edit existing
                dataUtil.JobEdit(_id, Name, Status, PageId, Description);
            }

            ResetStatusCounters();
        }

        /// <summary>
        /// Loads the specified job.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <returns>The job if found, null if no job with the id could be found</returns>
        public static Job Load(int jobId)
        {
            // Job job = new Jobs();
            JobData dataUtil = GetWorker<JobData>();
            DataSet jobsAndStatus = dataUtil.JobGetById(jobId);
            if (jobsAndStatus.Tables.Count != 2)
                throw new IndexOutOfRangeException("The JobGetAll procedure should return two tables. It returned " + jobsAndStatus.Tables.Count.ToString());

            DataTable jobTable = jobsAndStatus.Tables[0];
            
            // See if we found one
            if (jobTable.Rows.Count != 1)
            {
                return null;
            }

            // Found one
            
            // Table only has statuses for this job
            DataTable statusTable = jobsAndStatus.Tables[1];
            // ctor takes array as param, not collection
            DataRow[] statusRows = new DataRow[statusTable.Rows.Count];
            statusTable.Rows.CopyTo(statusRows, 0);

            Job job = new Job(jobTable.Rows[0], statusRows);
            return job;
        }

        /// <summary>
        /// Loads the Job by EPiServer page id.
        /// </summary>
        /// <param name="pageId">The page id to search for jobs against.</param>
        /// <returns>A Job object if found, null if not.</returns>
        public static Job LoadByPageId(int pageId)
        {
            JobData dataUtil = GetWorker<JobData>();
            // TODO: Change sp to return status counts too.
            DataTable jobTable = dataUtil.JobGetByPage(pageId);

            if (jobTable.Rows == null || jobTable.Rows.Count == 0)
                return null;

            // We could actually get more than one, that is a problem
            // for now, return the first one.
            Job job = new Job(jobTable.Rows[0]);
            return job;
        }

        /// <summary>
        /// Resets the status for all work items to the specified value.
        /// </summary>
        /// <param name="newStatus">The new status to give all work items.</param>
        public void SetStatusForWorkItems(JobWorkStatus newStatus)
        {
            // Get worker items for this job
            if (_id <= 0)
                throw new ApplicationException("Cannot change status for work items for a job that has no id. Save the job first to get the id.");

            WorkItemData dataUtil = GetWorker<WorkItemData>();
            dataUtil.WorkItemChangeStatusForAllWorkerItems(_id, newStatus);

            // Counters are no longer valid
            ResetStatusCounters();
        }


        public override string ToString()
        {
            return string.Format("ID:{0} \nName: {1} \nPageId: {2} \nDescription: {3}",
                                 _id.ToString(),
                                 Name ?? "(null)", 
                                 PageId.ToString(),
                                 Description ?? "(null)");
        }

    }
}
