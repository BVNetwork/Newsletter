namespace BVNetwork.EPiSendMail.Library
{
	/// <summary>
	/// Type of verification.
	/// </summary>
	public enum VerificationType
	{
		Error,
		Warning
	}

	public class EnvironmentVerificationItem
	{
		private string _message;
		private VerificationType _verificationType;

		/// <summary>
		/// Initializes a new instance of the EnvironmentVerificationItem class taking type and message.
		/// </summary>
		/// <param name="verificationType">Type of item, can be used to stop processing for an example.</param>
		/// <param name="message">A message explaining the </param>
		public EnvironmentVerificationItem(VerificationType verificationType, string message)
		{
			_message = message;
			_verificationType = verificationType;
		}

		/// <summary>
		/// The type of verification item this is. Depending on
		/// the type, the caller can choose the action to take. 
		/// One or more Error items could lead to the stop of 
		/// the process.
		/// </summary>
		/// <value></value>
		public VerificationType VerificationType
		{
			get
			{
				return _verificationType;
			}
		}

		/// <summary>
		/// The verification message
		/// </summary>
		/// <value></value>
		public string Message
		{
			get
			{
				return _message;
			}
		}

		
	}
}
