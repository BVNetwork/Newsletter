using System.Collections.Generic;
using EPiServer.Core;

namespace BVNetwork.EPiSendMail.Library
{
	/// <summary>
	/// Holds information about one mail to send to several recipients. The information
	/// is separated from the sending method, to allow more than one type of senders.
	/// </summary>
	public class MailInformation
	{
	    private Dictionary<string, object> _customProperties = new Dictionary<string, object>();

	    public MailInformation()
		{
		    EnableTracking = true;
            Utm = new UtmCode();

        }

		public MailInformation(string from, string subject, string bodyText, string bodyHtml)
		{
            EnableTracking = true;
			From = from;
			Subject = subject;
			BodyText = bodyText;
			BodyHtml = bodyHtml;
		}

		/// <summary>
		/// Gets or sets the base URL for links in newsletters.
		/// </summary>
		/// <value>The base URL.</value>
		public string BaseUrl { get; set; }

		/// <summary>
		/// Gets or sets the page link to the EPiServer page
		/// used for generating the mail info.
		/// </summary>
		/// <value></value>
		public PageReference PageLink { get; set; }

		/// <summary>
		/// Gets or sets the name of the page to be sent.
		/// </summary>
		/// <value></value>
		public string PageName { get; set; }

		/// <summary>
		/// Gets or sets the email address of the sender.
		/// </summary>
		/// <value></value>
		public string From { get; set; }

		/// <summary>
		/// Gets or sets the subject of the email message
		/// </summary>
		/// <value></value>
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the clear text message to be sent to those
		/// that wants that.
		/// </summary>
		/// <value></value>
		public string BodyText { get; set; }

		/// <summary>
		/// Gets or sets the body HTML text of the mail.
		/// </summary>
		/// <value></value>
		public string BodyHtml { get; set; }

        /// <summary>
        /// Enables or disables tracking of open and link click in the email
        /// if the sender supports this.
        /// </summary>
        /// <value>
        ///   <c>true</c> to enable tracking; otherwise, <c>false</c>. Default is true.
        /// </value>
	    public bool EnableTracking { get; set; }

	    /// <summary>
	    /// Gets or sets custom properties that can be 
	    /// </summary>
	    /// <value>
	    /// The custom properties.
	    /// </value>
	    public Dictionary<string, object> CustomProperties
	    {
	        get { return _customProperties; }
	        set { _customProperties = value; }
	    }

        public UtmCode Utm { get; set; }
        
	}
}
