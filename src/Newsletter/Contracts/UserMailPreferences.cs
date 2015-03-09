namespace BVNetwork.EPiSendMail.Contracts
{
	/// <summary>
	/// Mail Preferences for a user.
	/// </summary>
	public class UserMailPreferences
	{
		public bool SendAsHTML = false;
		public bool IsSubscribing = false;

		public UserMailPreferences()
		{
		
		}
	}
}
