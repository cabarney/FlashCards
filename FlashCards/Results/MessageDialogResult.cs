using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Windows.UI.Popups;
using Action = System.Action;

namespace FlashCards.Results
{
    public class MessageDialogResult : ResultBase
    {
        private readonly string _content;
        private readonly string _title;
        private readonly IEnumerable<MessageDialogCommand> _commands;

        public MessageDialogResult(string content, string title, IEnumerable<MessageDialogCommand> commands = null)
        {
            _content = content;
            _title = title;
            _commands = commands;
        }

        public async override void Execute(CoroutineExecutionContext context)
        {
            var dialog = new MessageDialog(_content, _title);
            if(_commands != null)
                foreach(var cmd in _commands)
                    dialog.Commands.Add(cmd.ToUICommand());
            
            await dialog.ShowAsync();

            OnCompleted();
        }
    }
}
