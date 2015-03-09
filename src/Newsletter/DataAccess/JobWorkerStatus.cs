namespace BVNetwork.EPiSendMail.DataAccess
{
    public enum JobWorkStatus
    {
        /// <summary>
        /// The worker item has not been handled yet
        /// </summary>
        NotStarted,
        /// <summary>
        /// The work item has been collected for sending
        /// </summary>
        Sending,
        /// <summary>
        /// The worker item has failed to process, see the note for more information
        /// </summary>
        Failed,
        /// <summary>
        /// The worker item has succeeded, see note for any additional information
        /// </summary>
        Complete,
        
    }
}
