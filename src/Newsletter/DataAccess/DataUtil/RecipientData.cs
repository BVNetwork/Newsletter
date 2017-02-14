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
    internal class RecipientData : DataAccessBase
    {
        public RecipientData(IDatabaseExecutor databaseHandler)
            : base(databaseHandler)
        {
            this.Executor = databaseHandler;
        }

        /// <summary>
        /// Create a new recipientlist.
        /// </summary>
        /// <param name="listType">Enum, list type</param>
        /// <param name="name">List name</param>
        /// <param name="description">List description</param>
        /// <returns></returns>
        public int RecipientListCreate(RecipientListType listType, string name, string description)
        {
            int recipientId = 0;

            Executor.Execute(() =>
                                 {
                                     DbCommand cmd = base.CreateCommand("NewsletterRecipientListCreate");
                                     cmd.Parameters.Add(base.CreateParameter("listType", listType));
                                     cmd.Parameters.Add(base.CreateParameter("name", name));
                                     cmd.Parameters.Add(base.CreateParameter("description", description));

                                     object objId = cmd.ExecuteScalar();

                                     if (objId == null)
                                     {
                                         throw new ApplicationException(
                                             "Unable to create recipient list. No Id returned.");
                                     }
                                     if (int.TryParse(objId.ToString(), out recipientId) == false)
                                     {
                                         throw new ApplicationException(
                                             "Unable to create recipient list. Unexpected return value: " +
                                             objId.ToString());
                                     }
                                 });

            return recipientId;

        }

        /// <summary>
        /// Edit the recipient list 
        /// </summary>
        /// <param name="recipientListId">The recipient list id.</param>
        /// <param name="listType">Enum, list type.</param>
        /// <param name="name">Name of recipient list.</param>
        /// <param name="description"></param>
        public void RecipientListEdit(int recipientListId, RecipientListType listType, string name, string description)
        {
            Executor.Execute(() =>
                                 {
                                     DbCommand cmd = base.CreateCommand("NewsletterRecipientListEdit");
                                     cmd.Parameters.Add(base.CreateParameter("recipientlistid", recipientListId));
                                     cmd.Parameters.Add(base.CreateParameter("listType", listType));
                                     cmd.Parameters.Add(base.CreateParameter("name", name));
                                     cmd.Parameters.Add(base.CreateParameter("description", description));

                                     cmd.ExecuteNonQuery();
                                 });

        }

        /// <summary>
        /// Gets all recipient lists from the database
        /// </summary>
        /// <returns>All recipient lists</returns>
        public DataTable RecipientListGetAll()
        {
            DataTable recipientLists = new DataTable();
            Executor.Execute(() =>
                                 {
                                     DbCommand cmd = base.CreateCommand("NewsletterRecipientListGetAll");
                                     DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                     adapter.Fill(recipientLists);
                                 });

            return recipientLists;
        }

        /// <summary>
        /// Gets all recipient lists from the database for sepcified user email
        /// </summary>
        /// <returns>All recipient lists</returns>
        public DataTable RecipientListGetAllByEmail(string email)
        {
            DataTable recipientLists = new DataTable();
            Executor.Execute(() =>
                                  {
                                      DbCommand cmd = base.CreateCommand("NewsletterRecipientListGetAllByEmail");
                                      cmd.Parameters.Add(base.CreateParameter("email", email));
                                      System.Data.Common.DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                      adapter.Fill(recipientLists);
                                  });

            return recipientLists;
        }

        /// <summary>
        /// Gets all email items for a given recipient lists from the Executor.
        /// </summary>
        /// <remarks>
        /// The list is sorted by added date, with the most recently added items first
        /// </remarks>
        /// <param name="recipientListId">The list to retrieve items for</param>
        /// <returns>All email addresses for a recipient lists</returns>
        public DataTable RecipientListGetAllItems(int recipientListId)
        {
            DataTable recipientListItems = new DataTable();
            Executor.Execute(() =>
                              {
                                  DbCommand cmd = base.CreateCommand("NewsletterRecipientListGetAllItems");
                                  cmd.Parameters.Add(base.CreateParameter("recipientlistid", recipientListId));
                                  System.Data.Common.DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                  adapter.Fill(recipientListItems);

                              });

            return recipientListItems;
        }

        /// <summary>
        /// Gets one recipient list from the database
        /// </summary>
        /// <returns></returns>
        public DataTable RecipientListGetById(int recipientListId)
        {
            DataTable recipientList = new DataTable();
            Executor.Execute(() =>
                                 {
                                     DbCommand cmd = base.CreateCommand("NewsletterRecipientListGet");
                                     cmd.Parameters.Add(base.CreateParameter("recipientlistid", recipientListId));

                                     System.Data.Common.DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                     adapter.Fill(recipientList);
                                 });
            return recipientList;
        }

        /// <summary>
        /// Adds or changes information for an email address in a recipient list.
        /// </summary>
        /// <remarks>
        /// The added field will be set automatically to the timestamp
        /// the address is added to the Executor.
        /// </remarks>
        /// <param name="recipientListId">The recipient list id to store this email address on.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="comment">A comment for this address. Can be null.</param>
        /// <param name="source">Emailaddress source (Enum)</param>
        public void RecipientListEditItem(int recipientListId, string emailAddress, string comment, EmailAddressSource source)
        {
            Executor.Execute(() =>
                                  {
                                      DbCommand cmd = base.CreateCommand("NewsletterRecipientListEditItem");
                                      cmd.Parameters.Add(base.CreateParameter("recipientlistid", recipientListId));
                                      cmd.Parameters.Add(base.CreateParameter("emailAddress", emailAddress));
                                      cmd.Parameters.Add(base.CreateParameter("comment", comment));
                                      cmd.Parameters.Add(base.CreateParameter("source", source));

                                      cmd.ExecuteNonQuery();
                                  });
        }

        public void RecipientListAddItem(int recipientListId, string emailAddress, string comment, EmailAddressSource source)
        {
            Executor.Execute(() =>
                                 {
                                     DbCommand
                                         cmd = base.CreateCommand("NewsletterRecipientListAddItem");
                                     cmd.Parameters.Add(base.CreateParameter("recipientlistid", recipientListId));
                                     cmd.Parameters.Add(base.CreateParameter("emailAddress", emailAddress));
                                     cmd.Parameters.Add(base.CreateParameter("comment", comment));
                                     cmd.Parameters.Add(base.CreateParameter("source", source));

                                     cmd.ExecuteNonQuery();
                                 });
        }

        /// <summary>
        /// Removes an email address from a recipient list.
        /// </summary>
        /// <param name="recipientListId">The recipient list id.</param>
        /// <param name="emailAddress">The email address.</param>
        public void RecipientListRemoveItem(int recipientListId, string emailAddress)
        {
            Executor.Execute(() =>
                                 {
                                     DbCommand cmd = base.CreateCommand("NewsletterRecipientListRemoveItem");
                                     cmd.Parameters.Add(base.CreateParameter("recipientlistid", recipientListId));
                                     cmd.Parameters.Add(base.CreateParameter("emailAddress", emailAddress));

                                     cmd.ExecuteNonQuery();
                                 });
        }

        /// <summary>
        /// Removes all email addresses from a recipient list.
        /// </summary>
        /// <param name="recipientListId">The recipient list id.</param>
        public void RecipientListRemoveAllItems(int recipientListId)
        {
            Executor.Execute(() =>
                                 {
                                     DbCommand cmd = base.CreateCommand("NewsletterRecipientListRemoveAllItems");
                                     cmd.Parameters.Add(base.CreateParameter("recipientlistid", recipientListId));
                                     cmd.ExecuteNonQuery();
                                 });
        }

        /// <summary>
        /// Deletes the recipient list and all its email addresses
        /// </summary>
        /// <param name="recipientListId">The recipient list id.</param>
        public void RecipientListDelete(int recipientListId)
        {
            Executor.Execute(() =>
                                 {
                                     DbCommand cmd = base.CreateCommand("NewsletterRecipientListDelete");
                                     cmd.Parameters.Add(base.CreateParameter("recipientlistid", recipientListId));
                                     cmd.ExecuteNonQuery();
                                 });
        }


        public DataTable RecipientListSearch(int recipientListId, string searchFor)
        {
            DataTable items = new DataTable();
            Executor.Execute(() =>
                                  {
                                      DbCommand  cmd = base.CreateCommand("NewsletterRecipientListSearch");
                                      cmd.Parameters.Add(base.CreateParameter("recipientListid", recipientListId));
                                      cmd.Parameters.Add(base.CreateParameter("searchfor", searchFor));

                                      DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                      adapter.Fill(items);
                                  });
            return items;
        }


        /// <summary>
        /// Gets one email address item for a recipient list.
        /// </summary>
        /// <param name="recipientListId">The recipient list id.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>One row of data with email address information</returns>
        public DataRow RecipientListGetItem(int recipientListId, string emailAddress)
        {
            DataTable emailAddresses = new DataTable();
            return Executor.Execute(() =>
                             {
                                 DbCommand  cmd = base.CreateCommand("NewsletterRecipientListGetItem");
                                 cmd.Parameters.Add(base.CreateParameter("recipientlistid", recipientListId));
                                 cmd.Parameters.Add(base.CreateParameter("emailaddress", emailAddress));

                                 System.Data.Common.DbDataAdapter adapter = base.CreateDataAdapter(cmd);
                                 adapter.Fill(emailAddresses);

                                 if (emailAddresses.Rows.Count > 1)
                                     throw new ApplicationException("More than one (" + emailAddresses.Rows.Count.ToString() +
                                         ") email address item rows returned for recipient list " + recipientListId.ToString() + " and email '" + emailAddress +
                                         "'). This is unexpected and might imply data corruption.");

                                 if (emailAddresses.Rows.Count == 1)
                                 {
                                     return emailAddresses.Rows[0];
                                 }

                                 return null;
                             });
        }

        /// <summary>
        /// Inserts email addresses from a recipient list into another recipient list.
        /// </summary>
        /// <remarks>
        /// The status on new and existing items in the list is changed to the status parameter
        /// </remarks>
        /// <param name="recipientListFrom">The recipient list id to add addresses from.</param>
        /// <param name="recipientListIdTo">The recipient list id to add addresses to.</param>
        /// <returns>Returns the number of items that was inserted, which might 
        /// be less than the total count of items in the recipient list</returns>
        public int RecipientItemInsertFromRecipientList(int recipientListIdTo, int recipientListFrom)
        {
            int count = 0;
            Executor.Execute(() =>
                             {
                                 DbCommand  cmd = CreateCommand("NewsletterRecipientListInsertFromRecipientList");
                                 cmd.Parameters.Add(CreateParameter("recipientListFrom", recipientListFrom));
                                 cmd.Parameters.Add(CreateParameter("recipientListIdTo", recipientListIdTo));
                                 object objCount = cmd.ExecuteScalar();

                                 if (objCount == null)
                                 {
                                     throw new ApplicationException("Unable to read count of added recipient items.");
                                 }
                                 if (int.TryParse(objCount.ToString(), out count) == false)
                                 {
                                     throw new ApplicationException("Unable to cast count to integer. Unexpected return value: " + objCount.ToString());
                                 }
                             });

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recipientListId"></param>
        /// <param name="recipientListWashListId"></param>
        /// <returns></returns>
        public int RecipientListWashList(int recipientListId, int recipientListWashListId)
        {
            int count = 0;
            Executor.Execute(() =>
                                 {
                                     DbCommand  cmd = CreateCommand("NewsletterRecipientListWash");
                                     cmd.Parameters.Add(CreateParameter("recipientListId", recipientListId));
                                     cmd.Parameters.Add(CreateParameter("recipientListWashId", recipientListWashListId));
                                     object objCount = cmd.ExecuteScalar();

                                     if (objCount == null)
                                     {
                                         throw new ApplicationException("Unable to read count of deleted items from list.");
                                     }
                                     if (int.TryParse(objCount.ToString(), out count) == false)
                                     {
                                         throw new ApplicationException("Unable to cast count to integer. Unexpected return value: " + objCount.ToString());
                                     }
                                 });
            return count;
        }

    }
}
