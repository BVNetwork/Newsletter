using System.Collections;
using System.Collections.Generic;
using System.Data;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;

namespace BVNetwork.EPiSendMail.DataAccess
{
    public class JobWorkItems : IEnumerable<JobWorkItem>
    {
        public List<JobWorkItem> Items { get; private set; }

        public JobWorkItems()
        {
            Items = new List<JobWorkItem>();
        }

        public void Add(JobWorkItem item)
        {
            Items.Add(item);
        }

        public IEnumerator<JobWorkItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static WorkItemData GetWorker()
        {
            return EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<WorkItemData>();
        }

        public static JobWorkItems ListAll(int jobId)
        {
            JobWorkItems items = new JobWorkItems();
            WorkItemData dataUtil = GetWorker();
            DataTable workItemsTable = dataUtil.WorkItemGetAllForJob(jobId);
            FillFromDataTable(workItemsTable, items);
            return items;
        }
        public static JobWorkItems ListAll(Job job, JobWorkStatus status)
        {
            return ListAll(job.Id, status);
        }

        public static JobWorkItems ListAll(int jobId, JobWorkStatus status)
        {
            JobWorkItems items = new JobWorkItems();
            WorkItemData dataUtil = GetWorker();
            DataTable workItemsTable = dataUtil.WorkItemGetAllForJob(jobId, status);
            FillFromDataTable(workItemsTable, items);
            return items;
        }

        internal static void FillFromDataTable(DataTable table, JobWorkItems workItems)
        {
            foreach (DataRow row in table.Rows)
            {
                JobWorkItem workItem = new JobWorkItem(row);
                workItems.Items.Add(workItem);
            }
        }

        /// <summary>
        /// Searches the specified job for work items that matches
        /// a specified string. Will search all emails with a LIKE clause.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <param name="searchFor">The email to search for.</param>
        /// <returns>A collection of Work Items. The collection count can be 0</returns>
        public static JobWorkItems Search(int jobId, string searchFor)
        {
            JobWorkItems items = new JobWorkItems();
            WorkItemData dataUtil = GetWorker();
            DataTable workItemsTable = dataUtil.WorkItemSearch(jobId, searchFor);
            FillFromDataTable(workItemsTable, items);
            return items;
        }
        }
    

    //public class JobWorkItems2 : CollectionBase
    //{

    //    // public methods...
    //    public int Add(JobWorkItem jobWorkItem)
    //    {
    //        return List.Add(jobWorkItem);
    //    }

    //    public int IndexOf(JobWorkItem jobWorkItem)
    //    {
    //        for (int i = 0; i < List.Count; i++)
    //            if (this[i] == jobWorkItem)    // Found it
    //                return i;
    //        return -1;
    //    }

    //    public void Insert(int index, JobWorkItem jobWorkItem)
    //    {
    //        List.Insert(index, jobWorkItem);
    //    }

    //    public void Remove(JobWorkItem jobWorkItem)
    //    {
    //        List.Remove(jobWorkItem);
    //    }

    //    public JobWorkItem Find(string emailAddress)
    //    {
    //        foreach (JobWorkItem lJobWorkItemItem in this)
    //            if (string.Compare(lJobWorkItemItem.EmailAddress, emailAddress, true /* ignore case */) == 0)    // Found it
    //                return lJobWorkItemItem;
    //        return null;    // Not found
    //    }

    //    public JobWorkItem Find(JobWorkItem jobWorkItem)
    //    {
    //        foreach (JobWorkItem lJobWorkItemItem in this)
    //            if (lJobWorkItemItem == jobWorkItem)    // Found it
    //                return lJobWorkItemItem;
    //        return null;    // Not found
    //    }

    //    public bool Contains(JobWorkItem jobWorkItem)
    //    {
    //        return (Find(jobWorkItem) != null);
    //    }

    //    public void Delete()
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    // public properties...
    //    public JobWorkItem this[int index]
    //    {
    //        get
    //        {
    //            return (JobWorkItem)List[index];
    //        }
    //        set
    //        {
    //            List[index] = value;
    //        }
    //    }

    //    public static JobWorkItems ListAll(int jobId)
    //    {
    //        JobWorkItems items = new JobWorkItems();
    //        DataUtil.WorkItemData dataUtil = new DataUtil.WorkItemData();
    //        DataTable workItemsTable = dataUtil.WorkItemGetAllForJob(jobId);
    //        FillFromDataTable(workItemsTable, items);
    //        return items;
    //    }
    //    public static JobWorkItems ListAll(Job job, JobWorkStatus status)
    //    {
    //        return ListAll(job.Id, status);
    //    }

    //    public static JobWorkItems ListAll(int jobId, JobWorkStatus status)
    //    {
    //        JobWorkItems items = new JobWorkItems();
    //        DataUtil.WorkItemData dataUtil = new DataUtil.WorkItemData();
    //        DataTable workItemsTable = dataUtil.WorkItemGetAllForJob(jobId, status);
    //        FillFromDataTable(workItemsTable, items);
    //        return items;
    //    }

    //    internal static void FillFromDataTable(DataTable table, JobWorkItems workItems)
    //    {
    //        foreach (DataRow row in table.Rows)
    //        {
    //            JobWorkItem workItem = new JobWorkItem(row);
    //            workItems.Add(workItem);
    //        }
    //    }

    //    /// <summary>
    //    /// Searches the specified job for work items that matches
    //    /// a specified string. Will search all emails with a LIKE clause.
    //    /// </summary>
    //    /// <param name="jobId">The job id.</param>
    //    /// <param name="searchFor">The email to search for.</param>
    //    /// <returns>A collection of Work Items. The collection count can be 0</returns>
    //    public static JobWorkItems Search(int jobId, string searchFor)
    //    {
    //        JobWorkItems items = new JobWorkItems();
    //        DataUtil.WorkItemData dataUtil = new DataUtil.WorkItemData();
    //        DataTable workItemsTable = dataUtil.WorkItemSearch(jobId, searchFor);
    //        FillFromDataTable(workItemsTable, items);
    //        return items;
    //    }
    //}

}
