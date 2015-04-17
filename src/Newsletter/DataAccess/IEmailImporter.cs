using System.Collections.Generic;

namespace BVNetwork.EPiSendMail.DataAccess
{
    public interface IEmailImporter
    {
        int ImportEmailAddresses(string emailAddressesDelimited, out List<string> invalidEmailAddresses, out List<string> duplicateAddresses);

        /// <summary>
        /// Imports email addresses into a Recipient List
        /// </summary>
        /// <param name="emailArray">The email addresses to import.</param>
        /// <returns>The number of email addresses imported as new work items. Duplicates are not part of this number.</returns>
        int ImportEmailAddresses(string[] emailArray);

        /// <summary>
        /// Imports email addresses into a Recipient List
        /// </summary>
        /// <param name="emailArray">The email addresses to import.</param>
        /// <param name="invalidEmailAddresses">A list of all available email addresses that could not be parsed as a valid address</param>
        /// <returns>The number of email addresses imported as new work items. Duplicates are not part of this number.</returns>
        int ImportEmailAddresses(string[] emailArray, out List<string> invalidEmailAddresses, out List<string> duplicateAddresses);
    }
}