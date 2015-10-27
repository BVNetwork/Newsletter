using BVNetwork.EPiSendMail.DataAccess;
using EPiServer.Web.Hosting;

namespace BVNetwork.EPiSendMail.Plugin
{
    public class RecipientListUiPageBase : PluginWebFormsBase, IRecipientListUi, IShowFeedback
    {
        private RecipientList _recipientList;
        private int _recipientListId = -1;
        private StatusMessage _messageControl;


        public StatusMessage MessageControl
        {
            get
            {
                return _messageControl;
            }
            set
            {
                _messageControl = value;
            }
        }

        public RecipientList RecipientList
        {
            get
            {
                if (_recipientList == null)
                {
                    if (RecipientListId > 0)
                        _recipientList = BVNetwork.EPiSendMail.DataAccess.RecipientList.Load(RecipientListId);
                }
                return _recipientList;
            }
        }

        public int RecipientListId
        {
            get
            {
                if (_recipientListId == -1)
                    _recipientListId = GetRecipientListIdFromQuery();

                return _recipientListId;
            }
        }

        protected int GetRecipientListIdFromQuery()
        {
            string id = Request.QueryString["id"];
            if (string.IsNullOrEmpty(id))
                id = Request.QueryString["recipientlistid"];

            if (string.IsNullOrEmpty(id))
                id = "0";

            return int.Parse(id);
        }

        /// <summary>
        /// Shows an error message on the page.
        /// </summary>
        /// <param name="message">Error message.</param>
        public void ShowError(string message)
        {
            MessageControl.ErrorMessage = message;
        }

        /// <summary>
        /// Shows an info message on the page.
        /// </summary>
        /// <param name="infoMessage">The info message.</param>
        public void ShowInfo(string message)
        {
            MessageControl.InfoMessage = message;
        }
    }
}
