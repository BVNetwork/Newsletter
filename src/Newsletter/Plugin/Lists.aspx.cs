using System;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class Lists : PluginWebFormsBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            RecipientLists recipLists = RecipientLists.ListAll();

            rptRecipientLists.DataSource = recipLists;
            rptRecipientLists.DataBind();

            // Links needs to be set here
            A3.HRef = BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir() +
                      "/plugin/recipientlists/newrecipientlist.aspx";

        }

        public string GetRecipientListTypeString(object listType)
        {
            return GetRecipientListTypeString((RecipientListType)listType);
        }

        public string GetRecipientListTypeString(RecipientListType listType)
        {
            return NewsLetterUtil.GetEnumLanguageName(listType);
        }

    }
}