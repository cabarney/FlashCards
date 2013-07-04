using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;

namespace FlashCards.ViewModels
{
    public class GroupViewModel : PropertyChangedBase
    {
        private BindableCollection<ItemViewModel> _items;

        public GroupViewModel()
        {
            Items = new BindableCollection<ItemViewModel>();
        }

        public List<ItemViewModel> TopItems
        {
            get
            {
                var top = Items.OrderByDescending(x => x.LastUsed).Take(10).ToList();
                return top;
            }
        }

        public int Id { get; set; } // id of 0 is stat group
        public string Title { get; set; }
        public int SortOrder { get; set; }

        public void RaiseChildNotifications()
        {
            NotifyOfPropertyChange(() => Items);
            NotifyOfPropertyChange(() => TopItems);
        }

        public BindableCollection<ItemViewModel> Items
        {
            get { return _items; }
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                NotifyOfPropertyChange(() => Items);
                NotifyOfPropertyChange(() => TopItems);
            }
        }
    }
}