using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BVNetwork.EPiSendMail;
using BVNetwork.EPiSendMail.Contracts;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;
using EPiServer.Web;

namespace $rootnamespace$.Models.Pages
{
    [ContentType(DisplayName = "Newsletter", GUID = "c783f548-e63a-4fb5-b1b1-a6fb8d672fa2", Description = "")]
    [ImageUrl("/static/gfx/newsletter-page-thumbnail-blue.png")]
    public class NewsletterPage : NewsletterBase, IPopulateCustomProperties
    {
        [Display(
            Name = "Main body",
            Description = "The main body of the newsletter",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        public virtual XhtmlString MainBody { get; set; }

        [Display(
            Name = "Lead",
            Description = "A subtitle shown below the main title",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        public virtual string Lead { get; set; }

        [Display(
            Name = "Logo",
            Description = "The logo to show in the top of the newsletter",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference Logo { get; set; }

        [Display(
            Name = "Mailgun Campaign",
            Description = "The name of a campaign in Mailgun. Leave empty if you do not use Mailgun campaigns",
            GroupName = SystemTabNames.Content,
            Order = 30)]
        public virtual string MailgunCampaignName { get; set; }

        [Display(
            Name = "Mailgun Tag",
            Description = "The name of a tag in Mailgun. Leave empty if you do not use Mailgun tags. Note! Number of tags are limited to 200 in Mailgun.",
            GroupName = SystemTabNames.Content,
            Order = 40)]
        public virtual string MailgunTagName { get; set; }

        public void AddCustomProperties(Dictionary<string, object> properties)
        {
            if (string.IsNullOrEmpty(MailgunCampaignName) == false)
                properties.Add("o:campaign", MailgunCampaignName);

            if (string.IsNullOrEmpty(MailgunTagName) == false)
                properties.Add("o:tag", MailgunTagName);
        }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            // MailSender = "your@email.here";
        }

    }
}