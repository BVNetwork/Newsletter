using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BVNetwork.EPiSendMail.Api;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;
using EPiServer;
using EPiServer.Core;
using EPiServer.Shell.WebForms;

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