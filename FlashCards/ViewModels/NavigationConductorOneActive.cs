using Caliburn.Micro;

namespace FlashCards.ViewModels
{
    public class NavigationConductorOneActive<T> :  Conductor<T>.Collection.OneActive where T : class
    {
        protected readonly INavigationService NavigationService;

        public NavigationConductorOneActive(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public virtual void GoBack()
        {
            NavigationService.GoBack();
        }

        public bool CanGoBack
        {
            get
            {
                return NavigationService.CanGoBack;
            }
        }       
    }
}