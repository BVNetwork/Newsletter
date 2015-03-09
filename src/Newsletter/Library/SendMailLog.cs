using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using BVNetwork.EPiSendMail.Configuration;

namespace BVNetwork.EPiSendMail.Library
{
	/// <summary>
	/// Class for holding log information about a particular mail job
	/// </summary>
	/// <remarks>
	/// This class holds collections of email addresses that are part
	/// of a job. The following statuses are tracked:
	///  * Success
	///  * Error
	/// </remarks>
	public class SendMailLog
	{
		private DateTime _sendStart;
		private DateTime _sendStop;
		private Stopwatch _totalDurationTimer = null;
		private string _subject;

		private StringCollection _successMessages;
		private StringCollection _errorMessages;
		private StringCollection _warningMessages;

		public SendMailLog()
		{
			_successMessages = new StringCollection();
			_errorMessages = new StringCollection();
			_warningMessages = new StringCollection();
            SendStart = DateTime.Now;
		}

		/// <summary>
		/// Starts the job, starting a total duration timer
		/// and stores the time of start.
		/// </summary>
		public void StartJob()
		{
			_sendStart = DateTime.Now;
			_totalDurationTimer = Stopwatch.StartNew();
		}

		public void StopJob()
		{
			_sendStop = DateTime.Now;
			_totalDurationTimer.Stop();
		}

		public string Subject
		{
			get
			{
				return _subject;
			}
			set
			{
				_subject = value;
			}
		}

		/// <summary>
		/// Gets or sets the total duration of the sending process
		/// in milliseconds
		/// </summary>
		/// <remarks>Will return 0 if the job has not been started yet.</remarks>
		/// <value>The total duration.</value>
		public long TotalDuration
		{
			get
			{
				if (_totalDurationTimer == null)
					return 0;

				return _totalDurationTimer.ElapsedMilliseconds;
			}
		}

		public DateTime SendStart
		{
			get
			{
				return _sendStart;
			}
			set
			{
				_sendStart = value;
			}
		}

		public DateTime SendStop
		{
			get
			{
				return _sendStop;
			}
			set
			{
				_sendStop = value;
			}
		}

		public StringCollection SuccessMessages
		{
			get
			{
				return _successMessages;
			}
		}

		public StringCollection ErrorMessages
		{
			get
			{
				return _errorMessages;
			}
		}

		public StringCollection WarningMessages
		{
			get
			{
				return _warningMessages;
			}
		}

		public int NumberOfEmailsSent
		{
			get
			{
				return _successMessages.Count + _errorMessages.Count;
			}
		}

		/// <summary>
		/// Returns a report in html format.
		/// </summary>
		/// <returns></returns>
		public MailInformation GetHtmlReport()
		{
			string subject = string.Format("Mail Sender Log, mail sent between {0} and {1}", this.SendStart, this.SendStop);
		    string from = NewsLetterConfiguration.GetAppSettingsConfigValueEx<string>("EPsSendMailSendReportMailTo", null);

			return GetHtmlReport(subject, from);
		}

		public MailInformation GetHtmlReport(string from)
		{
			string subject = string.Format("Mail Sender Log, mail sent between {0} and {1}", this.SendStart, this.SendStop);
			return GetHtmlReport(subject, from);
		}


		public MailInformation GetHtmlReport(string from, string subject)
		{
			return GetHtmlReport(from, subject, true);
		}

		public MailInformation GetHtmlReport(string from, string subject, bool ShowSuccessAddresses)
		{
			if (from == null)
				throw new ArgumentNullException("from", "From email address cannot be null");

			MailInformation mailInfo = new MailInformation();
			mailInfo.Subject = subject;
			mailInfo.From = from;

			System.Text.StringBuilder reportMailBody = new System.Text.StringBuilder();
			reportMailBody.AppendLine("<div id=\"MailReport\">");

			// Start send
			reportMailBody.AppendFormat("<div class=\"onelinesection\"><span class=\"onelinesectionlabel\">Started send:</span> <span class=\"onelinesectionvalue\">{0}</span></span></div>", this.SendStart.ToString());

			// Start send
			reportMailBody.AppendFormat("<div class=\"onelinesection\"><span class=\"onelinesectionlabel\">End send:</span> <span class=\"onelinesectionvalue\">{0}</span></div>", this.SendStop.ToString());

			// Duration
			TimeSpan duration = TimeSpan.FromMilliseconds((double)this.TotalDuration);
			reportMailBody.AppendFormat("<div class=\"onelinesection\"><span class=\"onelinesectionlabel\">Duration:</span> <span class=\"onelinesectionvalue\">{0}hr {1}min {2}sec</span></div>", duration.Hours.ToString(), duration.Minutes.ToString(), duration.Seconds.ToString());

			// Number of emails sent
			reportMailBody.AppendFormat("<div class=\"onelinesection\"><span class=\"onelinesectionlabel\">Number of emails sent:</span> <span class=\"onelinesectionvalue\">{0}</span></div>", this.NumberOfEmailsSent.ToString());

			// Exceptions first
			if (ErrorMessages.Count > 0)
				AppendMultiLineSectionAsHtml("Exceptions", ErrorMessages, reportMailBody);

			// Warnings next
			if (WarningMessages.Count > 0)
				AppendMultiLineSectionAsHtml("Warnings", WarningMessages, reportMailBody);

			// Successfull HTML Text Recipients
			if (ShowSuccessAddresses == true)
				AppendMultiLineSectionAsHtml("Successfull HTML Format Recipients", SuccessMessages, reportMailBody);

			reportMailBody.AppendLine("</div>");
			mailInfo.BodyHtml = reportMailBody.ToString();

			return mailInfo;
		}

		private void AppendMultiLineSectionAsHtml(string caption, StringCollection stringItems, System.Text.StringBuilder reportMailBody)
		{
			reportMailBody.Append("<div class=\"multilinesection\"><b>" + caption + ":</b><br /><blockquote class=\"multilinesection\">");
			foreach(string itm in stringItems)
				reportMailBody.AppendFormat("{0}<br />\r\n", itm);
			reportMailBody.Append("</blockquote></div>");
		}

		public string GetClearTextLogString()
		{
			return GetClearTextLogString(false);
		}

		public string GetClearTextLogString(bool formatAsHtml)
		{
			System.Text.StringBuilder reportBody = new System.Text.StringBuilder();

			string msg = "Send Newsletter report \nSubject: {5} \nStart: {0} \nFinished: {1} \nDuration: {2}ms \nEmails sent: {3} \n Errors: {4}\n";

			reportBody.AppendFormat(msg,
					SendStart.ToString(),
					SendStop.ToString(),
					TotalDuration.ToString(),
					SuccessMessages.Count.ToString(),
					ErrorMessages.Count.ToString(),
					Subject
					);

            GetMessagesMarkup(formatAsHtml, reportBody, ErrorMessages, "Errors", "danger");

			GetMessagesMarkup(formatAsHtml, reportBody, WarningMessages, "Warnings", "warning");

		    msg = reportBody.ToString();
			if (formatAsHtml)
				msg = msg.Replace("\n", "<br />\n");

			return msg;
		}

	    protected void GetMessagesMarkup(bool formatAsHtml, StringBuilder reportBody, StringCollection messages, string header, string panelClass)
	    {
	        // Warnings
	        if (messages.Count > 0)
	        {
                
	            if (formatAsHtml)
	            {
                    reportBody.AppendFormat(
                        "<div class=\"panel panel-{1}\"><div class=\"panel-heading\"><h3 class=\"panel-title\">{0}</h3></div><div class=\"panel-body\" style=\"max-height: 100px;overflow-y:auto;\">",
                        header, panelClass);
	            }
	            else
	                reportBody.AppendFormat("{0}\n", header);

	            foreach (string msg in messages)
	            {
                    reportBody.AppendFormat("{0}\n", msg);
	            }

	            if (formatAsHtml)
                    reportBody.Append("</div></div>");
	        }
	    }
	}
}
