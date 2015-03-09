using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BVNetwork.EPiSendMail.Api.Models;
using BVNetwork.EPiSendMail.Configuration;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.DataAccess.DataUtil;
using BVNetwork.EPiSendMail.ScheduledTasks;
using EPiServer.DataAbstraction;
using Newtonsoft.Json.Linq;
using JobStatus = BVNetwork.EPiSendMail.Api.Models.JobStatus;

namespace BVNetwork.EPiSendMail.Api
{

    [Authorize(Roles = "CmsAdmins,NewsletterAdmins")]
    public class InstallerController : ApiController
    {

        [HttpGet]
        public JObject GetDatabaseVersion()
        {
            SystemData systemData = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<SystemData>();
            JObject ret = JObject.FromObject(new
            {
                database = new
                {
                    version = systemData.GetNewsletterDatabaseVersion()
                }
            });

            return ret;
        }
        
        [HttpPost]
        public JObject InstallDatabase()
        {
            SystemData systemData = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<SystemData>();

            systemData.InstallNewsletterDatabase(DatabaseVersion.CurrentDatabaseVersion);

            return GetDatabaseVersion();
        }

        [HttpPost]
        public JObject SetDatabaseVersion(int version)
        {
            SystemData systemData = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<SystemData>();
            
            int oldDatabaseVersion = systemData.GetNewsletterDatabaseVersion();

            systemData.SetNewsletterDatabaseVersion(version);

            JObject ret = JObject.FromObject(new
            {
                database = new
                {
                    oldVersion = oldDatabaseVersion,
                    newVersion = systemData.GetNewsletterDatabaseVersion()
                }
            });

            return ret;
        }

    }
}
