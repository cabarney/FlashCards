using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Caliburn.Micro;
using FlashCards.Data;
using FlashCards.Extensions;
using FlashCards.Model;
using FlashCards.Results;
using Windows.UI.ViewManagement;

namespace FlashCards.ViewModels
{
    public class NewDeckOptionsViewModel : NavigationViewModelBase, INotifyDataErrorInfo
    {
        private Deck _deck;
        private bool _addition;
        private bool _subtraction;
        private bool _multiplication;
        private bool _division;
        private bool _focusOnTroubleSpots;
        private int _selectedAdditionOption;
        private int _selectedSubtractionOption;
        private int _selectedMultiplicationOption;
        private int _selectedDivisionOption;
        private bool _timed;

        public NewDeckOptionsViewModel(INavigationService navigation) : base(navigation)
        {
            _deck = new Deck();
        }

        public int SelectedCardCount
        {
            get { return _deck.CardCount; }
            set
            {
                _deck.CardCount = value;
                NotifyOfPropertyChange(()=>SelectedCardCount);
            }
        }

        public bool Addition
        {
            get { return _addition; }
            set
            {
                if (value.Equals(_addition)) return;
                _addition = value;
                NotifyOfPropertyChange(()=>Addition);
                SetOperationValues();
            }
        }

        public bool Subtraction
        {
            get { return _subtraction; }
            set
            {
                if (value.Equals(_subtraction)) return;
                _subtraction = value;
                NotifyOfPropertyChange(()=>Subtraction);
                SetOperationValues();
            }
        }

        public bool Multiplication
        {
            get { return _multiplication; }
            set
            {
                if (value.Equals(_multiplication)) return;
                _multiplication = value;
                NotifyOfPropertyChange(()=>Multiplication);
                SetOperationValues();
            }
        }

        public bool Division
        {
            get { return _division; }
            set
            {
                if (value.Equals(_division)) return;
                _division = value;
                NotifyOfPropertyChange(()=>Division);
                SetOperationValues();
            }
        }

        public bool FocusOnTroubleSpots
        {
            get { return _focusOnTroubleSpots; }
            set
            {
                if (value.Equals(_focusOnTroubleSpots)) return;
                _focusOnTroubleSpots = value;
                NotifyOfPropertyChange(()=>FocusOnTroubleSpots);
                SetOperationValues();
            }
        }

        public int SelectedAdditionOption
        {
            get { return _selectedAdditionOption; }
            set
            {
                if (value == _selectedAdditionOption) return;
                _selectedAdditionOption = value;
                NotifyOfPropertyChange(() => SelectedAdditionOption);
                SetOperationValues();
            }
        }

        public int SelectedSubtractionOption
        {
            get { return _selectedSubtractionOption; }
            set
            {
                if (value == _selectedSubtractionOption) return;
                _selectedSubtractionOption = value;
                NotifyOfPropertyChange(() => SelectedSubtractionOption);
                SetOperationValues();
            }
        }

        public int SelectedMultiplicationOption
        {
            get { return _selectedMultiplicationOption; }
            set
            {
                if (value == _selectedMultiplicationOption) return;
                _selectedMultiplicationOption = value;
                NotifyOfPropertyChange(() => SelectedMultiplicationOption);
                SetOperationValues();
            }
        }

        public int SelectedDivisionOption
        {
            get { return _selectedDivisionOption; }
            set
            {
                if (value == _selectedDivisionOption) return;
                _selectedDivisionOption = value;
                NotifyOfPropertyChange(() => SelectedDivisionOption);
                SetOperationValues();
            }
        }

        private bool OperationsAreValid()
        {
            return _subtraction || _addition || _multiplication || _division;
        }


        public List<int> CardCounts { get { return new List<int> { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 75, 80, 90, 100 }; } }
        public List<int> AdditionOptions { get { return new List<int> {10, 12, 20, 50, 100}; } }
        public List<int> SubtractionOptions { get { return new List<int> {10, 12, 20, 50, 100}; } }
        public List<int> MultiplicationOptions { get { return new List<int> {10, 12}; } }
        public List<int> DivisionOptions { get { return new List<int> {10, 12}; } }
        public List<int> MinuteOptions { get { return new List<int> {1,2,3,4,5,6,7,8,9,10,15,20,25,30,45,60}; } }
        public List<int> SecondOptions { get { return new List<int> {00,15,30,45}; } }

        private void SetOperationValues()
        {
            _deck.OperationsIncluded = "";
            if (Addition)
                _deck.OperationsIncluded += string.Format("[A{0}]", SelectedAdditionOption);
            if (Subtraction)
                _deck.OperationsIncluded += string.Format("[S{0}]", SelectedSubtractionOption);
            if (Multiplication)
                _deck.OperationsIncluded += string.Format("[M{0}]", SelectedMultiplicationOption);
            if (Division)
                _deck.OperationsIncluded += string.Format("[D{0}]", SelectedDivisionOption);
            if (FocusOnTroubleSpots)
                _deck.OperationsIncluded += "[T]";
        }

        public bool Timed
        {
            get { return _timed; }
            set
            {
                if (value.Equals(_timed)) return;
                _timed = value;
                NotifyOfPropertyChange(() => Timed);
            }
        }

        public int SelectedMinuteOption
        {
            get { return (int)Math.Floor((double)_deck.TimeLimit/60d); }
            set
            {
                _deck.TimeLimit = value * 60 + SelectedSecondOption;
                NotifyOfPropertyChange(()=>SelectedMinuteOption);
            }
        }

        public int SelectedSecondOption
        {
            get { return _deck.TimeLimit % 60; }
            set
            {
                if (value > 60)
                {
                    _deck.TimeLimit = value;
                    NotifyOfPropertyChange(() => SelectedMinuteOption);
                }
                else
                    _deck.TimeLimit = SelectedMinuteOption * 60 + value;
                NotifyOfPropertyChange(()=>SelectedSecondOption);
            }
        }

        public bool SaveAsPreset
        {
            get { return _deck.PresetOnly; }
            set
            {
                _deck.PresetOnly = value;
                NotifyOfPropertyChange(()=>SaveAsPreset);

                if (SaveAsPreset && string.IsNullOrWhiteSpace(PresetName))
                    PresetName = "New Preset";
            }
        }

        public string PresetName
        {
            get { return _deck.PresetName; }
            set
            {
                _deck.PresetName = value;
                NotifyOfPropertyChange(()=>PresetName);
            }
        }


        protected override void OnViewReady(object view)
        {
            DisplayName = "New Flash Card Deck";
            
            SelectedCardCount = 30;
            SelectedAdditionOption = 12;
            SelectedSubtractionOption = 12;
            SelectedMultiplicationOption = 12;
            SelectedDivisionOption = 12;
            SelectedSecondOption = 0;
            SelectedMinuteOption = 2;
            SaveAsPreset = false;
            Addition = true;
            Subtraction = true;
            Multiplication = true;
            Division = true;

            _deck.UserId = (int) Parameter;
        }

        public void StartDeck()
        {
            if(!OperationsAreValid())
            {
                Run.Coroutine(new IResult[] { new MessageDialogResult("Please select at least one operation for this deck (addition, subtraction, multiplication, division)", "At least one operation is required") });
                return;
            }
            if(SaveAsPreset && string.IsNullOrWhiteSpace(PresetName))
            {
                Run.Coroutine(new IResult[] { new MessageDialogResult("Please specify a name for this preset", "Preset name cannot be empty")});
                return;
            }

            _deck.DateStarted = DateTime.Now;

            if (!Timed)
                _deck.TimeLimit = 0;

            using(var repo = IoC.Get<IDeckRepository>())
            {
                if(_deck.PresetOnly)
                {
                    repo.InsertOrUpdate(_deck);
                    _deck.PresetOnly = false;
                    _deck.PresetId = _deck.Id;
                    _deck.Id = 0;
                }
                repo.InsertOrUpdate(_deck);
            }
            NavigationService.NavigateToViewModel<DeckViewModel>(_deck.Id);
        }



        private void WireUpAutomaticErrorNotifications()
        {
            PropertyChanged += (s,e)=> NotifyOfErrorsChanged(e.PropertyName);
        }

        // Navigation Parameter - UserId
        public int Parameter { get; set; }
        public IEnumerable GetErrors(string propertyName)
        {
            if((!_subtraction && !_addition && !_multiplication && !_division) 
                && new string[]{"Addition","Subtraction","Multiplication","Division"}.Contains(propertyName))
            {
                yield return "Must select at least one operation";
            }

            if (propertyName == "PresetName" && SaveAsPreset && string.IsNullOrWhiteSpace(PresetName))
                yield return "Preset name is required";
        }

        public bool HasErrors { get; private set; }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void NotifyOfErrorsChanged(string propertyName)
        {
            EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
            if (handler != null) handler(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable<IResult> SizeChanged(FrameworkElement e)
        {
            yield return new VisualStateResult(e.ActualWidth < 650 ? "Paused" : "Running", true);
        }
    }
}