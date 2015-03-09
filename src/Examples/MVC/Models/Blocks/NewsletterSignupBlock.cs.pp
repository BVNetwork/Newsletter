using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;

namespace $rootnamespace$.Models.Blocks
{
    [ContentType(DisplayName = "Newsletter Signup", 
        GUID = "e76d019a-7d9f-4f42-b106-d5f691dc1353", Description = "")]
    public class NewsletterSignupBlock : BlockData
    {

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 10,
            Name = "Title")]
        public virtual string Title { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 20,
            Name = "Lead")]
        public virtual string Lead { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 30,
            Name = "Button Text")]
        [UIHint(UIHint.Textarea)]
        public virtual string ButtonText { get; set; }


        [Display(
            GroupName = SystemTabNames.Content,
            Order = 40,
            Name = "Inline text for input field")]
        [UIHint(UIHint.Textarea)]
        public virtual string InlineText { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 50,
            Name = "Error Text",
            Description = "The text shown if an error occors, or the email is not valid.")]
        public virtual XhtmlString ErrorText { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 60,
            Name = "Success Text",
            Description = "The text shown after an email has been added to the public recipient list.")]
        public virtual XhtmlString SuccessText { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            ButtonText = "Subscribe Now!";
            Title = "Sign Up for our Newsletter";
            Lead = "Don't miss out on our latest news and offers.";
            InlineText = "Enter your email address";
            ErrorText = new XhtmlString("<strong>Oh no!</strong> We apologize sincerely, but your subcription could not be confirmed. Please check your email address. If you're unable to sign up, please <a href=\"/contactus\">contact us</a> and we'll get it sorted out.");
            SuccessText = new XhtmlString("<strong>Thank you!</strong> You'll soon receive a notification about the subscription. If you have not recieved anything from us in a couple of minutes, please check your spam folder.");
        }
    }
}