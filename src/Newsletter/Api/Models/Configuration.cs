using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.Library;

namespace BVNetwork.EPiSendMail.Api.Models
{
    public class Configuration
    {
        public Configuration()
        {

            BatchSize = NewsLetterConfiguration.SendBatchSize;
            SchedulerIsOnline = EPiServer.DataAbstraction.ScheduledJob.IsServiceOnline;
        }

        public bool SchedulerIsOnline { get; set; }

        public int BatchSize { get; set; }
    }
}
