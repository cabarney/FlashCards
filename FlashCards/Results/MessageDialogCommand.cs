using Windows.UI.Popups;

namespace FlashCards.Results
{
    public class MessageDialogCommand
    {
        public MessageDialogCommand(string label,UICommandInvokedHandler command)
        {
            Command = command;
            Label = label;
        }

        public UICommandInvokedHandler Command { get; set; }
        public string Label { get; set; }

        public UICommand ToUICommand()
        {
            return new UICommand(Label, Command);
        }
    }
}