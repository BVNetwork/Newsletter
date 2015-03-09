using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;

namespace BVNetwork.EPiSendMail.DataAccess
{
    public class Jobs : IEnumerable<Job>
    {
         public List<Job> Items { get; private set; }

         public Jobs()
        {
            Items = new List<Job>();
        }

        public void Add(Job item)
        {
            Items.Add(item);
        }

        public IEnumerator<Job> GetEnumerator()
        {
            return Items.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static JobData GetWorker()
        {
            return EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<JobData>();
        }



        /// <summary>
        /// Lists all jobs with a given status
        /// </summary>
        /// <param name="status">The status to list jobs by</param>
        /// <returns>A collection of jobs with the specified status</returns>
        public static Jobs ListAll(JobStatus status)
        {
            JobData dataUtil = GetWorker();
            DataSet jobsAndStatus = dataUtil.JobGetAllByStatus(status);

            if (jobsAndStatus.Tables.Count != 2)
                throw new IndexOutOfRangeException("The JobGetAllByStatus procedure should return two tables. It returned " + jobsAndStatus.Tables.Count.ToString());

            return GetJobsFromDataSet(jobsAndStatus);
        }

        /// <summary>
        /// Lists all jobs
        /// </summary>
        /// <returns>A Jobs collection of all jobs</returns>
        public static Jobs ListAll()
        {
            JobData dataUtil = GetWorker();
            DataSet jobsAndStatus = dataUtil.JobGetAll();
            if (jobsAndStatus.Tables.Count != 2)
                throw new IndexOutOfRangeException("The JobGetAll procedure should return two tables. It returned " + jobsAndStatus.Tables.Count.ToString());

            return GetJobsFromDataSet(jobsAndStatus);
        }

        private static Jobs GetJobsFromDataSet(DataSet jobsAndStatus)
        {
            Jobs jobs = new Jobs();
            DataTable jobsTable = jobsAndStatus.Tables[0];
            DataTable statusTable = jobsAndStatus.Tables[1];

            foreach (DataRow row in jobsTable.Rows)
            {
                int jobId = (int)row["pkJobId"];
                DataRow[] statusRows = statusTable.Select("pkJobId = " + jobId.ToString());

                Job job = new Job(row, statusRows);
                jobs.Add(job);
            }
            return jobs;
        }
    }

    //public class Jobs : CollectionBase
    //{
    //    // public methods...
    //    public int Add(Job job)
    //    {
    //        return List.Add(job);
    //    }

    //    public int IndexOf(Job job)
    //    {
    //        for (int i = 0; i < List.Count; i++)
    //            if (this[i] == job)    // Found it
    //                return i;
    //        return -1;
    //    }

    //    /// <summary>
    //    /// Removes a job from this collection, but does not delete it from the
    //    /// database. This is an in memory operation only, no actions will be
    //    /// performed on the job or its worker items in the database.
    //    /// </summary>
    //    public void Remove(Job job)
    //    {
    //        List.Remove(job);
    //    }

    //    public void Remove(int jobId)
    //    {
    //        Job job = Find(jobId);
    //        if (job != null)
    //        {
    //            List.Remove(job);
    //        }
    //    }

    //    public Job Find(Job job)
    //    {
    //        foreach (Job jobItem in this)
    //            if (jobItem == job)    // Found it
    //                return jobItem;
    //        return null;    // Not found
    //    }

    //    public Job Find(int jobId)
    //    {
    //        foreach (Job jobItem in this)
    //            if (jobItem.Id == jobId)    // Found it
    //                return jobItem;
    //        return null;    // Not found
    //    }

    //    public bool Contains(Job job)
    //    {
    //        return (Find(job) != null);
    //    }
        
    //    public bool Contains(int jobId)
    //    {
    //        return (Find(jobId) != null);
    //    }

    //    /// <summary>
    //    /// Creates a new job, and return the instanciated job
    //    /// </summary>
    //    public static Job Create(string name)
    //    {
    //        return Create(name, null, 0);
    //    }

    //    public static Job Create(string name, string description)
    //    {
    //        return Create(name, description, 0);
    //    }

    //    public static Job Create(string name, string description, int pageId)
    //    {
    //        Job job = new Job(pageId, name, description);
    //        job.Save();
    //        return job;
    //    }



    //    /// <summary>
    //    /// Lists all jobs with a given status
    //    /// </summary>
    //    /// <param name="status">The status to list jobs by</param>
    //    /// <returns>A collection of jobs with the specified status</returns>
    //    public static Jobs ListAll(JobStatus status)
    //    {
    //        DataUtil.JobData dataUtil = new DataUtil.JobData();
    //        DataSet jobsAndStatus = dataUtil.JobGetAllByStatus(status);

    //        if (jobsAndStatus.Tables.Count != 2)
    //            throw new IndexOutOfRangeException("The JobGetAllByStatus procedure should return two tables. It returned " + jobsAndStatus.Tables.Count.ToString());

    //        return GetJobsFromDataSet(jobsAndStatus);
    //    }

    //    /// <summary>
    //    /// Lists all jobs
    //    /// </summary>
    //    /// <returns>A Jobs collection of all jobs</returns>
    //    public static Jobs ListAll()
    //    {
    //        DataUtil.JobData dataUtil = new DataUtil.JobData();
    //        DataSet jobsAndStatus = dataUtil.JobGetAll();
    //        if (jobsAndStatus.Tables.Count != 2)
    //            throw new IndexOutOfRangeException("The JobGetAll procedure should return two tables. It returned " + jobsAndStatus.Tables.Count.ToString());

    //        return GetJobsFromDataSet(jobsAndStatus);
    //    }

    //    private static Jobs GetJobsFromDataSet(DataSet jobsAndStatus)
    //    {
    //        Jobs jobs = new Jobs();
    //        DataTable jobsTable = jobsAndStatus.Tables[0];
    //        DataTable statusTable = jobsAndStatus.Tables[1];

    //        foreach (DataRow row in jobsTable.Rows)
    //        {
    //            int jobId = (int)row["pkJobId"];
    //            DataRow[] statusRows = statusTable.Select("pkJobId = " + jobId.ToString());

    //            Job job = new Job(row, statusRows);
    //            jobs.Add(job);
    //        }
    //        return jobs;
    //    }

    //    // public properties...
    //    #region this[int index]
    //    /// <summary>
    //    /// A collection of Job instances
    //    /// </summary>
    //    public Job this[int index]
    //    {
    //        get
    //        {
    //            return (Job)List[index];
    //        }
    //        set
    //        {
    //            List[index] = value;
    //        }
    //    }
    //    #endregion
    //}
    
}
