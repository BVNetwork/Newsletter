using System;
using System.ComponentModel.DataAnnotations;
using $rootnamespace$.Models.Blocks;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace $rootnamespace$.Models.Pages
{
    [ContentType(DisplayName = "Newsletter with Sidebar", 
        GUID = "685EB25B-54FE-42A4-8EBF-06B9E8FEC42C", 
        Description = "A Newsletter layout with a sidebar for related content")]
    [ImageUrl("/static/gfx/newsletter-page-thumbnail-blue.png")]
    public class NewsletterWithSidebarPage : NewsletterPage
    {
        [Display(
            Name = "SubPanel",
            Description = "Additional content in a panel below the main image",
            GroupName = SystemTabNames.Content,
            Order = 30)]
        public virtual XhtmlString SubPanel { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 41,
            Name = "Call to Action Button")]
        public virtual NewsletterButtonBlock CallToActionButton { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 50,
            Name = "Title above Link List")]
        public virtual string LinkListTitle { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 55,
            Name = "Links")]
        public virtual LinkItemCollection LinkList { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 60,
            Name = "Header Text",
            Description = "Text shown in the top right corner of the newsletter.")]
        public virtual string HeaderText { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 70,
            Name = "Contact Details")]
        public virtual NewsletterContactDetailsBlock ContactDetails { get; set; }

    }
}