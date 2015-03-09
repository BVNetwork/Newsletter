namespace BVNetwork.EPiSendMail.DataAccess
{
    public enum EmailAddressSource
    {
        /// <summary>
        /// The email has been imported
        /// </summary>
        Imported = 0,
        /// <summary>
        /// The user has registered and consented to share the email address
        /// </summary>
        SelfRegistered,
    }
}
