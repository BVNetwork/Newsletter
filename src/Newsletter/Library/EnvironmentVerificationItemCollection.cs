using System.Collections;

namespace BVNetwork.EPiSendMail.Library
{
	public class EnvironmentVerificationItemCollection: CollectionBase
	{
		// public methods...
		#region Add
		public EnvironmentVerificationItem Add(VerificationType verificationType, string message)
		{
			EnvironmentVerificationItem itm = new EnvironmentVerificationItem(verificationType, message);
			this.Add(itm);
			return itm;
		}

		public int Add(EnvironmentVerificationItem environmentVerificationItem)
		{
			return List.Add(environmentVerificationItem);
		}
		#endregion
		#region IndexOf
		public int IndexOf(EnvironmentVerificationItem environmentVerificationItem)
		{
			for(int i = 0; i < List.Count; i++)
				if (this[i] == environmentVerificationItem)    // Found it
					return i;
			return -1;
		}
		#endregion
		#region Insert
		public void Insert(int index, EnvironmentVerificationItem environmentVerificationItem)
		{
			List.Insert(index, environmentVerificationItem);
		}
		#endregion
		#region Remove
		public void Remove(EnvironmentVerificationItem environmentVerificationItem)
		{
			List.Remove(environmentVerificationItem);
		}
		#endregion
		#region Find
		// TODO: If desired, change parameters to Find method to search based on a property of EnvironmentVerificationItem.
		public EnvironmentVerificationItem Find(EnvironmentVerificationItem environmentVerificationItem)
		{
			foreach(EnvironmentVerificationItem lenvironmentVerificationItem in this)
				if (lenvironmentVerificationItem == environmentVerificationItem)    // Found it
					return lenvironmentVerificationItem;
			return null;    // Not found
		}
		#endregion
		#region Contains
		// TODO: If you changed the parameters to Find (above), change them here as well.
		public bool Contains(EnvironmentVerificationItem environmentVerificationItem)
		{
			return (Find(environmentVerificationItem) != null);
		}
		#endregion
 	
		// public properties...
		#region this[int aIndex]
		public EnvironmentVerificationItem this[int index] 
		{
			get
			{
				return (EnvironmentVerificationItem) List[index];
			}
			set
			{
				List[index] = value;
			}
		}
		#endregion
	}
	
}
