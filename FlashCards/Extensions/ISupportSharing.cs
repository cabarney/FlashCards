using Windows.ApplicationModel.DataTransfer;

namespace FlashCards.Extensions
{
    public interface ISupportSharing
    {
        void OnShareRequested(DataRequest dataRequest);
    }
}
