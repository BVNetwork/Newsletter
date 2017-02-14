using System;
using System.Data;
using System.Data.Common;
using EPiServer.Data;
using EPiServer.DataAccess;

namespace BVNetwork.EPiSendMail.DataAccess.DataUtil
{
    /// <summary>
    /// Data access functions for getting data to and from the database
    /// </summary>
    public class JobData : DataAccessBase
    {
        public JobData(IDatabaseExecutor databaseHandler)
            : base(databaseHandler)
        {
            this.Executor = databaseHandler;
        }


        public int JobCreate(string name, int pageId, string description)
        {
            JobStatus status = JobStatus.Editing;
            int id = 0;

            Executor.Execute(() =>
                                      {
                                          DbCommand cmd = CreateCommand("NewsletterJobCreate");
                                          cmd.Parameters.Add(CreateParameter("name", name));
                                          cmd.Parameters.Add(CreateParameter("pageId", pageId));
                                          cmd.Parameters.Add(CreateParameter("status", status));
                                          cmd.Parameters.Add(CreateParameter("description", description));
                                          object objId = cmd.ExecuteScalar();
                                          if (objId == null)
                                          {
                                              throw new ApplicationException(
                                                  "Unable to create newsletter job. No Id returned.");
                                          }
                                          if (int.TryParse(objId.ToString(), out id) == false)
                                          {
                                              throw new ApplicationException(
                                                  "Unable to create newsletter job. Unexpected return value: " + objId);
                                          }
                                      });

            return id;

        }

        public void JobEdit(int jobId, string name, JobStatus status, int pageId, string description)
        {
            Executor.Execute(() =>
                                      {
                                          DbCommand cmd = CreateCommand("NewsletterJobEdit");
                                          cmd.Parameters.Add(CreateParameter("jobid", jobId));
                                          cmd.Parameters.Add(CreateParameter("name", name));
                                          cmd.Parameters.Add(CreateParameter("pageId", pageId));
                                          cmd.Parameters.Add(CreateParameter("status", status));
                                          cmd.Parameters.Add(CreateParameter("description", description));
                                          cmd.ExecuteNonQuery();
                                      });

        }

        /// <summary>
        /// Gets all jobs from the database and the count of items for each job status
        /// </summary>
        /// <returns>A dataset with two tables</returns>
        public DataSet JobGetAll()
        {
            DataSet jobs = new DataSet();

            Executor.Execute<DataSet>(() =>
                                          {
                                              DbCommand cmd = CreateCommand("NewsletterJobGetAll");
                                              DbDataAdapter adapter = CreateDataAdapter(cmd);
                                              adapter.Fill(jobs);
                                              return jobs;
                                          });

            return jobs;
        }

        /// <summary>
        /// Gets all jobs from the database with a given status and the count of items for each job status
        /// </summary>
        /// <returns>A dataset with two tables</returns>
        public DataSet JobGetAllByStatus(JobStatus status)
        {
            DataSet jobs = new DataSet();
            Executor.Execute<DataSet>(() =>
                                          {
                                              DbCommand cmd = CreateCommand("NewsletterJobGetAllByStatus");
                                              cmd.Parameters.Add(base.CreateParameter("status", status));

                                              DbDataAdapter adapter = CreateDataAdapter(cmd);
                                              adapter.Fill(jobs);
                                              return jobs;
                                          });
            return jobs;
        }

        /// <summary>
        /// Gets one job from the database
        /// </summary>
        /// <returns></returns>
        public DataSet JobGetById(int jobId)
        {
            DataSet jobs = new DataSet();
            Executor.Execute(() =>
                                          {
                                              DbCommand cmd = base.CreateCommand("NewsletterJobGet");
                                              cmd.Parameters.Add(base.CreateParameter("jobid", jobId));

                                              DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                              adapter.Fill(jobs);
                                          });
            return jobs;
        }

        /// <summary>
        /// Gets all jobs to a page from the database
        /// </summary>
        /// <returns></returns>
        public DataTable JobGetByPage(int pageId)
        {
            DataTable jobs = new DataTable();
            Executor.Execute(() =>
                                          {
                                              DbCommand  cmd = base.CreateCommand("NewsletterJobGetByPage");
                                              cmd.Parameters.Add(base.CreateParameter("pageId", pageId));
                                              DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                              adapter.Fill(jobs);
                                          });
            return jobs;
        }

        /// <summary>
        /// Deletes a job, and all corresponding worker items
        /// </summary>
        /// <param name="jobId">The id to locate the job by.</param>
        public void JobDelete(int jobId)
        {
            Executor.Execute(() =>
                                          {
                                              DbCommand cmd = CreateCommand("NewsletterJobDelete");
                                              cmd.Parameters.Add(CreateParameter("jobid", jobId));
                                              cmd.ExecuteNonQuery();
                                              
                                          });
        }
    }
}
