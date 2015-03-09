namespace BVNetwork.EPiSendMail.Library
{
    public interface IMailSenderVerification
    {
        /// <summary>
        /// Verifies the environment, called before the send process 
        /// starts to verify that important settings and infrastructure
        /// is in place.
        /// </summary>
        /// <returns>An EnvironmentVerification object, holding all the verification tests that has been run</returns>
        EnvironmentVerification VerifyEnvironment();
    }
}