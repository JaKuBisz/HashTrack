using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;
using System.Diagnostics;

namespace HashTrack
{
    public partial class ThisAddIn
    {
        private Microsoft.Office.Tools.CustomTaskPane myCustomTaskPane;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                var myUserControl1 = new UserControl1();
                myCustomTaskPane = this.CustomTaskPanes.Add(myUserControl1, "My Task Pane");
                myCustomTaskPane.Visible = true;

                myUserControl1.SearchInitiated += MyUserControl1_SearchInitiated;

                Application.AdvancedSearchComplete += Application_AdvancedSearchComplete;
                //TestAdvancedSearchComplete();
            }
            catch (Exception ex)
            {
                // Log or handle exception
                Debug.WriteLine(ex.Message);
            }

        }

        public bool blnSearchComp = false;

        private void Application_AdvancedSearchComplete(Outlook.Search SearchObject)
        {
            if (SearchObject.Tag == "Test")
            {
                blnSearchComp = true;
                Outlook.Results results = SearchObject.Results;
                for (int i = 1; i <= results.Count; i++)
                {
                    Outlook._MailItem mailItem = results[i] as Outlook._MailItem;
                    if (mailItem != null)
                    {
                        mailItem.Display(false);
                        System.Diagnostics.Debug.WriteLine(mailItem.SenderName);
                    }
                    var contactItem = results[i] as Outlook._ContactItem;
                    if (contactItem != null)
                    {
                        contactItem.Display(false);
                        System.Diagnostics.Debug.WriteLine(contactItem.FullName);
                    }
                    var appointmentItem = results[i] as Outlook._AppointmentItem;
                    if (appointmentItem != null)
                    {
                        appointmentItem.Display(false);
                        System.Diagnostics.Debug.WriteLine(appointmentItem.Body);
                    }
                    var taskItem = results[i] as Outlook._TaskItem;
                    if (taskItem != null)
                    {
                        taskItem.Display(false);
                        System.Diagnostics.Debug.WriteLine(taskItem.Body);
                    }
                }
            }
        }

        public void TestAdvancedSearchComplete(string keyword)
        {
            Outlook.MAPIFolder inboxFolder = Application.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            var inboxDefault = inboxFolder.Name;
            //var resultSet = inboxFolder.Items.Restrict($"");

            //inboxFolder.Items.Find
            //inboxFolder.Items.Custom
            /*
            var res = inboxFolder.GetTable("");
            foreach (var item in res.Columns)
            {
                Console.WriteLine(item.ToString());
            }
            var count = res.GetRowCount();
            for (int i = 0; i < count; i++)
            {
                var tmp = res.GetNextRow();
                Console.WriteLine(tmp.ToString());

            }*/
            DateTime expirationDate = DateTime.Now.AddDays(30);
            //string expiresFilter = String.Format("urn:schemas:mailheader:expires>'{0}' AND urn:schemas:mailheader:expires<'{0}'", DateTime.Now, expirationDate);
            string expiresFilter = String.Format("urn:schemas:httpmail:textdescription ci_phrasematch '{0}'", keyword);
            //string expiresFilter = "urn:schemas:httpmail:textdescription LIKE '%a%'";
            //string expiresFilter = String.Format("urn:schemas:calendar:notes ci_phrasematch '{0}'", keyword);
            //string expiresFilter = String.Format("urn:schemas:contacts:notes ci_phrasematch '{0}'", keyword);
            //string expiresFilter = String.Format("urn:schemas:contacts:httpmail:textdescription ci_phrasematch '{0}'", keyword);
            //string expiresFilter = "urn:schemas:contacts LIKE '%T%'";
            //string expiresFilter = String.Format($"[Body] = 'Microsoft'");

            string scope = "'" + inboxFolder.FolderPath + "'";
            Outlook.Search search = Globals.ThisAddIn.Application.AdvancedSearch($"'{inboxDefault}','Contacts','Calendar', 'Tasks'", expiresFilter, true, "Test");

            /*
            string filter = "@SQL=\"urn:schemas:mailheader:subject = 'Test'\"";
            string scope = "Inbox";
            blnSearchComp = false;
            Outlook.Search search = Application.AdvancedSearch(scope, filter, true, "Test");*/
        }

        private void FilterBuilder()
        {

        }

        private void MyUserControl1_SearchInitiated(string keyword)
        {
            // Execute search logic here
            try
            {
                TestAdvancedSearchComplete(keyword); // Update this method to accept a keyword
            }
            catch (Exception ex)
            {
                // Log or handle exception
                Debug.WriteLine(ex.Message);
            }
        }

        private enum ArtifactType
        {
            Emails = 0,
            Appointments = 1,
            Tasks = 2,

        }

        /*
        private void SearchForHashtag(string hashtag)
        {
            //Outlook.OlSearchScope.GetValues
            Application.Session.Session.
            Outlook.MAPIFolder inboxFolder = Application.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            Outlook.Items items = inboxFolder.Items;

            foreach (Outlook.MailItem mail in items)
            {
                if (mail.Body.Contains(hashtag))
                {
                    // Perform your action here
                    // For example, display the subject of emails containing the hashtag
                    System.Diagnostics.Debug.WriteLine($"Subject: {mail.Subject}");
                }
            }
        }

        private Outlook.Search advancedSearch = null;
        private string searchCriteria = string.Empty;

        public void PerformAdvancedSearch(string keyword)
        {
            string scope = "mailbox";
            searchCriteria = $"@SQL=\"http://schemas.microsoft.com/mapi/proptag/0x1000001E\" ci_phrasematch '{keyword}'";
            advancedSearch = Application.AdvancedSearch(scope, searchCriteria, true, "MySearchTag");

            Application.AdvancedSearchComplete += AdvancedSearch_Complete;
        }

        private void AdvancedSearch_Complete()
        {
            if (advancedSearch.Results != null && advancedSearch.Results.Count > 0)
            {
                // Handle results - e.g., display in a task pane or process as needed
                Console.WriteLine($"Found {advancedSearch.Results.Count} items");
            }
        }
        */
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
