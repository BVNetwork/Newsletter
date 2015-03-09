namespace BVNetwork.EPiSendMail.Library
{
	public class EnvironmentVerification
	{
		private EnvironmentVerificationItemCollection _envItems;

		/// <summary>
		/// All verification items processed
		/// </summary>
		/// <value></value>
		public EnvironmentVerificationItemCollection VerificationItems
		{
			get
			{
				if (_envItems == null)
					_envItems = new EnvironmentVerificationItemCollection();
				return _envItems;
			}
		}

		public bool HasErrors()
		{
			foreach(EnvironmentVerificationItem item in VerificationItems)
			{
				if (item.VerificationType == VerificationType.Error)
					return true;
			}
			return false;
		}

		public bool HasWarnings()
		{
			foreach(EnvironmentVerificationItem item in VerificationItems)
			{
				if (item.VerificationType == VerificationType.Warning)
					return true;
			}
			return false;
		}

	}
}
