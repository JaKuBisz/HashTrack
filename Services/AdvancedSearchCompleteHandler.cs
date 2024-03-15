using HashTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.Services
{
    public class AdvancedSearchCompleteHandler
    {
        public void OnAdvancedSearchComplete(Outlook.Search SearchObject)
        {
            // Handle the event here
            // This is just an example, replace with your own logic
            if (SearchObject.Tag == Constants.DefaultSearchTag)
            {
                //blnSearchComp = true;
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
    }
}
