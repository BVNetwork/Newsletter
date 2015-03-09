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
    internal class WorkItemData : DataAccessBase
    {
        public WorkItemData(IDatabaseHandler databaseHandler)
            : base(databaseHandler)
        {
            this.Database = databaseHandler;
        }
        /// <summary>
        /// Gets one work item based on job id and email address.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>A DataRow with the work item if successfull, null if not found</returns>
        public DataRow WorkItemGet(int jobId, string emailAddress)
        {
            DataTable workItem = new DataTable();
            Database.Execute(() =>
                             {
                                 DbCommand  cmd = base.CreateCommand("NewsletterWorkItemGet");
                                 cmd.Parameters.Add(base.CreateParameter("jobid", jobId));
                                 cmd.Parameters.Add(base.CreateParameter("emailaddress", emailAddress));

                                 System.Data.Common.DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                 adapter.Fill(workItem);

                                 if (workItem.Rows.Count > 1)
                                     throw new ApplicationException("More than one (" + workItem.Rows.Count.ToString() +
                                         ") work item rows returned for job " + jobId.ToString() + " and email '" + emailAddress +
                                         "'). This is unexpected and might imply data corruption.");

                                 if (workItem.Rows.Count == 1)
                                 {
                                     return workItem.Rows[0];
                                 }
                                 return null;

                             });
            return null;
        }

        /// <summary>
        /// Gets all work items for a job.
        /// </summary>
        /// <param name="jobId">The job id</param>
        /// <returns></returns>
        public DataTable WorkItemGetAllForJob(int jobId)
        {
            DataTable workItems = new DataTable();
            Database.Execute(() =>
                              {
                                  DbCommand cmd = base.CreateCommand("NewsletterWorkItemGetAllForJob");
                                  cmd.Parameters.Add(base.CreateParameter("jobid", jobId));

                                  System.Data.Common.DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                  adapter.Fill(workItems);
                              });

            return workItems;
        }

        /// <summary>
        /// Gets a batch of work items ready for processing. All work items
        /// will be updated with the status given
        /// </summary>
        /// <param name="jobId">The job id</param>
        /// <param name="status">The status all the work items in the batch should be set to</param>
        /// <returns></returns>
        public DataTable WorkItemGetBatchForProcessing(int jobId, JobWorkStatus getStatus, JobWorkStatus setStatus, int count)
        {
            DataTable workItems = new DataTable();
            Database.Execute(() =>
                             {
                                 DbCommand  cmd = base.CreateCommand("NewsletterWorkItemGetBatchForProcessing");
                                 cmd.Parameters.Add(base.CreateParameter("jobid", jobId));
                                 cmd.Parameters.Add(base.CreateParameter("selectStatus", getStatus));
                                 cmd.Parameters.Add(base.CreateParameter("updatedStatus", setStatus));
                                 cmd.Parameters.Add(base.CreateParameter("count", count));

                                 System.Data.Common.DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                 adapter.Fill(workItems);
                             });

            return workItems;
        }

        /// <summary>
        /// Gets all work items for a job with a given status
        /// </summary>
        /// <param name="jobId">The job id</param>
        /// <returns></returns>
        public DataTable WorkItemGetAllForJob(int jobId, JobWorkStatus status)
        {
            DataTable workItems = new DataTable();
            Database.Execute(() =>
                              {
                                  DbCommand  cmd = base.CreateCommand("NewsletterWorkItemGetAllWithStatusForJob");
                                  cmd.Parameters.Add(base.CreateParameter("jobid", jobId));
                                  cmd.Parameters.Add(base.CreateParameter("status", status));

                                  System.Data.Common.DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                  adapter.Fill(workItems);
                              });
            return workItems;
        }

        /// <summary>
        /// Searches for work items doing a LIKE query on the
        /// email address.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <param name="searchFor">The email to search for.</param>
        /// <returns>A DataTable with the results</returns>
        public DataTable WorkItemSearch(int jobId, string searchFor)
        {
            DataTable workItems = new DataTable();
            Database.Execute(() =>
                                 {
                                     DbCommand cmd = base.CreateCommand("NewsletterWorkItemSearch");
                                     cmd.Parameters.Add(base.CreateParameter("jobid", jobId));
                                     cmd.Parameters.Add(base.CreateParameter("searchfor", searchFor));

                                     System.Data.Common.DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                     adapter.Fill(workItems);
                                 });
            return workItems;
        }

        /// <summary>
        /// Changes the information for a work item. If the job id and email address does not
        /// exist, the work item is added.
        /// </summary>
        /// <remarks>
        /// Does not return anything, as the jobId and emailadress is a composite
        /// primary key.
        /// </remarks>
        /// <param name="jobId">The job id.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="status">The status.</param>
        /// <param name="info">The info.</param>
        public void WorkItemEdit(int jobId, string emailAddress, JobWorkStatus status, string info)
        {
            Database.Execute(() =>
                                 {
                                     DbCommand cmd = CreateCommand("NewsletterWorkItemEdit");
                                     cmd.Parameters.Add(CreateParameter("jobid", jobId));
                                     cmd.Parameters.Add(CreateParameter("emailaddress", emailAddress));
                                     cmd.Parameters.Add(CreateParameter("status", status));
                                     cmd.Parameters.Add(CreateParameter("info", info));
                                     cmd.ExecuteNonQuery();
                                 });
        }

        /// <summary>
        /// Changes status for a work item
        /// </summary>
        /// <remarks>
        /// Only updates the status field of a work item. Throws an exception if
        /// the work item does not exist
        /// </remarks>
        /// <param name="jobId">The job id.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="status">The status.</param>
        public void WorkItemChangeStatus(int jobId, string emailAddress, JobWorkStatus status)
        {
            Database.Execute(() =>
                                 {
                                     DbCommand cmd = CreateCommand("NewsletterWorkItemChangeStatus");
                                     cmd.Parameters.Add(CreateParameter("jobid", jobId));
                                     cmd.Parameters.Add(CreateParameter("emailaddress", emailAddress));
                                     cmd.Parameters.Add(CreateParameter("status", status));
                                     cmd.ExecuteNonQuery();
                                 });
        }

        /// <summary>
        /// Changes the status for all worker items for a given job.
        /// </summary>
        /// <remarks>
        /// This will also reset the comment for each work item, effectively
        /// resetting the work items table to a spesific state
        /// </remarks>
        /// <param name="jobId">The job id.</param>
        /// <param name="status">The status to update all work items with.</param>
        public void WorkItemChangeStatusForAllWorkerItems(int jobId, JobWorkStatus status)
        {
            Database.Execute(() =>
                                 {
                                     DbCommand cmd = CreateCommand("NewsletterWorkItemChangeStatusForAll");
                                     cmd.Parameters.Add(CreateParameter("jobid", jobId));
                                     cmd.Parameters.Add(CreateParameter("status", status));
                                     cmd.ExecuteNonQuery();
                                 });
        }

        /// <summary>
        /// Sets the status of several work items to complete status.
        /// </summary>
        /// <remarks>
        /// This will update all the work items for a given job and the email
        /// addresses provided to the complete status. If an email address does
        /// not exist, it is ignored.
        /// </remarks>
        /// <param name="jobId">The job id.</param>
        /// <param name="emailAddresses">The email addresses to update the status for.</param>
        public void WorkItemSetToComplete(int jobId, params string[] emailAddresses)
        {
            int numberOfWorkItemsToSend = 25; // 4000 max param chars / 150 char per address

            string emailAddressesSeperated;
            int numberOfIterations = (emailAddresses.Length % numberOfWorkItemsToSend);

            for (int i = 0; i <= numberOfIterations; i++)
            {
                if (i == numberOfIterations)
                    numberOfWorkItemsToSend = emailAddresses.Length - (i * numberOfWorkItemsToSend);

                emailAddressesSeperated = string.Join(",", emailAddresses, i * numberOfWorkItemsToSend + 1, numberOfWorkItemsToSend);

                Database.Execute(() =>
                                     {
                                         DbCommand cmd = CreateCommand("NewsletterWorkItemSetComplete");
                                         cmd.Parameters.Add(CreateParameter("jobid", jobId));
                                         cmd.Parameters.Add(CreateParameter("status", JobWorkStatus.Complete));
                                         cmd.Parameters.Add(CreateParameter("emailAddress", emailAddressesSeperated));
                                         cmd.ExecuteNonQuery();
                                     });
            }

        }

        /// <summary>
        /// Deletes the work item for a given job.
        /// </summary>
        /// <remarks>
        /// If the work item does not exist the exception is ignored
        /// </remarks>
        /// <param name="jobId">The job id.</param>
        /// <param name="emailAddress">The email address to delete.</param>
        public void WorkItemDelete(int jobId, string emailAddress)
        {
            Database.Execute(() =>
                                 {
                                     DbCommand cmd = CreateCommand("NewsletterWorkItemRemoveItem");
                                     cmd.Parameters.Add(CreateParameter("jobid", jobId));
                                     cmd.Parameters.Add(CreateParameter("emailAddress", emailAddress));
                                     cmd.ExecuteNonQuery();
                                 });
        }

        /// <summary>
        /// Deletes all work items for a job.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        public void WorkItemDeleteAllForJob(int jobId)
        {
            Database.Execute(() =>
                                 {
                                     DbCommand cmd = CreateCommand("NewsletterWorkItemDeleteAllForJob");
                                     cmd.Parameters.Add(CreateParameter("jobid", jobId));
                                     cmd.ExecuteNonQuery();
                                 });
        }

        /// <summary>
        /// Filters the work items for a job against the addresses in a recipient list, removing
        /// all work items that has email addresses that exists in both lists.
        /// </summary>
        /// <remarks>
        /// Commonly used to delete email addresses that are part of block lists.
        /// </remarks>
        /// <param name="jobId">The job id.</param>
        /// <param name="recipientListId">The recipient list id.</param>
        public int WorkItemFilterAgainstRecipientList(int jobId, int recipientListId)
        {
            int count = 0;
            Database.Execute(() =>
                                 {
                                     DbCommand cmd = CreateCommand("NewsletterWorkItemFilterList");
                                     cmd.Parameters.Add(CreateParameter("jobid", jobId));
                                     cmd.Parameters.Add(CreateParameter("recipientListId", recipientListId));
                                     object objCount = cmd.ExecuteScalar();

                                     if (objCount == null)
                                     {
                                         throw new ApplicationException(
                                             "Unable to read count on filter work items procedure.");
                                     }
                                     if (int.TryParse(objCount.ToString(), out count) == false)
                                     {
                                         throw new ApplicationException(
                                             "Unable to cast filter count to integer. Unexpected return value: " +
                                             objCount.ToString());
                                     }
                                 });
            return count;
        }

        /// <summary>
        /// Returns the number of work items of a given status for a job
        /// </summary>
        /// <param name="jobId">The job id to query work items for</param>
        /// <param name="status">The status to query for</param>
        public int WorkItemGetCountByStatus(int jobId, JobWorkStatus status)
        {
            int count = 0;
            Database.Execute<int>(() =>
                                 {
                                     DbCommand cmd = CreateCommand("NewsletterWorkItemGetCountForStatusForJob");
                                     cmd.Parameters.Add(CreateParameter("jobid", jobId));
                                     cmd.Parameters.Add(CreateParameter("status", status));
                                     object objCount = cmd.ExecuteScalar();

                                     if (objCount == null)
                                     {
                                         // throw new ApplicationException("Unable to read count value.");
                                         // null means zero records
                                         return 0;
                                     }
                                     if (int.TryParse(objCount.ToString(), out count) == false)
                                     {
                                         throw new ApplicationException(
                                             "Unable to cast count to integer. Unexpected return value: " +
                                             objCount.ToString());
                                     }

                                     return count;
                                 });
            return count;

        }

        /// <summary>
        /// Inserts email addresses from a recipient list into a work items list.
        /// </summary>
        /// <remarks>
        /// The status on new and existing items in the list is changed to the status parameter
        /// </remarks>
        /// <param name="jobId">The job id.</param>
        /// <param name="recipientListId">The recipient list id.</param>
        /// <param name="status">The status to update all new or matching work items</param>
        /// <returns>Returns the number of items that was inserted, which might 
        /// be less than the total count of items in the recipient list</returns>
        public int WorkItemInsertFromRecipientList(int jobId, int recipientListId, JobWorkStatus status)
        {
            int count = 0;
            Database.Execute(() =>
                                 {
                                     DbCommand cmd = CreateCommand("NewsletterWorkItemInsertFromRecipient");
                                     cmd.Parameters.Add(CreateParameter("jobid", jobId));
                                     cmd.Parameters.Add(CreateParameter("recipientListId", recipientListId));
                                     cmd.Parameters.Add(CreateParameter("status", status));
                                     object objCount = cmd.ExecuteScalar();

                                     if (objCount == null)
                                     {
                                         throw new ApplicationException("Unable to read count of added work items.");
                                     }
                                     if (int.TryParse(objCount.ToString(), out count) == false)
                                     {
                                         throw new ApplicationException(
                                             "Unable to cast count to integer. Unexpected return value: " +
                                             objCount.ToString());
                                     }
                                 });
            return count;
        }
    }
}
