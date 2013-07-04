using Caliburn.Micro;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FlashCards.Results
{
    public class ShowAppBarResult : ResultBase
    {
        private readonly string _appBarName;

        public ShowAppBarResult(string appBarName)
        {
            _appBarName = appBarName;
        }

        public override void Execute(ActionExecutionContext context)
        {
            var view = (Control)context.View;
            var appBar = view.FindName(_appBarName);
            if (appBar as AppBar != null)
                (appBar as AppBar).Focus(FocusState.Programmatic);
            
            OnCompleted();
        }
    }
}