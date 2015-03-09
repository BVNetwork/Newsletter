using System.Collections.Generic;

namespace BVNetwork.EPiSendMail.Contracts
{
    public interface IPopulateCustomProperties
    {
        void AddCustomProperties(Dictionary<string, object> properties);
    }
}