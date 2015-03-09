using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer;
using EPiServer.DataAnnotations;

namespace $rootnamespace$.Models.Blocks
{
    /// <summary>
    /// Used to insert a link which is styled as a button with
    /// Newsletter compatible markup.
    /// </summary>
    [ContentType(
        DisplayName = "Newsletter Contact Details",
        GroupName = "Newsletter",
        AvailableInEditMode = false,
        GUID = "C848824C-FC75-4910-BE77-7788D5537011")]
    public class NewsletterContactDetailsBlock : BlockData
    {

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 10,
            Name = "Email Address")]
        [CultureSpecific(false)]
        public virtual string EmailAddress { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 20,
            Name = "Phone Number")]
        [CultureSpecific(false)]
        public virtual string PhoneNumber { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 30,
            Name = "Web Site")]
        [CultureSpecific(false)]
        public virtual Url WebSite { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 40,
            Name = "Facebook Link")]
        [CultureSpecific(false)]
        public virtual Url FacebookUrl { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 50,
            Name = "Twitter Link")]
        [CultureSpecific(false)]
        public virtual Url TwitterUrl { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 60,
            Name = "Google Plus Link")]
        [CultureSpecific(false)]
        public virtual Url GooglePlusUrl { get; set; }


    }
}
