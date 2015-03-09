using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin
{
    public interface IJobUi
    {
        /// <summary>
        /// Gets the newsletter job.
        /// </summary>
        /// <value>The newsletter job.</value>
        Job NewsletterJob
        {
            get;
        }

        /// <summary>
        /// Gets the job id.
        /// </summary>
        /// <value>The job id.</value>
        int JobId
        {
            get;
        }

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
}
