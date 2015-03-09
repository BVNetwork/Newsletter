using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using BVNetwork.EPiSendMail.Contracts;
using EPiServer.Core;
using EPiServer.DataAbstraction;

namespace BVNetwork.EPiSendMail
{
	public class NewsletterBase : PageData, IRegisterNewsletterDetailsView
	{

		[Display(
			GroupName = SystemTabNames.Content,
            Name = "From Address",
            Description = "The email address that will show as the from field in most email clients",
			Order = 110)]
		public virtual string MailSender { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
            Name = "Email Subject",
            Description = "The subject line of the email. If empty, the name of the page wil be used",
			Order = 120)]
		public virtual string MailSubject { get; set; }
	}
}