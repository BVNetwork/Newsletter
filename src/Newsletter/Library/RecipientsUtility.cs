using System.Web.Security;

namespace BVNetwork.EPiSendMail.Library
{
	/// <summary>
	/// Handles everything that has to do with retrieving the
	/// mail recipients, filtering etc.
	/// </summary>
	public class RecipientsUtility
	{
		public RecipientsUtility()
		{
		}

		/// <summary>
		/// Finds a user with a given email address. Searches both username
		/// and email fields of the user.
		/// </summary>
		/// <remarks>
		///	If the search returns more than one, it will pick the first in
		///	the list.
		/// </remarks>
		/// <param name="emailAddress">The email address to seach for</param>
		/// <returns>The found user as a UserSid or null if not found</returns>
        public MembershipUser FindUser(string emailAddress)
		{
			MembershipUserCollection foundUsers;
			// Search username LIKE emailAddress
			// foundUsers = UserSid.Search(SecurityIdentityType.AnyUserSid, false, emailAddress, null, null, null, null);
            foundUsers = Membership.FindUsersByEmail(emailAddress);
            
			// Return the first one
            if (foundUsers.Count > 0)
                return foundUsers[emailAddress];
			return null;
		}

		/// <summary>
		/// Get all groups in EPiServer.
		/// </summary>
		/// <returns></returns>
		public string[] GetLocalGroups()
		{
            return Roles.GetAllRoles();
		}

        public string[] GetNewsletterReceivers(string groupName)
		{
            return Roles.GetUsersInRole(groupName);
		}

	}
}
