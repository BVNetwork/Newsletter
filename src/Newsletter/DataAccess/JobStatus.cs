namespace BVNetwork.EPiSendMail.DataAccess
{
    public enum JobStatus
    {
        /// <summary>
        /// The job is beeing edited
        /// </summary>
        Editing,
        /// <summary>
        /// The job has been started, and it is ready for or in the process of sending emails
        /// </summary>
        Sending,
        /// <summary>
        /// The job has finished
        /// </summary>
        Closed,
    }
}
