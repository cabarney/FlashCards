using System;
using Windows.UI.Xaml;
using Caliburn.Micro;
using FlashCards.Data;
using FlashCards.Model;
using FlashCards.Results;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.ViewManagement;

namespace FlashCards.ViewModels
{
    public class MainViewModel : PresetViewModelBase, IHandle<AddUserMessage>, IHandle<RemoveUserMessage>
    {
        private BindableCollection<GroupViewModel> _groups;
        
        public MainViewModel(INavigationService navigation, IEventAggregator events) : base(navigation)
        {

            //// one time data population
            //var r = new Random();
            //using (var repo = IoC.Get<IDeckRepository>())
            //{
            //    foreach (var deck in repo.All.Where(x=>!x.PresetOnly))
            //    {
            //        repo.Delete(deck);
            //    }

            //    var presets = repo.All.Where(x => x.PresetOnly).ToList();
            //    foreach (var preset in presets)
            //    {
            //        int start = r.Next(40, 70);
            //        int end = r.Next(80, 100);
            //        int cnt = r.Next(20, 60);
            //        for (int i = 0; i < cnt; i++)
            //        {
            //            var deck = Deck.FromPreset(preset);
            //            double targetPercentage = (start + ((double) i/(double) cnt)*(end - start));
            //            targetPercentage = targetPercentage + (r.Next(10) - 5);
            //            targetPercentage = Math.Min(100, targetPercentage);
            //            int correct = (int)((targetPercentage/100.0)*deck.CardCount);
            //            deck.CardsAnswered = deck.CardCount;
            //            deck.CardsCorrect = correct;
            //            deck.DateStarted = DateTime.Now.AddDays(i-cnt+1);
            //            repo.InsertOrUpdate(deck);
            //        }
            //    }
            //}

            DisplayName = "Flash Cards: Math";
            events.Subscribe(this);
            LoadData();
        }

        protected override void OnViewReady(object view)
        {
            NotifyOfPropertyChange(()=>AddAChild);
        }

        private void LoadData()
        {
            Groups = new BindableCollection<GroupViewModel>();

            using (var repo = IoC.Get<IUserRepository>())
            {
                //var statsGroup = new GroupViewModel { Id = 0, Title = "Stats", SortOrder=100 };

                foreach (var u in repo.All.OrderBy(x=>x.Name))
                {
                    Groups.Add(new GroupViewModel { Id = u.Id, Title = u.Name, SortOrder=1 });
                    //statsGroup.Items.Add(new ItemViewModel { Id = u.Id, Title = u.Name, ItemType = "stat" });
                }
                //Groups.Add(statsGroup);
            }

            foreach (var g in Groups.Where(x => x.Id > 0))
            {
                g.Items = new BindableCollection<ItemViewModel>(GetPresets(g.Id));
            }

            SelectedItem = null;

        }

        public BindableCollection<GroupViewModel> Groups
        {
            get { return _groups; }
            set
            {
                if (Equals(value, _groups)) return;
                _groups = value;
                NotifyOfPropertyChange(() => Groups);
            }
        }


 

        public void GoToUser(GroupViewModel group)
        {
            NavigationService.NavigateToViewModel<UserDetailsViewModel>(group.Id);
        }

        public void Handle(RefreshMessage message)
        {
            //NavigationService.NavigateToViewModel<MainViewModel>();
            LoadData();
            Refresh();
        }

        public void Handle(AddUserMessage message)
        {
            User user;
            GroupViewModel group;
            using(var repo = IoC.Get<IUserRepository>())
            {
                user = repo.Find(message.UserId);
                group = new GroupViewModel { Id = user.Id, Items = new BindableCollection<ItemViewModel>(), Title = user.Name };
            }
            group.Items = new BindableCollection<ItemViewModel>(GetPresets(user.Id));

            bool placed = false;
            for (var i = 0; i < Groups.Count; i++)
            {
                if (Groups[i].Title.CompareTo(group.Title) > 0)
                {
                    Groups.Insert(i, group);
                    placed = true;
                    break;
                }
            }
            if(!placed)
                Groups.Add(group);

            NotifyOfPropertyChange(()=>Groups);
            NotifyOfPropertyChange(()=>AddAChild);
        }

        public override void DeletePreset()
        {
            if (SelectedItem != null)
            {
                using (var repo = IoC.Get<IDeckRepository>())
                {
                    repo.Delete(SelectedItem.Id);
                }
                GroupViewModel group = null;
                int idx = 0;
                for (idx=0;idx<Groups.Count;idx++)
                    if (Groups[idx].Items.Contains(SelectedItem))
                    {
                        group = Groups[idx];
                        group.Items.Remove(SelectedItem);
                        break;
                    }
                if (group != null)
                {
                    Groups.RemoveAt(idx);
                    Groups.Insert(idx, group);
                }
                SelectedItem = null;
            }
        }

        public bool AddAChild
        {
            get
            {
                if (Groups == null)
                    return false;
                if (!Groups.Any())
                    return true;
                return false;
            }
        }

        public void Handle(RemoveUserMessage message)
        {
            var group = Groups.SingleOrDefault(x => x.Id == message.UserId);
            Groups.Remove(group);
            NotifyOfPropertyChange(() => Groups);
            NotifyOfPropertyChange(() => AddAChild);
        }

        public IEnumerable<IResult> SizeChanged(FrameworkElement e)
        {
            yield return new VisualStateResult(e.ActualWidth < 650 ? "Paused" : "Running", true);
        }

        public override bool CanGoBack
        {
            get { return false; }
        }
    }
}
