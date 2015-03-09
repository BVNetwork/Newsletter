using System.Collections;
using System.Collections.Generic;
using System.Data;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;

namespace BVNetwork.EPiSendMail.DataAccess
{
    public class RecipientLists : IEnumerable<RecipientList>
    {
        public List<RecipientList> Items { get; private set; }

        public RecipientLists()
        {
            Items = new List<RecipientList>();
        }

        public void Add(RecipientList item)
        {
            Items.Add(item);
        }

        public IEnumerator<RecipientList> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static RecipientData GetWorker()
        {
            return EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<RecipientData>();
        }

        ///<summary>
        /// Lists all recipient lists
        /// </summary>
        /// <returns>A RecipientLists collection of all recipient lists</returns>
        public static RecipientLists ListAll()
        {
            RecipientLists recipientLists = new RecipientLists();
            
            RecipientData dataUtil = GetWorker();
            DataTable recipTable = dataUtil.RecipientListGetAll();
            foreach (DataRow row in recipTable.Rows)
            {
                RecipientList recipientList = new RecipientList(row);
                recipientLists.Add(recipientList);
            }
            return recipientLists;
        }

        /// <summary>
        /// Lists all recipient lists that specified email adress belongs to
        /// </summary>
        /// <returns>A RecipientLists collection of all recipient lists</returns>
        public static RecipientLists ListAllByEmail(string email)
        {
            RecipientLists recipientLists = new RecipientLists();
            RecipientData dataUtil = GetWorker();
            DataTable recipTable = dataUtil.RecipientListGetAllByEmail(email);
            foreach (DataRow row in recipTable.Rows)
            {
                RecipientList recipientList = new RecipientList(row);
                recipientLists.Add(recipientList);
            }
            return recipientLists;
        }

        public static RecipientLists ListOneType(string listType)
        {
            RecipientLists recipientLists = new RecipientLists();
            RecipientData dataUtil = GetWorker();
            DataTable recipTable = dataUtil.RecipientListGetAll();
            foreach (DataRow row in recipTable.Rows)
            {
                RecipientList recipientList = new RecipientList(row);
                if (recipientList.ListType.ToString().Equals(listType))
                    recipientLists.Add(recipientList);
            }
            return recipientLists;
        }
    }

    //public class RecipientLists : CollectionBase
    //{
    //    // public methods...
    //    public int Add(RecipientList recipientList)
    //    {
    //        return List.Add(recipientList);
    //    }

    //    public int IndexOf(RecipientList recipientList)
    //    {
    //        for (int i = 0; i < List.Count; i++)
    //            if (this[i] == recipientList)    // Found it
    //                return i;
    //        return -1;
    //    }

    //    /// <summary>
    //    /// Removes a recipientList from this collection, but does not delete it from the
    //    /// database. This is an in memory operation only, no actions will be
    //    /// performed on the recipientList or its worker items in the database.
    //    /// </summary>
    //    public void Remove(RecipientList recipientList)
    //    {
    //        List.Remove(recipientList);
    //    }

    //    public void Remove(int recipientListId)
    //    {
    //        RecipientList recipientList = Find(recipientListId);
    //        if (recipientList != null)
    //        {
    //            List.Remove(recipientList);
    //        }
    //    }

    //    public RecipientList Find(RecipientList recipientList)
    //    {
    //        foreach (RecipientList recipientListItem in this)
    //            if (recipientListItem == recipientList)    // Found it
    //                return recipientListItem;
    //        return null;    // Not found
    //    }

    //    public RecipientList Find(int recipientListId)
    //    {
    //        foreach (RecipientList jobItem in this)
    //            if (jobItem.Id == recipientListId)    // Found it
    //                return jobItem;
    //        return null;    // Not found
    //    }

    //    public bool Contains(RecipientList recipientList)
    //    {
    //        return (Find(recipientList) != null);
    //    }

    //    public bool Contains(int recipientListId)
    //    {
    //        return (Find(recipientListId) != null);
    //    }

    //    /// <summary>
    //    /// Creates a new recipientList, and return the instanciated recipientList
    //    /// </summary>
    //    public static RecipientList Create(string name)
    //    {
    //        return Create(RecipientListType.PrivateList, name, null);
    //    }

    //    public static RecipientList Create(RecipientListType listType, string name, string description)
    //    {
    //        RecipientList recipientList = new RecipientList(listType, name, description);
    //        recipientList.Save();
    //        return recipientList;
    //    }

    //    /// <summary>
    //    /// Lists all recipient lists
    //    /// </summary>
    //    /// <returns>A RecipientLists collection of all recipient lists</returns>
    //    public static RecipientLists ListAll()
    //    {
    //        RecipientLists recipientLists = new RecipientLists();


    //        DataUtil.RecipientData dataUtil = new DataUtil.RecipientData();
    //        DataTable recipTable = dataUtil.RecipientListGetAll();
    //        foreach (DataRow row in recipTable.Rows)
    //        {
    //            RecipientList recipientList = new RecipientList(row);
    //            recipientLists.Add(recipientList);
    //        }
    //        return recipientLists;
    //    }

    //    /// <summary>
    //    /// Lists all recipient lists that specified email adress belongs to
    //    /// </summary>
    //    /// <returns>A RecipientLists collection of all recipient lists</returns>
    //    public static RecipientLists ListAllByEmail(string email)
    //    {
    //        RecipientLists recipientLists = new RecipientLists();
    //        DataUtil.RecipientData dataUtil = new DataUtil.RecipientData();
    //        DataTable recipTable = dataUtil.RecipientListGetAllByEmail(email);
    //        foreach (DataRow row in recipTable.Rows)
    //        {
    //            RecipientList recipientList = new RecipientList(row);
    //            recipientLists.Add(recipientList);
    //        }
    //        return recipientLists;
    //    }

    //    public static RecipientLists ListOneType(string listType)
    //    {
    //        RecipientLists recipientLists = new RecipientLists();
    //        DataUtil.RecipientData dataUtil = new DataUtil.RecipientData();
    //        DataTable recipTable = dataUtil.RecipientListGetAll();
    //        foreach (DataRow row in recipTable.Rows)
    //        {
    //            RecipientList recipientList = new RecipientList(row);
    //            if(recipientList.ListType.ToString().Equals(listType))
    //                recipientLists.Add(recipientList);
    //        }
    //        return recipientLists;
    //    }


    //    // public properties...
    //    #region this[int index]
    //    /// <summary>
    //    /// A collection of RecipientList instances
    //    /// </summary>
    //    public RecipientList this[int index]
    //    {
    //        get
    //        {
    //            return (RecipientList)List[index];
    //        }
    //        set
    //        {
    //            List[index] = value;
    //        }
    //    }
    //    #endregion
    //}

}
