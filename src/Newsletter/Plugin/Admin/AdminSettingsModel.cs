using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace BVNetwork.EPiSendMail.Plugin.Admin
{
    [EPiServerDataStore(AutomaticallyRemapStore = true, AutomaticallyCreateStore = true)]
    public class AdminSettingsModel
    {
        //Should be siteid, handle load balanced site
        public Identity Id { get; set; }

        //TODO: add properties, all from NewsLetterAdmin.aspx should be saved
 
        
    }
}