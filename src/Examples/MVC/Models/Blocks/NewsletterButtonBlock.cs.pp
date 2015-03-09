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
        DisplayName = "Newsletter Call to Action Button",
        GroupName = "Newsletter",
        AvailableInEditMode = false,
        GUID = "0DD40A87-D5A4-47D1-ADB1-381175A7C0E7")]
    public class NewsletterButtonBlock : BlockData
    {
        [Display(Order = 1, GroupName = SystemTabNames.Content)]
        public virtual string ButtonText { get; set; }

        [Display(Order = 2, GroupName = SystemTabNames.Content)]
        public virtual Url ButtonLink { get; set; }
    }
}
