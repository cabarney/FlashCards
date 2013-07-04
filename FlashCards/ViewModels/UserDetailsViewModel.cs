using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FlashCards.Data;
using FlashCards.Model;
using FlashCards.Results;
using Windows.UI.ViewManagement;

namespace FlashCards.ViewModels
{
    public class UserDetailsViewModel : PresetViewModelBase
    {
        private BindableCollection<ItemViewModel> _items;

        public UserDetailsViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        protected override void OnViewReady(object view)
        {
            Items = new BindableCollection<ItemViewModel>(GetPresets(Parameter));
            using(var repo=IoC.Get<IUserRepository>())
            {
                var user = repo.Find(Parameter);
                DisplayName = user.Name;
            }
            SelectedItem = null;
        }

        public BindableCollection<ItemViewModel> Items
        {
            get { return _items; }
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }

        public int Parameter { get; set; }

        public IEnumerable<IResult> SizeChanged()
        {
            yield return new VisualStateResult(ApplicationView.Value.ToString(), true);
        }
    }
}
