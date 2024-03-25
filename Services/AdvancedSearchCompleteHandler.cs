using HashTrack.DTOs;
using HashTrack.Enums;
using HashTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.Services
{
    [RegisterService(typeof(AdvancedSearchCompleteHandler), LifeCycle.Singleton)]
    public class AdvancedSearchCompleteHandler
    {
        private readonly HashTrackSearchWpfControl _hashTrackSearchWpfControl;
        public AdvancedSearchCompleteHandler(HashTrackSearchWpfControl hashTrackSearchWpfControl)
        {

            _hashTrackSearchWpfControl = hashTrackSearchWpfControl;

        }
        public void OnAdvancedSearchComplete(Outlook.Search SearchObject)
        {
            // Handle the event here
            // This is just an example, replace with your own logic
            if (SearchObject.Tag == Constants.DefaultSearchTag)
            {
                //blnSearchComp = true;
                Outlook.Results results = SearchObject.Results;
                var transformedResults = TransformResultForView(results);


                _hashTrackSearchWpfControl.AddSearchResults(transformedResults);

                /*
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
                    }*/
            }
        }

        private List<SearchResultViewItem> TransformResultForView(Outlook.Results results)
        {
          
            var searchResults = new List<SearchResultViewItem>();
            for (int i = 1; i <= results.Count; i++)
            {
                searchResults.Add(MapSingleResult(results[i]));
            }
            return searchResults;

            SearchResultViewItem MapSingleResult(object result)
            {
                if(result is Outlook._MailItem mailItem)
                {
                    return new SearchResultViewItem
                    {
                        Title = mailItem.Subject,
                        Sender = mailItem.SenderName,
                        Date = mailItem.ReceivedTime,
                        Type = "Email",
                        OriginalItem = mailItem
                    };
                }
                else if(result is Outlook._ContactItem contactItem)
                {
                    return new SearchResultViewItem
                    {
                        Title = contactItem.FullName,
                        Sender = contactItem.Email1Address,
                        Date = DateTime.Now,
                        Type = "Contact",
                        OriginalItem = contactItem
                    };
                }
                else if(result is Outlook._AppointmentItem appointmentItem)
                {
                    return new SearchResultViewItem
                    {
                        Title = appointmentItem.Subject,
                        Sender = appointmentItem.Organizer,
                        Date = appointmentItem.Start,
                        Type = "Appointment",
                        OriginalItem = appointmentItem
                    };
                }
                else if(result is Outlook._TaskItem taskItem)
                {
                    return new SearchResultViewItem
                    {
                        Title = taskItem.Subject,
                        Sender = taskItem.Owner,
                        Date = taskItem.CreationTime,
                        Type = "Task",
                        OriginalItem = taskItem
                    };
                }
                else
                {
                    throw new ArgumentException("The result is not of any known type");
                }


            }
        }
    }
}
