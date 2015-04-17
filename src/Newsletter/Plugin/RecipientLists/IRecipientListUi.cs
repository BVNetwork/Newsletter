using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin
{
    public interface IShowFeedback
    {
        /// <summary>
        /// Shows an error message on the page.
        /// </summary>
        /// <param name="message">Error message.</param>
        void ShowError(string message);

        /// <summary>
        /// Shows an info message on the page.
        /// </summary>
        /// <param name="infoMessage">The info message.</param>
        void ShowInfo(string message);
    }

    public interface IRecipientListUi
    {
        /// <summary>
        /// Gets the recipient list.
        /// </summary>
        /// <value>The recipient list.</value>
        RecipientList RecipientList
        {
            get;
        }

        /// <summary>
        /// Gets the recipient list id.
        /// </summary>
        /// <value>The recipient list id.</value>
        int RecipientListId
        {
            get;
        }
    }
}
