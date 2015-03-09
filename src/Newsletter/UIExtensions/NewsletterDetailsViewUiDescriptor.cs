using BVNetwork.EPiSendMail.Contracts;
using EPiServer.Shell;

namespace BVNetwork.EPiSendMail.UIExtensions
{
    [UIDescriptorRegistration]
    public class NewsletterDetailsViewUiDescriptor : UIDescriptor<NewsletterBase>
    {

    }


    [UIDescriptorRegistration]
    public class RegisterNewsletterDetailsViewUiDescriptor : UIDescriptor<IRegisterNewsletterDetailsView>
    {

    }
    
}
