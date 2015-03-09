namespace BVNetwork.EPiSendMail.DataAccess
{
    public enum RecipientListType
    {
        /// <summary>
        /// A private list, used for collecting email addresses.
        /// Not exposed to subscribers
        /// </summary>
        PrivateList = 0,
        /// <summary>
        /// Holds email addresses of people that do NOT want to
        /// recieve emails. Used to "wash" work lists before a
        /// newsletter is sent.
        /// </summary>
        BlockList,
        /// <summary>
        /// A public joinable list, used to show to users that
        /// want to subscribe to a newsletter. People can join
        /// and remove themselves from this kind of list.
        /// </summary>
        PublicList
    }
}
