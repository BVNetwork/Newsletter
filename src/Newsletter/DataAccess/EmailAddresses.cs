using System.Collections;
using System.Data;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;

namespace BVNetwork.EPiSendMail.DataAccess
{
    public class EmailAddresses : CollectionBase
    {
        private static RecipientData GetWorker()
        {
            return EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<RecipientData>();
        }
        // public methods...
        public int Add(EmailAddress emailAddress)
        {
            return List.Add(emailAddress);
        }
        
        public int IndexOf(EmailAddress emailAddress)
        {
            for (int i = 0; i < List.Count; i++)
                if (this[i] == emailAddress)    // Found it
                    return i;
            return -1;
        }
        
        
        public void Insert(int index, EmailAddress emailAddress)
        {
            List.Insert(index, emailAddress);
        }
        
        
        public void Remove(EmailAddress emailAddress)
        {
            List.Remove(emailAddress);
        }

        public EmailAddress Find(string emailAddress)
        {
            foreach (EmailAddress emailAddressItem in this)
                if (emailAddressItem.Email == emailAddress)    // Found it
                    return emailAddressItem;
            return null;    // Not found
        }
        
        public EmailAddress Find(EmailAddress emailAddress)
        {
            foreach (EmailAddress emailAddressItem in this)
                if (emailAddressItem == emailAddress)    // Found it
                    return emailAddressItem;
            return null;    // Not found
        }
        
        
        // TODO: If you changed the parameters to Find (above), change them here as well.
        public bool Contains(EmailAddress emailAddress)
        {
            return (Find(emailAddress) != null);
        }
        

        public static EmailAddresses ListAll(int recipientListId)
        {
            EmailAddresses items = new EmailAddresses();

            RecipientData dataUtil = GetWorker();
            DataTable emailAddressTable = dataUtil.RecipientListGetAllItems(recipientListId);
            FillFromDataTable(emailAddressTable, items);
            return items;
        }

        private static void FillFromDataTable(DataTable table, EmailAddresses emailAddresses)
        {
            foreach (DataRow row in table.Rows)
            {
                EmailAddress emailAddress = new EmailAddress(row);
                emailAddresses.Add(emailAddress);
            }
        }


        // public properties...
        #region this[int index]
        public EmailAddress this[int index]
        {
            get
            {
                return (EmailAddress)List[index];
            }
            set
            {
                List[index] = value;
            }
        }
        #endregion

    }
    
}
