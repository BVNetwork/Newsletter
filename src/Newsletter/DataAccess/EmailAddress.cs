using System;
using System.Data;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;

namespace BVNetwork.EPiSendMail.DataAccess
{
    public class EmailAddress
    {
        private static RecipientData GetWorker()
        {
            return EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<RecipientData>();
        }
        
        private int _recipientListId;
        private string _emailAddress;
        private string _comment;
        private DateTime _added = DateTime.MinValue;
        private EmailAddressSource _source = EmailAddressSource.Imported;

        internal EmailAddress()
        {
        }

        public EmailAddress(int recipientListId, string emailAddress)
        {
            _recipientListId = recipientListId;
            _emailAddress = emailAddress;
        }

        public EmailAddress(DataRow dataRow)
        {
            _recipientListId = (int)dataRow["fkRecipientListId"];
            _emailAddress = dataRow["EmailAddress"] != null ? dataRow["EmailAddress"].ToString() : null;
            _comment = dataRow["Comment"] != null ? dataRow["Comment"].ToString() : null;
            _added = dataRow["Added"] != null ? (DateTime)dataRow["Added"] : DateTime.MinValue;
            _source = dataRow["Source"] != null ? (EmailAddressSource)dataRow["Source"] : EmailAddressSource.Imported;
        }

        public string Email
        {
            get
            {
                return _emailAddress;
            }
            set
            {
                _emailAddress = value;
            }
        }

        /// <summary>
        /// The date and time the email address was added
        /// </summary>
        public DateTime Added
        {
            get
            {
                return _added;
            }
            set
            {
                _added = value;
            }
        }

        /// <summary>
        /// The source of this email address.
        /// </summary>
        public EmailAddressSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }

        /// <summary>
        /// Saves this email address.
        /// </summary>
        /// <remarks>
        public virtual void Save()
        {
            if (_recipientListId <= 0)
                throw new ArgumentException("The recipient list id must be a postitve integer");

            // verify paramterers before sending them to the database
            if (string.IsNullOrEmpty(Email))
                throw new ArgumentException("Email cannot be null or empty");

            if (Email.Length > 150)
                throw new ArgumentException("Email cannot be more then 150 characters");

            if (string.IsNullOrEmpty(Comment) == false)
                if (Comment.Length > 2000)
                    throw new ArgumentException("Comment cannot be more then 2000 characters");

            DataUtil.RecipientData dataUtil = GetWorker();
            dataUtil.RecipientListEditItem(_recipientListId, Email, Comment, Source);
        }

        public static EmailAddress Load(int recipientListId, string emailAddress)
        {
            RecipientData dataUtil = GetWorker();
            
            // Get it, making sure we pass a washed address
            DataRow emailRow = dataUtil.RecipientListGetItem(recipientListId, NewsLetterUtil.CleanEmailAddress(emailAddress));
            if (emailRow != null)
                return new EmailAddress(emailRow);
            else
                return null;
        }

        /// <summary>
        /// Deletes this email address.
        /// </summary>
        public void Delete()
        {
            if (_recipientListId == 0)
                throw new NullReferenceException("This email address has no recipient list association, and cannot be deleted.");
            if (_emailAddress == null)
                throw new NullReferenceException("Cannot delete with empty email address.");
            EmailAddress.Delete(_recipientListId, _emailAddress);

        }

        /// <summary>
        /// Deletes the specified email address from a specified recipient list.
        /// </summary>
        /// <param name="recipientId">The id of the recipient list.</param>
        /// <param name="emailAddress">The email address.</param>
        public static void Delete(int recipientId, string emailAddress)
        {
            RecipientData dataUtil = GetWorker();
            dataUtil.RecipientListRemoveItem(recipientId, emailAddress);
        }

    }
}
