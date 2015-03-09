using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVNetwork.EPiSendMail.Api.Models
{
    public class RecipientStatus
    {
        public int ImportedEmails { get; set; }
        public int DuplicatedEmails{ get; set; }
        public int InvalidEmails{ get; set; }
        public string InvalidMessage{ get; set; }
        public string Status { get; set; }
        public DateTime TimeStamp { get; set; }
        public long TimeToImport { get; set; }
    }
}
