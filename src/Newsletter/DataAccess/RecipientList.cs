using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;
using Newtonsoft.Json;

namespace BVNetwork.EPiSendMail.DataAccess
{
    public class RecipientList : IEmailImporter
    {
        private RecipientListType _type = RecipientListType.PrivateList;
        private int _id = 0;
        private string _name;
        private string _description;
        private EmailAddresses _emailAddresses = null;
        private DateTime _created;
        private int _emailAddressCount = -1;

        private static RecipientData GetWorker()
        {
            return EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<RecipientData>();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RecipientList()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipientList"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="listType">Type of the list.</param>
        /// <param name="created">The created.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        internal RecipientList(int id, RecipientListType listType, DateTime created, string name, string description)
        {
            _id = id;
            _type = listType;
            _created = created;
            _name = name;
            _description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipientList"/> class.
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        internal RecipientList(DataRow dataRow)
        {
            _id = (int)dataRow["pkRecipientListId"];
            _name = dataRow["Name"] != null ? dataRow["Name"].ToString() : null;
            _description = dataRow["Description"] != null ? dataRow["Description"].ToString() : null;
            _created = dataRow["Created"] != null ? (DateTime)dataRow["Created"] : DateTime.MinValue;
            _type = dataRow["ListType"] != null ? (RecipientListType)dataRow["ListType"] : RecipientListType.PrivateList;
            _emailAddressCount = (dataRow["Count"] != null && dataRow["Count"] != DBNull.Value) ? (int)dataRow["Count"] : -1;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RecipientList"/> class.
        /// </summary>
        /// <param name="listType">Type of the list.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public RecipientList(RecipientListType listType, string name, string description)
        {
            _type = listType;
            _name = name;
            if (string.IsNullOrEmpty(description))
                _description = null;
            else
                _description = description;
        }
        
        
        /// <summary>
        /// The unique id of this recipient list
        /// </summary>
        public int Id
        {
            get
            {
                return _id;
            }
        }


        /// <summary>
        /// Gets the count of email addresses in the database
        /// </summary>
        /// <remarks>
        /// This is a statistical value only, and will not be updated after the
        /// list has been fetched from the database. If you add email addresses
        /// to the list, this value will not be updated.
        /// </remarks>
        /// <value>The count of email addresses for this recipient list.</value>
        public int EmailAddressCount
        {
            get
            {
                if (_emailAddressCount == -1 && _id > 0)
                {
                    RecipientData util = GetWorker();
                    DataTable addresses = util.RecipientListGetAllItems(_id);
                    _emailAddressCount = addresses.Rows.Count;
                }

                return _emailAddressCount;
            }
        }

        public DateTime Created
        {
            get
            {
                return _created;
            }
            set
            {
                _created = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the recipient list.
        /// </summary>
        /// <value>The type of the list.</value>
        public RecipientListType ListType
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }


        /// <summary>
        /// Name of the newsletter, identifying the job
        /// </summary>
        /// <value>A name to describe the job in a list of jobs</value>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// A short description of this newsletter
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Gets the email addresses for this recipient list.
        /// </summary>
        /// <remarks>
        /// When accessed for the first time, the list will be
        /// filled with all the email addresses for this list. 
        /// This can be time consuming if there are many items
        /// in the database.
        /// </remarks>
        /// <value>The email addresses.</value>
        [JsonIgnore]
        public EmailAddresses EmailAddresses
        {
            get
            {

                if (_emailAddresses == null)
                {
                    // Get worker items for this job
                    if (_id <= 0)
                        throw new ApplicationException("Cannot retrieve email address items for a recipient list that has no id. Save the list first to get the id.");

                    // Get items
                    _emailAddresses = EmailAddresses.ListAll(_id);
                }
                return _emailAddresses;
            }
        }


        /// <summary>
        /// Deletes this recipient list and all email address items related to it.
        /// </summary>
        /// <remarks>
        /// Using this class and any members after its deletion is risky, as any
        /// data it might be referencing will not be available.
        /// The EmailAddress property will be cleared when calling Delete.
        /// </remarks>
        public void Delete()
        {
            RecipientData dataUtil = GetWorker();
            dataUtil.RecipientListDelete(_id);

            // Reset collection
            _emailAddresses = null;
            ClearEmailAddressCount();
        }


        

        /// <summary>
        /// Deletes the email address items for this recipient list
        /// </summary>
        /// <remarks>
        /// The EmailAddress property will be cleared when calling this method.
        /// </remarks>
        public void DeleteEmailAddressItems()
        {
            RecipientData dataUtil = GetWorker();
            dataUtil.RecipientListRemoveAllItems(_id);

            // Reset collection
            _emailAddresses = null;
            ClearEmailAddressCount();
        }

        public int ImportEmailAddresses(string emailAddressesDelimited, out List<string> invalidEmailAddresses, out List<string> duplicateAddresses)
        {
            string[] emailAddressArray = emailAddressesDelimited.Split(new char[] { ';', ',', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Do Import
            return ImportEmailAddresses(emailAddressArray, out invalidEmailAddresses, out duplicateAddresses);
        }

        public int RemoveEmailAddresses(string emailAddressesDelimited)
        {
            string[] emailAddressArray = emailAddressesDelimited.Split(new char[] { ';', ',', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Do Remove
            return RemoveEmailAddresses(emailAddressArray);
        }

        /// <summary>
        /// Removes emailaddresses from the recipient list. 
        /// Validates the input email address to prevent sql statements in the input.
        /// </summary>
        /// <param name="emailAddressArray">Array of emailadrresses</param>
        /// <returns></returns>
        private int RemoveEmailAddresses(string[] emailAddressArray)
        {
            int numberOfDeletedItems = 0;

            RecipientData recipientUtil = GetWorker();
            foreach (string emailAddress in emailAddressArray)
            {
                string emailAddressCleaned = NewsLetterUtil.CleanEmailAddress(emailAddress);

                if (EmailSyntaxValidator.Validate(emailAddressCleaned))
                {
                    recipientUtil.RecipientListRemoveItem(_id, emailAddressCleaned);
                    numberOfDeletedItems++;
                }
            }
            ClearEmailAddressCount();
            return numberOfDeletedItems;
        }

        /// <summary>
        /// Imports email addresses into a Recipient List
        /// </summary>
        /// <param name="emailArray">The email addresses to import.</param>
        /// <returns>The number of email addresses imported as new work items. Duplicates are not part of this number.</returns>
        public int ImportEmailAddresses(string[] emailArray)
        {
            List<string> invalidEmailAddresses;
            List<string> duplicateAddresses;
            return ImportEmailAddresses(emailArray, out invalidEmailAddresses, out duplicateAddresses);
        }

       
        /// <summary>
        /// Imports email addresses into a Recipient List
        /// </summary>
        /// <param name="emailArray">The email addresses to import.</param>
        /// <param name="invalidEmailAddresses">A list of all available email addresses that could not be parsed as a valid address</param>
        /// <returns>The number of email addresses imported as new work items. Duplicates are not part of this number.</returns>
        public int ImportEmailAddresses(string[] emailArray, out List<string> invalidEmailAddresses, out List<string> duplicateAddresses)
        {
            EmailAddresses importedItems = new EmailAddresses();
            invalidEmailAddresses = new List<string>();
            duplicateAddresses = new List<string>();

            int numberOfNewItems = 0;
            foreach (string emailAddress in emailArray)
            {
                // TODO: This can be optimized by checking the
                // existance of these email addresses in batches

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
                    EmailAddress emailItem = importedItems.Find(emailAddressCleaned);
                    if (emailItem == null)
                    {
                        // Handle duplicates - try to load it first
                        emailItem = EmailAddress.Load(Id, emailAddressCleaned);
                        if (emailItem == null)
                        {
                            
                            // Create it, and save it. It is automatically
                            // added to the WorkItems collection
                            emailItem = this.CreateEmailAddress(emailAddressCleaned);
                            // Save
                            emailItem.Save();
                            numberOfNewItems++;
                        }
                        else
                        {
                            // Duplicate
                            duplicateAddresses.Add(emailAddressCleaned);
                        }

                        // Add to imported collection, for quick
                        // in memory duplicate check
                        importedItems.Add(emailItem);
                    }
                }
            }

            ClearEmailAddressCount();

            return numberOfNewItems;
        }

        internal void ClearEmailAddressCount()
        {
            _emailAddressCount = -1;
        }

        /// <summary>
        /// Adds email addresses from another recipient list into this list.
        /// </summary>
        /// <remarks>
        /// Only new email addresses will be added. Email addresses that exists
        /// in this list already will not be affected.
        /// </remarks>
        /// <param name="sourceRecipientListId">The id of the recipient list where email addresses will be fetched from.</param>
        /// <returns>The number of email addresses imported into this list</returns>
        public int AddRecipientItemsFromRecipientList(int sourceRecipientListId)
        {
            RecipientData dataUtil = GetWorker();
            int count = dataUtil.RecipientItemInsertFromRecipientList(_id, sourceRecipientListId);
            ClearEmailAddressCount();
            return count;
        }

        /// <summary>
        /// Creates a new recipient email item with a given email address, 
        /// connected to this list, but does not add it to the EmailAddresses collection.
        /// The item is not saved, it has to be explicitly saved.
        /// </summary>
        /// <remarks>
        /// There is no check for a duplicate email address for this
        /// list when creating the work item.
        /// </remarks>
        /// <returns>
        /// A new email address item with an email address connected to this list
        /// </returns>
        public EmailAddress CreateEmailAddress(string email)
        {
            if (_id <= 0)
                throw new ApplicationException("Cannot create email address object for a recipient list that has no id. Save the list first to get the id.");

            EmailAddress emailAddress = new EmailAddress(_id, email);
            return emailAddress;
        }

        /// <summary>
        /// Creates a new work item, connected to this job, and adds it to
        /// the WorkItems collection.
        /// The work item is not saved, it has to be explicitly saved.
        /// </summary>
        /// <returns>
        /// A new work item object connected to this job
        /// </returns>
        public EmailAddress CreateEmailAddress()
        {
            return CreateEmailAddress(null);
        }

        /// <summary>
        /// Saves the job
        /// </summary>
        public virtual void Save()
        {
            // verify paramterers before sending them to the database
            if (string.IsNullOrEmpty(Name))
                throw new ArgumentException("Name cannot be null or empty");

            if (Name.Length > 255)
                throw new ArgumentException("Name cannot be more then 255 characters");

            if (string.IsNullOrEmpty(Description) == false)
                if (Description.Length > 2000)
                    throw new ArgumentException("Description cannot be more then 2000 characters");

            RecipientData dataUtil = GetWorker();

            if (Id == 0)
            {
                // New list
                // Verify that we do not have a list with same name
                RecipientList existing = Load(_name);
                if(existing != null)
                {
                    throw new ArgumentException("A Recipient List with the same name exists.");
                }

                int newId = dataUtil.RecipientListCreate(_type, _name, _description);
                _id = newId;
            }
            else
            {
                // Edit existing
                dataUtil.RecipientListEdit(_id, _type, _name, _description);
            }
        }

        /// <summary>
        /// Loads the specified list.
        /// </summary>
        /// <param name="recipientListId">The id of the recipient list.</param>
        /// <returns>The job if found, null if no job with the id could be found</returns>
        public static RecipientList Load(int recipientListId)
        {
            RecipientData dataUtil = GetWorker();
            DataTable recipTable = dataUtil.RecipientListGetById(recipientListId);

            // See if we found one
            if (recipTable.Rows.Count != 1)
            {
                return null;
            }

            // Found one
            RecipientList recipList = new RecipientList(recipTable.Rows[0]);
            return recipList;
        }

        /// <summary>
        /// Loads the specified list.
        /// </summary>
        /// <param name="name">The name of the list</param>
        /// <returns>The list if found, null if no job with the name could be found</returns>
        public static RecipientList Load(string name)
        {
            RecipientLists allLists = RecipientLists.ListAll();
            RecipientList list = allLists.FirstOrDefault(l => l.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return list;
        }



        internal static void FillFromDataTable(DataTable table, EmailAddresses items)
        {
            foreach (DataRow row in table.Rows)
            {
                //RecipientList recipientList = new RecipientList(row);
                //recList.CreateEmailAddress(row["EmailAddress"].ToString());
                EmailAddress email = new EmailAddress(row);
                items.Add(email);
            }
        }

        /// <summary>
        /// Searches the recipientList for emailaddresses that matches
        /// a specified string. Will search all emails with a LIKE clause.
        /// </summary>
        /// <param name="recipientListId">The recipient list id.</param>
        /// <param name="searchFor">The email to search for.</param>
        /// <returns>A collection of recipient. The collection count can be 0</returns>
        public static EmailAddresses Search(int recipientListId, string searchFor)
        {
            //RecipientList items = new RecipientList();
            EmailAddresses items = new EmailAddresses();
            RecipientData dataUtil = GetWorker();

            DataTable recipientsTable = dataUtil.RecipientListSearch(recipientListId, searchFor);
            FillFromDataTable(recipientsTable, items);
            return items;
        }

        public override string ToString()
        {
            return string.Format("ID:{0} \nName: {1} \nType: {2} \nCreated: {3} \nDescription: {4}",
                                 _id.ToString(),
                                 _name ?? "(null)", 
                                 _type.ToString(),
                                 _created.ToString(),
                                 _description ?? "(null)");
        }

    }
}
