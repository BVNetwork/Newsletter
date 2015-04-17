using System;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin
{
    public class RecipientListUiUserControlBase : EPiServer.UserControlBase, IRecipientListUi, IShowFeedback
    {
        public RecipientListUiPageBase RecipientListBase
        {
            get
            {
                return this.Page as RecipientListUiPageBase;
            }
        }

        public RecipientList RecipientList
        {
            get
            {
                RecipientListUiPageBase page = RecipientListBase;

                if (page == null)
                    throw new ApplicationException("This user control must be on a page that inherits RecipientListUiPageBase");
                return page.RecipientList;
            }
        }

        // public int JobId
        public int RecipientListId
        {
            get { return RecipientListBase.RecipientListId; }
        }

        public void ShowError(string message)
        {
            RecipientListBase.ShowError(message);
        }

        public void ShowInfo(string message)
        {
            RecipientListBase.ShowInfo(message);
        }
    }
}
