using System;
using BVNetwork.EPiSendMail.Library;
using EPiServer.Core;

namespace BVNetwork.EPiSendMail.Templates
{
	/// <summary>
	/// Summary description for preview.
	/// </summary>
	public class preview : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
            // We cannot use "id" as the friendly url rewriter will
            // remove it for us
			int id = int.Parse(this.Request["pageid"]);
			PageReference pageRef = new PageReference(id);

			// Get content to send
			string html = new EPiMailEngine().GetPreviewHtml(pageRef);

			Response.Write(html);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
