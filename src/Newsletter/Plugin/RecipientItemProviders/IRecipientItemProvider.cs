using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin.RecipientItemProviders
{
    public interface IRecipientItemProvider
    {
        /// <summary>
        /// Initializes the provider with a recipient list.
        /// </summary>
        void Initialize(RecipientList list, IRecipientListUi recipientListUi);

    }
}
