using System;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using EPiServer.Core;

namespace BVNetwork.EPiSendMail.Plugin
{
	public partial class CreateJob : JobUiUserControlBase
	{
		private bool _doRedirect;

		public bool DoRedirect
		{
			get
			{
				return _doRedirect;
			}
			set
			{
				_doRedirect = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (CurrentPage.ContentLink != ContentReference.EmptyReference)
			{
				txtNewNewsletterName.Text = CurrentPage.PageName;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			cmdSaveNewNewsletter.Click += new EventHandler(SaveNewNewsletter_ClickHandler);
		}

		void SaveNewNewsletter_ClickHandler(object sender, EventArgs e)
		{
			string name = txtNewNewsletterName.Text;
			string desc = txtNewNewsletterDesc.Text;

			if (string.IsNullOrEmpty(name))
				ShowError("Name cannot be empty");

			// Create and Save
			Job newJob = new Job(0, name, desc);
			newJob.PageId = CurrentPage.ContentLink.ID;
			newJob.Save();

			//Redirect to the job page
			if (newJob.Id != 0)
				/// TODO: This surely cannot work with EPiServer 7.x
				Response.Redirect(NewsLetterConfiguration.GetEditPanelLink(newJob));
			else
				ShowError("Something went wrong saving new job");
		}
	}
}