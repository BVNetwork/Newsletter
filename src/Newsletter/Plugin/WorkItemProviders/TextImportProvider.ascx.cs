using System;
using System.Collections.Generic;
using BVNetwork.EPiSendMail.DataAccess;

namespace BVNetwork.EPiSendMail.Plugin.WorkItemProviders
{
    public partial class TextImportProvider : System.Web.UI.UserControl, IWorkItemProvider
    {
        private Job _job;
        private IJobUi _jobUi;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Do not do databind here - the container will do this for us
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
        }

        protected void cmdAddCsvEmailAddresses_Click(object sender, EventArgs e)
        {
            string addresses = txtAddEmailWorkItems.Text;
            if (string.IsNullOrEmpty(addresses))
            {
                _jobUi.ShowError("Please enter email addresses to import.");
                return;
            }

            // Add the items
            List<string> duplicateAddresses;
            List<string> invalidAddresses;
            
            System.Diagnostics.Stopwatch tmr = System.Diagnostics.Stopwatch.StartNew();
            int count = _job.ImportEmailAddresses(addresses, out invalidAddresses, out duplicateAddresses);
            tmr.Stop();

            string invalidMessage = "";
            if (invalidAddresses.Count > 0)
            {
                // Show invalid addresses
                invalidMessage = "<blockquote>\n";
                invalidMessage += string.Join(", ", invalidAddresses.ToArray());
                invalidMessage += "</blockquote>\n";
            }

            // Construct log message
            string message = "Imported {0} email addresses <br/>\n" + 
                             "Number of duplicates: {2} <br />\n" +
                             "Number of invalid addresses: {3} <br />\n" +
                             "{4}";

            _jobUi.ShowInfo(string.Format(message, 
                                        count.ToString(),
                                        tmr.ElapsedMilliseconds.ToString(),
                                        duplicateAddresses.Count.ToString(),
                                        invalidAddresses.Count.ToString(),
                                        invalidMessage                                        
                                ));
        }

        #region IWorkItemProvider Members
  
        public void Initialize(Job job, IJobUi jobUi)
        {
            _job = job;
            _jobUi = jobUi;
        }

        #endregion
    }
}