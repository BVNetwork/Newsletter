using System;
using System.Web.UI;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class JobStatus : JobUiUserControlBase
    {
        private int _updateInterval = 10; // 10 seconds interval

        public int UpdateInterval
        {
            get
            {
                return _updateInterval;
            }
            set
            {
                _updateInterval = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude(Page.GetType(), 
                                    "jsRender", 
                                    NewsLetterConfiguration.GetModuleBaseDir() + "/content/js/jsrender.min.js");
        }

    }
}