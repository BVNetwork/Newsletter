using System;
using System.Collections.Generic;
using BVNetwork.EPiSendMail.DataAccess;
using BVNetwork.EPiSendMail.Plugin.ItemProviders;

namespace BVNetwork.EPiSendMail.Plugin.ItemProviders
{
    public partial class TextImportProvider : System.Web.UI.UserControl, IEmailImporterProvider
    {
        private IEmailImporter _importer;
        private IShowFeedback _feedbackCtrl;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Do not do databind here - the container will do this for us
        }

        protected void cmdAddCsvEmailAddresses_Click(object sender, EventArgs e)
        {
            string addresses = txtAddEmailWorkItems.Text;
            if (string.IsNullOrEmpty(addresses))
            {
                _feedbackCtrl.ShowError("Please enter email addresses to import.");
                return;
            }

            // Add the items
            List<string> duplicateAddresses;
            List<string> invalidAddresses;
            
            System.Diagnostics.Stopwatch tmr = System.Diagnostics.Stopwatch.StartNew();
            int count = _importer.ImportEmailAddresses(addresses, out invalidAddresses, out duplicateAddresses);
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
                             "Number of duplicates: {2} <br/>\n" +
                             "Number of invalid addresses: {3} <br />\n" +
                             "{4}";

            _feedbackCtrl.ShowInfo(string.Format(message,
                                        count.ToString(),
                                        tmr.ElapsedMilliseconds.ToString(),
                                        duplicateAddresses.Count.ToString(),
                                        invalidAddresses.Count.ToString(),
                                        invalidMessage
                                ));
        }

        public void Initialize(IEmailImporter importer, IShowFeedback feedbackUi)
        {
            _importer = importer;
            _feedbackCtrl = feedbackUi;
        }
    }
}