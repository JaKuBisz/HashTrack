using System.Windows.Forms;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;

namespace HashTrack.Services
{
    [RegisterService(LifeCycle.Transient, typeof(IMessageService))]
    public class MessageService : IMessageService
    {
        public void ShowMessage(string text, string caption, MessageType messageType)
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, GetMessageBoxIcon(messageType));
        }

        public void ShowMessage(System.Exception exception, string caption = "Exception occured",
            MessageType messageType = MessageType.Error)
        {
            ShowMessage(exception.Message, caption, messageType);
        }
        
        public bool ShowQuestion(string text, string caption)
        {
            var result = MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        private MessageBoxIcon GetMessageBoxIcon(MessageType messageType)
        {
            switch(messageType)
            {
                case MessageType.Information:
                    return MessageBoxIcon.Information;
                case MessageType.Warning:
                    return MessageBoxIcon.Warning;
                case MessageType.Error:
                    return MessageBoxIcon.Error;
                default:
                    return MessageBoxIcon.None;
            }
        }
    }
}