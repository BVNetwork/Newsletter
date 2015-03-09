using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVNetwork.EPiSendMail.Api.Models
{
    public class JobStatus
    {
        public int Job { get; set; }
        public bool SchedulerIsOnline { get; set; }
        public int EmailsSent { get; set; }
        public int EmailsFailed { get; set; }
        public int EmailsNotSent { get; set; }
        public int EmailsInQueue { get; set; }
        public string Status { get; set; }
        public bool ScheduledTaskIsEnabled { get; set; }
        public DateTime ScheduledTaskNextRun { get; set; }
        public bool ScheduledTaskIsRunning { get; set; }
        public string Name { get; set; }
        public int BatchSize { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
