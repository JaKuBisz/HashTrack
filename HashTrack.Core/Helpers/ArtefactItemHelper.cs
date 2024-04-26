using System;
using HashTrack.Core;
using HashTrack.Core.Models.Search;
using HashTrack.DTOs;
using HashTrack.Exception;
using Outlook = Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Interop.Outlook;

namespace HashTrack.Helpers
{
    public static class ArtefactItemHelper
    { 
        public static ArtefactModel MapSearchResultViewItem(object result)
        {
            if(result is Outlook._MailItem mailItem)
            {
                return new ArtefactModel
                {
                    Title = mailItem.Subject,
                    Sender = mailItem.SenderName,
                    Date = mailItem.ReceivedTime,
                    Type = "Email",
                    OriginalItem = mailItem,
                    Id = GetGuid(mailItem.UserProperties),
                };
            }
            
            if(result is Outlook._ContactItem contactItem)
            {
                return new ArtefactModel
                {
                    Title = contactItem.FullName,
                    Sender = contactItem.Email1Address,
                    Date = DateTime.Now,
                    Type = "Contact",
                    OriginalItem = contactItem,
                    Id = GetGuid(contactItem.UserProperties),
                };
            }
            
            if(result is Outlook._AppointmentItem appointmentItem)
            {
                return new ArtefactModel
                {
                    Title = appointmentItem.Subject,
                    Sender = appointmentItem.Organizer,
                    Date = appointmentItem.Start,
                    Type = "Appointment",
                    OriginalItem = appointmentItem,
                    Id = GetGuid(appointmentItem.UserProperties),
                };
            }
            
            if(result is Outlook._TaskItem taskItem)
            {
                return new ArtefactModel
                {
                    Title = taskItem.Subject,
                    Sender = taskItem.Owner,
                    Date = taskItem.CreationTime,
                    Type = "Task",
                    OriginalItem = taskItem,
                    Id = GetGuid(taskItem.UserProperties),
                };
            }
            
            throw new UnknownResultItemTypeException($"The result is not of any known type for result: '{result}'");
            
            Guid GetGuid(Outlook.UserProperties property)
            {
                return Guid.TryParse(property
                    .Find(Constants.CustomProperties.artefactID).Value.ToString(), out var id)
                    ? id
                    : Guid.Empty;
            }
        }
        
        public static string GetBody(object item)
        {
            switch (item)
            {
                case MailItem mailItem:
                    // For emails, you might prefer the Body property
                    return mailItem.Body;
                case AppointmentItem appointmentItem:
                    // Combine relevant fields for appointments
                    return appointmentItem.Body;
                case ContactItem contactItem:
                    // Perhaps concatenate relevant contact information
                    return contactItem.Body;
                case TaskItem taskItem:
                    // Task subject and body might be relevant
                    return taskItem.Body;
                // Add more cases for other item types as needed
                default:
                    return null; // or some indication that the type isn't handled
            }
        }
    }
}