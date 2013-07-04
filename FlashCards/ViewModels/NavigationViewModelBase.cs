using Caliburn.Micro;

namespace FlashCards.ViewModels
{
    public class NavigationViewModelBase : Screen
    {
        protected readonly INavigationService NavigationService;

        public NavigationViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public void GoBack()
        {
            NavigationService.GoBack();
        }

        public virtual bool CanGoBack
        {
            get
            {
                return NavigationService.CanGoBack;
            }
        }
    }
}