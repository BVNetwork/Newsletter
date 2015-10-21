namespace BVNetwork.EPiSendMail
{
    public interface INewsletterBase
    {
        string MailSender { get; set; }
        string MailSubject { get; set; }
    }
}