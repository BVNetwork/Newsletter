using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Library;
using SendGrid;

// ReSharper disable PossibleMultipleEnumeration

namespace BVNetwork.EPiSendMail.SendGrid
{
    public class MailSenderSendGrid : MailSenderBatchBase, IMailSenderVerification
    {
        private const string ConnectionStringName = "EPiCode.Newsletter.SendGrid";


        public class SendGridSettings
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// Sends the mail batch using the SendGrid API
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <param name="recipients">The recipients.</param>
        /// <param name="onlyTestDontSendMail">if set to <c>true</c> [only test dont send mail].</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool SendMailBatch(MailInformation mail, IEnumerable<JobWorkItem> recipients, bool onlyTestDontSendMail)
        {
            var settings = GetSettings();

            if (recipients == null || recipients.Any() == false)
                throw new ArgumentException("No workitems", "recipients");

            if (recipients.Count() > 1000)
                throw new ArgumentOutOfRangeException("recipients", "SendGrid supports maximum 1000 recipients per batch send.");

            var msg = new SendGridMessage();
            msg.From = new MailAddress(mail.From);
            msg.Subject = mail.Subject;
            msg.Html = mail.BodyHtml;
            msg.Text = mail.BodyText;
            
            // Add recipinets to header, to hide other recipients in to field. 
            List<string> addresses = recipients.Select(r => r.EmailAddress).ToList();
            msg.Header.SetTo(addresses);
            msg.AddSubstitution("%recipient%", addresses);
            // To send message we need to have a to address, set that to from
            msg.To = new MailAddress[] { msg.From };

            if (mail.EnableTracking)
            {
                // true indicates that links in plain text portions of the email 
                // should also be overwritten for link tracking purposes. 
                msg.EnableClickTracking(true);
                msg.EnableOpenTracking();
            }

            if(mail.CustomProperties.ContainsKey("SendGridCategory"))
            {
                string category = mail.CustomProperties["SendGridCategory"] as string;
                if (string.IsNullOrEmpty(category) == false)
                    msg.SetCategory(category);
            }

            var credentials = new NetworkCredential(settings.Username, settings.Password);

            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);

            transportWeb.Deliver(msg);

            return true;
        }

        public EnvironmentVerification VerifyEnvironment()
        {
            EnvironmentVerification env = new EnvironmentVerification();

            SendGridSettings settings = GetSettings();
            if (string.IsNullOrEmpty(settings.Username))
            {
                env.VerificationItems.Add(VerificationType.Error, "Missing username setting in SendGrid connection string.");
            }

            if (string.IsNullOrEmpty(settings.Password))
            {
                env.VerificationItems.Add(VerificationType.Error, "Missing password setting in SendGrid connection string.");
            }

            return env;
        }

        protected SendGridSettings GetSettings()
        {
            SendGridSettings settings = new SendGridSettings();

            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if(connectionString == null)
                return settings;
            
            var keyValuePairs = connectionString.ConnectionString.Split(new[]{';'}, StringSplitOptions.RemoveEmptyEntries)
                                        .Where(kvp => kvp.Contains('='))
                                        .Select(kvp => kvp.Split(new char[] { '=' }, 2))
                                        .ToDictionary(kvp => kvp[0].Trim(),
                                                      kvp => kvp[1].Trim(),
                                                      StringComparer.InvariantCultureIgnoreCase);
 
            
            
            // Mandatory
            if (keyValuePairs.ContainsKey("username"))
                settings.Username = keyValuePairs["username"];
            if (keyValuePairs.ContainsKey("password"))
                settings.Password = keyValuePairs["password"];
            
            // Optional

            return settings;
        }

        public override int GetBatchSize()
        {
            // SendGrid lets us send 1000 each time. We'll default to 999
            return 999;
        }


    }
}
