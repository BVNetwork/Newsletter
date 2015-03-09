using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BVNetwork.EPiSendMail.DataAccess;
using Newtonsoft.Json.Linq;

namespace BVNetwork.EPiSendMail.Api
{
    public class SubscriptionApi 
    {
        private static log4net.ILog _log = log4net.LogManager.GetLogger(typeof(SubscriptionApi));

        /// <summary>
        /// Adds an email address to the one public recipient list, or the one named 
        /// "Default" if there are more than one.
        /// </summary>
        /// <remarks>
        /// We do not care if the address already exists, due to
        /// security conserns, we won't tell if it do exists, just add it
        /// </remarks>
        /// <param name="email">The email address to add. It will be validated.</param>
        /// <returns>A success or fail message.</returns>
        public SubscriptionResult Subscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
                return SubscriptionResult.EmailNotValid;
                 // throw new ArgumentNullException("email", "Email address cannot be empty.");

            // Note! This may throw an error, and we want to, as it needs to
            // be handled by the application
            RecipientList selectedList = GetPublicList();

            return AddSubscriptionToList(email, selectedList);

        }

        /// <summary>
        /// Adds an email address to the specificed recipient list
        /// </summary>
        /// <remarks>
        /// We do not care if the address already exists, due to
        /// security conserns, we won't tell if it do exists, just add it
        /// </remarks>
        /// <param name="email">The email address to add. It will be validated.</param>
        /// <param name="recipientList">The ID of the recipient list</param>
        /// <returns>A success or fail message.</returns>
        public SubscriptionResult Subscribe(string email, int recipientList)
        {

            if (string.IsNullOrEmpty(email))
                return SubscriptionResult.EmailNotValid; // throw new ArgumentNullException("email", "Email address cannot be empty.");

            if (recipientList == 0)
                return SubscriptionResult.RecipientListNotValid; // throw new ArgumentException("recpientList cannot be 0", "recipientList");

            // Get a list to add to, will throw an exception if not found
            RecipientList selectedList = RecipientList.Load(recipientList);
            if(selectedList == null)
            {
                return SubscriptionResult.RecipientListNotValid;
                // throw new ApplicationException("No list with id " + recipientList + " exists.");
            }

            return AddSubscriptionToList(email, selectedList);
        }

        protected SubscriptionResult AddSubscriptionToList(string email, RecipientList selectedList)
        {
            EmailSyntaxValidator validator = new EmailSyntaxValidator(email, false);
            if (validator.IsValid)
            {
                _log.DebugFormat("Attemt to add email subscription for {0}", email);


                EmailAddress emailAddress = selectedList.CreateEmailAddress(email);
                emailAddress.Source = EmailAddressSource.SelfRegistered;
                emailAddress.Added = DateTime.Now;
                emailAddress.Save(); // Will add it to the list, or update it if it exists

                return SubscriptionResult.Success;
            }
            else
            {
                _log.WarnFormat("Failed to add email subscription for '{0}' (not valid)", email);
                return SubscriptionResult.EmailNotValid;
            }
        }


        protected static RecipientList GetPublicList()
        {
            RecipientList selectedList = null;
            RecipientLists lists = RecipientLists.ListOneType(RecipientListType.PublicList.ToString());
            if (lists != null && lists.Items != null && lists.Items.Any())
            {
                if (lists.Items.Count == 1)
                {
                    selectedList = lists.Items.First();
                }
                else
                {
                    // We could have more than one, pick the one with name "Default"
                    selectedList = lists.Items.Find(l => l.Name.ToLower().CompareTo("default") == 0);
                    if (selectedList == null)
                        throw new ApplicationException(
                            "There are more than one public lists, and none called \"Default\"");
                }
            }
            else
            {
                throw new ApplicationException("There are no public recipient lists defined.");
            }
            return selectedList;
        }


        public SubscriptionResult Unsubscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
                return SubscriptionResult.EmailNotValid;

            RecipientList selectedList = GetPublicList();
            return Unsubscribe(email, selectedList);


        }

        public SubscriptionResult Unsubscribe(string email, int recipientList)
        {
            if (string.IsNullOrEmpty(email))
                return SubscriptionResult.EmailNotValid;

            if (recipientList == 0)
                return SubscriptionResult.RecipientListNotValid;

            // Get a list to add to, will throw an exception if not found
            RecipientList selectedList = RecipientList.Load(recipientList);
            return Unsubscribe(email, selectedList);
        }

        public SubscriptionResult Unsubscribe(string email, RecipientList recipientList)
        {
            if (string.IsNullOrEmpty(email))
                return SubscriptionResult.EmailNotValid;

            if (recipientList == null)
                return SubscriptionResult.RecipientListNotValid;

            // Get a list to add to, will throw an exception if not found

            int count = recipientList.RemoveEmailAddresses(email);
            if (count == 0)
                return SubscriptionResult.NotMemberOfList;
            else
            {
                return SubscriptionResult.Success;
            }
        }

        public SubscriptionResult UnsubscribeUsingBlocklist(string email, int listId)
        {
            if (string.IsNullOrEmpty(email))
                return SubscriptionResult.EmailNotValid;

            RecipientList selectedList = RecipientList.Load(listId);
            if(selectedList.ListType != RecipientListType.BlockList)
            {
                throw new ApplicationException("Specified list is not a block list");
            }

            EmailAddress emailAddress = selectedList.CreateEmailAddress(email);
            emailAddress.Comment = "Unsubscribed using opt-out page.";
            emailAddress.Source = EmailAddressSource.SelfRegistered;
            emailAddress.Save();
            
            
            // if already there, fine, no problem
            // if not there, we've added the email
            return SubscriptionResult.Success;
        }
    }
}
