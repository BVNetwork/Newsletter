using System;

namespace BVNetwork.EPiSendMail.Plugin
{
    public partial class StatusMessage : System.Web.UI.UserControl
    {
        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        private string _infoMessage;
        public string InfoMessage
        {
            get
            {
                return _infoMessage;
            }
            set
            {
                _infoMessage = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            bool show = false;
            pnlErrorMessage.Visible = false;
            pnlInfoMessage.Visible = false;

            if (string.IsNullOrEmpty(ErrorMessage) == false)
            {
                lblErrorMessage.Text = ErrorMessage;
                pnlErrorMessage.Visible = true;
                show = true;
            }
            if (string.IsNullOrEmpty(InfoMessage) == false)
            {
                lblInfoMessage.Text = InfoMessage;
                pnlInfoMessage.Visible = true;
                show = true;
            }

            pnlStatusContainer.Visible = show;
                
        }
    }
}