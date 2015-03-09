using System;
using System.Web.UI.WebControls;
using BVNetwork.EPiSendMail.Library;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class EnvironmentVerification : JobUiUserControlBase
    {
        public EnvironmentVerificationItemCollection VerificationItems { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (NewsletterJob != null)
            {
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if(VerificationItems != null)
            {
                rptEnvVerification.DataSource = VerificationItems;
                rptEnvVerification.DataBind();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

    }
}