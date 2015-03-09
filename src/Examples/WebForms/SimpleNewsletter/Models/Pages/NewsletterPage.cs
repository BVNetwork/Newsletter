using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;

namespace BVNetwork.EPiSendMail.bvn.SendMail.Models.Pages
{
    [ContentType(
        DisplayName = "Newsletter",
       GUID = "52ab1483-cfcf-4140-abc8-244ff23bd437")]
    public partial class NewsletterPage : EPiServer.Core.PageData
    {

        [Display(
          GroupName = SystemTabNames.Content,
          Order = 110)]
        [Required]
        public virtual string MailSender { get; set; }

        [Display(
          GroupName = SystemTabNames.Content,
          Order = 120)]
        public virtual string MailSubject { get; set; }

        [Display(
          GroupName = SystemTabNames.Content,
          Order = 130)]
        public virtual string MailTitle { get; set; }
        
        [Display(
           GroupName = SystemTabNames.Content,
           Order = 140)]
        [Required]
        public virtual XhtmlString MainBody { get; set; }

        [Display(
          GroupName = SystemTabNames.Content,
          Order = 150)]
        [UIHint(UIHint.Textarea)]
        [Required]
        public virtual string MainBodyText { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 160)]
        public virtual ContentArea MainContentArea { get; set; }
    }
}