using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace $rootnamespace$.Models.Pages
{
    [ContentType(DisplayName = "Newsletter Unsubscribe Page", 
        GUID = "c38e8df3-0286-472d-9f1a-c38772c375fa", Description = "A page to allow a user to unsubscribe from a newsletter")]
    [ImageUrl("/static/gfx/newsletter-page-thumbnail-blue.png")]
    public class NewsletterUnsubscribePage : SitePageData
    {
        [CultureSpecific]
        [Display(
            Name = "Main body",
            Description = "The main body of page",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        public virtual XhtmlString MainBody { get; set; }


        [Display(
            GroupName = SystemTabNames.Content,
            Order = 20,
            Name = "Block List")]
        [CultureSpecific(true)]
        public virtual int BlockListId { get; set; }

    }
}