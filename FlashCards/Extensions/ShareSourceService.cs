using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FlashCards.Extensions
{
    public class ShareSourceService
    {
        private readonly Frame _rootFrame;
        private readonly DataTransferManager _transferManager;

        public ShareSourceService(Frame rootFrame)
        {
            _rootFrame = rootFrame;
            _transferManager = DataTransferManager.GetForCurrentView();

            _transferManager.DataRequested += OnDataRequested;
        }

        protected virtual void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var view = _rootFrame.Content as FrameworkElement;

            if (view == null)
                return;

            var supportSharing = view.DataContext as ISupportSharing;

            if (supportSharing == null)
                return;

            supportSharing.OnShareRequested(args.Request);
        }
    }
}
