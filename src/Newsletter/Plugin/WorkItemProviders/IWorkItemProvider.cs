using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin.WorkItemProviders
{
    public interface IWorkItemProvider
    {
        /// <summary>
        /// Initializes the provider with a job.
        /// </summary>
        /// <param name="job">The job.</param>
        void Initialize(Job job, IJobUi jobUi);

    }
}
