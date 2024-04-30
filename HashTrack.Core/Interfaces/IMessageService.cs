using HashTrack.Core.Enums;

namespace HashTrack.Core.Interfaces
{
    public interface IMessageService
    {
        void ShowMessage(string text, string caption, MessageType messageType);

        void ShowMessage(System.Exception exception, string caption = "Exception occured",
            MessageType messageType = MessageType.Error);

        bool ShowQuestion(string text, string caption);
    }
}