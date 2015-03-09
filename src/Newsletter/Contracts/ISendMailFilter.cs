
namespace BVNetwork.EPiSendMail.Contracts
{
	/// <summary>
	/// Interface for distribution filters.
	/// </summary>
	public interface ISendMailFilter
	{
		// Get information about the user preferences for receiving emails
		UserMailPreferences GetUserMailPreferences(System.Security.Principal.IPrincipal user);
	}
}
