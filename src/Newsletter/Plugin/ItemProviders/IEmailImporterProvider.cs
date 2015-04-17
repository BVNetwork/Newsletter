using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin.ItemProviders
{
    public interface IEmailImporterProvider
    {
        /// <summary>
        /// Initializes the provider.
        /// </summary>
        void Initialize(IEmailImporter importer, IShowFeedback feedbackUi);

    }
}
