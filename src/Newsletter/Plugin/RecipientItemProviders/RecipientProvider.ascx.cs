using System;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin.RecipientItemProviders
{
    public partial class RecipientProvider : System.Web.UI.UserControl, IRecipientItemProvider
    {
        private RecipientList _list;
        private IRecipientListUi _listUi;


        protected void Page_Load(object sender, EventArgs e)
        {
            // Do not do databind here - the container will do this for us
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            rptAddFromRecipientLists.DataSource = RecipientLists.ListAll();
            rptAddFromRecipientLists.DataBind();
        }

        protected void cmdSelect_Click(object sender, CommandEventArgs e)
        {
            string recipListIdString = (string)e.CommandArgument;
            int recipListId = int.Parse(recipListIdString);
            RecipientList list = RecipientList.Load(recipListId);
            
            // Add the items
            int count = _list.AddRecipientItemsFromRecipientList(recipListId);
            _listUi.ShowInfo("Added " + count.ToString() + " Recipients from Recipient List " + list.Name);

        }

        #region IRecipientItemProvider Members

        public void Initialize(BVNetwork.EPiSendMail.DataAccess.RecipientList list, IRecipientListUi listUi)
        {
            _list = list;
            _listUi = listUi;
        }

        #endregion
    }
}