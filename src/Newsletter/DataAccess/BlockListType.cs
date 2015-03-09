namespace BVNetwork.EPiSendMail.DataAccess
{
    public enum BlockListType
    {
        /// <summary>
        /// Denotes an optional block list. Users can choose to use these lists as block lists when sending emails
        /// </summary>
        Optional,
        /// <summary>
        /// Denotes a mandatory list. All mandatory lists will be used as filters when sending newsletters
        /// </summary>
        Mandatory,
    }
}
