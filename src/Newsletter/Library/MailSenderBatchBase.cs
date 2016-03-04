using System;
using System.Collections.Generic;
using System.Linq;
using BVNetwork.EPiSendMail.DataAccess;
using EPiServer.Logging;
using PreMailer.Net;

// ReSharper disable PossibleMultipleEnumeration

namespace BVNetwork.EPiSendMail.Library
{
	/// <summary>
	/// This class does the actual mail sending, using the standard .NET Framework
	/// </summary>
	public abstract class MailSenderBatchBase : MailSenderBase
	{
        private static readonly ILogger _log = LogManager.GetLogger();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailInfo"></param>
        /// <param name="recepients">receivers</param>
        /// <param name="testMode">No mails are sent, generating report for user</param>
        /// <returns>A html formatted report of the send process</returns>
        public override SendMailLog SendEmail(MailInformation mailInfo, JobWorkItems recepients, bool testMode)
		{
			_log.Debug("Starting Send");
			// for logging
			SendMailLog log = new SendMailLog();

			// Need someone to send to
			if (recepients.Items.Count == 0)
			{
				_log.Error("Trying to send newsletter with an empty JobWorkCollection. Please check the collection before attemting to send mail.");
				throw new ArgumentNullException("recepients", "Recipient collection is empty, there is no recipients to send to.");
			}

			// And, we need a sender address
			if (string.IsNullOrEmpty(mailInfo.From))
			{
				_log.Error("Missing from address.");
				throw new ArgumentNullException("mailInfo", "Missing from address.");
			}

            // Inline all css
            PreMailer.Net.PreMailer preMailer = new PreMailer.Net.PreMailer(mailInfo.BodyHtml);
            if (mailInfo.Utm.HasValidUtmCode)
            {
                preMailer.AddAnalyticsTags(mailInfo.Utm.Source, mailInfo.Utm.Medium, mailInfo.Utm.Campaign,
                    mailInfo.Utm.Content);
            }
            InlineResult cssInline = preMailer.MoveCssInline();
            mailInfo.BodyHtml = cssInline.Html;
			
            // Log any messages, debug is only detected
            // if we have an HttpContext.
            if (IsInDebugMode())
            {
                log.WarningMessages.Add("Premailer CSS warning messages are only shown in debug mode. Primarily for developers.");
                log.WarningMessages.AddRange(cssInline.Warnings.ToArray());
            }

			// Loop through receivers collection, add to collection and send
			// one email per batch size.
			int batchRun = 0;
			int batchSize = GetBatchSize();

			do
			{
				IEnumerable<JobWorkItem> workItems = recepients.Skip(batchRun*batchSize).Take(batchSize);
				int numberofItemsToSend = workItems.Count();
				if (numberofItemsToSend == 0)
					break;
				batchRun++;

				try
				{
					if (SendMailBatch(mailInfo, workItems, testMode))
					{
						// Mark each item as sent
						foreach (JobWorkItem workItem in workItems)
						{
							log.SuccessMessages.Add(workItem.EmailAddress);
							// Update status and save it
							workItem.Status = JobWorkStatus.Complete;
							// Only save if real work item (could be a test item)
							if (workItem.JobId > 0)
								workItem.Save();
						}
					}
				}
				catch (Exception ex)
				{
					_log.Error(string.Format("Error sending batch (to {0} recipients).", recepients.Count()), ex);
					string exceptionMsg = ex.Message;
					log.ErrorMessages.Add(exceptionMsg);

					// Update work item
					foreach (JobWorkItem workItem in workItems)
					{
						workItem.Status = JobWorkStatus.Failed;
						if (exceptionMsg.Length >= 2000)
							exceptionMsg = exceptionMsg.Substring(0, 1999);
						workItem.Info = exceptionMsg;
						if (workItem.JobId > 0)
							workItem.Save();
					}

					// can't continue
					break;
				}
			} while (true);

			// Finished
			log.SendStop = DateTime.Now;

			_log.Debug("Ending Send");
			// return report 
			return log;
		}

	    public abstract bool SendMailBatch(MailInformation mail, IEnumerable<JobWorkItem> recipients, bool onlyTestDontSendMail);
	    
	}

}
