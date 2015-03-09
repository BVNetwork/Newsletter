namespace BVNetwork.EPiSendMail.Library
{
	public class SubscriptionStatus
	{
	    public string Message { get; set; }
	    public bool SubscriptionResult { get; set; }
	    public string RecipientListName { get; set; }

	    public SubscriptionStatus(){}

		public SubscriptionStatus(string newsletterName, bool subscriptionResult, string message)
		{
			RecipientListName = newsletterName;
			SubscriptionResult = subscriptionResult;
			Message = message;
		}
	}

}
