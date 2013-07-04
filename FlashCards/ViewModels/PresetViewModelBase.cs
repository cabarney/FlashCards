using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using FlashCards.Data;
using FlashCards.Model;
using Windows.UI.Xaml.Controls;

namespace FlashCards.ViewModels
{
    public class PresetViewModelBase : NavigationViewModelBase
    {
        public PresetViewModelBase(INavigationService navigationService) : base(navigationService)
        {
        }

        private ItemViewModel _selectedItem;

        protected IEnumerable<ItemViewModel> GetPresets(int userId)
        {
            var items = new List<ItemViewModel>();
            using (var repo = IoC.Get<IDeckRepository>())
            {
                foreach (var deck in repo.All.Where(x => x.UserId == userId && x.PresetOnly))
                {
                    var preset = new ItemViewModel
                                     {
                                         TimeLimit = deck.TimeLimit == 0? "": String.Format("{0}:{1}", (int)deck.TimeLimit/60, (deck.TimeLimit%60).ToString().PadLeft(2,'0')),
                                         Title = deck.PresetName,
                                         Id = deck.Id,
                                         ItemType = "preset",
                                         LastUsed = deck.DateStarted,
                                         IncludesAddition = deck.OperationsIncluded.Contains("A"),
                                         IncludesSubtraction = deck.OperationsIncluded.Contains("S"),
                                         IncludesMultiplication = deck.OperationsIncluded.Contains("M"),
                                         IncludesDivision = deck.OperationsIncluded.Contains("D"),
                                     };
                    items.Add(preset);
                }
                foreach (var preset in items)
                {
                    var decks = repo.All.Where(x => x.PresetId == preset.Id && x.CardsAnswered > 0).OrderByDescending(x => x.DateStarted);
                    if (decks != null && decks.Count() > 0)
                    {
                        preset.HasBeenUsed = true;
                        preset.TimesTaken = decks.Count();
                        preset.AverageScore =
                            (int)decks.Average(x => (decimal)(100 * x.CardsCorrect) / (decimal)x.CardCount);
                        preset.HighScore = (int)decks.Max(x => (decimal)(100 * x.CardsCorrect) / (decimal)x.CardCount);
                        preset.LastUsed = decks.Max(x => x.DateStarted);
                        preset.Last10Score =
                            (int)decks.Take(10).Average(x => (decimal)(100 * x.CardsCorrect) / (decimal)x.CardCount);
                    }
                    else
                    {
                        preset.HasBeenUsed = false;
                    }
                }
            }
            items.Insert(0, new ItemViewModel { LastUsed = DateTime.Now.AddYears(1), Title = "Custom", ItemType = "newdeck", Id = userId });
            return items;
        }

        public ItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (value != null && value.ItemType == "newdeck")
                {
                    _selectedItem = null;
                }
                else
                {
                    //if (Equals(value, _selectedItem)) return;
                    _selectedItem = value;
                }
                NotifyOfPropertyChange(() => SelectedItem);
                NotifyOfPropertyChange(() => CanDeletePreset);
            }
        }

        public bool CanDeletePreset { get { return SelectedItem != null; } }

        public virtual void DeletePreset()
        {
        }

        public void OpenItem(ItemClickEventArgs eventArgs)
        {
            var item = eventArgs.ClickedItem as ItemViewModel;
            switch (item.ItemType)
            {
                case "newdeck":
                    NavigationService.NavigateToViewModel<NewDeckOptionsViewModel>(item.Id);
                    break;
                case "preset":
                    using (var repo = IoC.Get<IDeckRepository>())
                    {
                        var preset = repo.Find(item.Id);
                        var deck = Deck.FromPreset(preset);
                        repo.InsertOrUpdate(deck);
                        NavigationService.NavigateToViewModel<DeckViewModel>(deck.Id);
                    }
                    break;
                    //case "stat":
                    //    NavigationService.NavigateToViewModel<StatsViewModel>(item.Id);
                    //    break;
            }
        }
    }
}