using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Caliburn.Micro;
using FlashCards.Data;
using FlashCards.Extensions;
using FlashCards.Model;
using FlashCards.Results;
using Windows.UI.ViewManagement;

namespace FlashCards.ViewModels
{
    public class DeckViewModel : NavigationConductorOneActive<CardViewModel>, IHandle<AnswerChosenMessage>
    {
        private Deck _deck;

        private readonly IEventAggregator _events;

        public DeckViewModel(IEventAggregator events, INavigationService navigation) : base(navigation)
        {
            _events = events;
            _events.Subscribe(this);
        }

        protected override void OnViewReady(object view)
        {
            using (var repo = IoC.Get<IDeckRepository>())
            {
                _deck = repo.Find(Parameter);
            }

            using (var repo = IoC.Get<IUserRepository>())
            {
                DisplayName = repo.Find(_deck.UserId).Name + ": ";
                if(!string.IsNullOrWhiteSpace(_deck.PresetName))
                    DisplayName += _deck.PresetName;
                else
                {
                    var title = "";
                    if (_deck.OperationsIncluded.Contains("A"))
                        title += "Addition, ";
                    if (_deck.OperationsIncluded.Contains("S"))
                        title += "Subtraction, ";
                    if (_deck.OperationsIncluded.Contains("M"))
                        title += "Multiplication, ";
                    if (_deck.OperationsIncluded.Contains("D"))
                        title += "Division, ";
                    var idx = title.LastIndexOf(",");
                    title = title.Substring(0, idx);
                    DisplayName += title;
                }
            }

            
            _deck.GenerateCards();
            
            foreach (var card in _deck.Cards)
                Items.Add(new CardViewModel(card, TimedMode, _events));

            ActivateItem(Items[0]);
            
            CorrectAnswers = new BindableCollection<MathFact>();
            IncorrectAnswers = new BindableCollection<MathFact>();

            if (TimedMode)
            {
                _secondsRemaining = _deck.TimeLimit;
                NotifyOfPropertyChange(()=>Minutes);
                NotifyOfPropertyChange(()=>Seconds);
                StartTimer();
            }
            NotifyOfPropertyChange(()=>TimedMode);
        }

        public BindableCollection<MathFact> CorrectAnswers
        {
            get { return _correctAnswers; }
            set
            {
                if (Equals(value, _correctAnswers)) return;
                _correctAnswers = value;
                NotifyOfPropertyChange(() => CorrectAnswers);
            }
        }

        public BindableCollection<MathFact> IncorrectAnswers
        {
            get { return _incorrectAnswers; }
            set
            {
                if (Equals(value, _incorrectAnswers)) return;
                _incorrectAnswers = value;
                NotifyOfPropertyChange(() => IncorrectAnswers);
            }
        }

        public async void Handle(AnswerChosenMessage message)
        {
            bool isCorrect = ActiveItem.Card.MathFact.Resultant == message.Answser;

            using(var repo = IoC.Get<IAnswerRepository>())
            {
                var answer = repo.All.SingleOrDefault(x =>
                        x.Operand1 == ActiveItem.Operand1 && x.Operand2 == ActiveItem.Operand2 && 
                        x.Operation == ActiveItem.Operation);
                if (answer == null)
                {
                    answer = new Answer
                                 {
                                     Operand1 = ActiveItem.Operand1,
                                     Operand2 = ActiveItem.Operand2,
                                     Operation = ActiveItem.Operation,
                                     Resultant = ActiveItem.Resultant,
                                     UserId = _deck.UserId,
                                     NumberAnswered = 1,
                                     NumberCorrect = isCorrect ? 1 : 0
                                 };
                }
                else
                {
                    answer.NumberAnswered++;
                    if (isCorrect) answer.NumberCorrect++;
                }
                repo.InsertOrUpdate(answer);
            }

            _deck.CardsAnswered++;
            if (isCorrect)
                _deck.CardsCorrect++;

            await Task.Delay(1000);
            
            if (isCorrect)
            {
                CorrectAnswers.Insert(0, ActiveItem.Card.MathFact);
            }
            else
            {
                IncorrectAnswers.Insert(0, ActiveItem.Card.MathFact);
            }

            
            if(_deck.CardsAnswered==_deck.CardCount)
                Run.Coroutine(ShowResults());
            else
                ChangeActiveItem(Items[_deck.CardsAnswered], true);
        }


        public string GetResultMessage()
        {
            var result = _deck.CardsCorrect + " correct out of " + _deck.CardsAnswered;
            if (TimedMode)
            {
                if (_secondsRemaining == 0)
                {
                    if (_deck.CardsAnswered < _deck.CardCount)
                        result += " (" + (_deck.CardCount - _deck.CardsAnswered) + " unanswered)";
                }
                else
                {
                    result += " (";
                    if (Minutes != "0")
                        result += Minutes + " minutes";
                    if (Seconds != "00")
                        result += " " + Seconds + " seconds";
                    result += " remaining )";
                }
            }
            result += " : " + Math.Ceiling((decimal) (_deck.CardsCorrect*100)/(decimal) _deck.CardCount) + "%";

            return result;
        }


        public IEnumerable<IResult> ShowResults()
        {
            StopTimer();

            using (var repo = IoC.Get<IDeckRepository>())
            {
                repo.InsertOrUpdate(_deck);
            }
            

            var messageBox = new MessageDialogResult(GetResultMessage(), "You're finished!");
            messageBox.Completed += (s, e) => NavigationService.NavigateToViewModel<MainViewModel>();
            yield return messageBox;
            _events.Unsubscribe(this);
        }

        public bool TimedMode
        {
            get { return _deck != null && _deck.TimeLimit > 0; }
        }

        private int _secondsRemaining;
        private BindableCollection<MathFact> _correctAnswers;
        private BindableCollection<MathFact> _incorrectAnswers;


        private bool isTimerRunning;

        private async void StartTimer()
        {
            this.isTimerRunning = true;

            while (this.isTimerRunning)
            {
                await Task.Delay(1000);
                OnOneSecondPassed();
            }
        }

        private void StopTimer()
        {
            this.isTimerRunning = false;
        }

        protected virtual void OnOneSecondPassed()
        {
            _secondsRemaining--;
            NotifyOfPropertyChange(() => Minutes);
            NotifyOfPropertyChange(() => Seconds);
            if(_secondsRemaining == 0 )
            {
                StopTimer();

                Run.Coroutine(ShowResults());
            }
        }


        public string Minutes
        {
            get { return ((int)(_secondsRemaining / 60d)).ToString(); }
        }

        public string Seconds
        {
            get
            {
                var seconds = (_secondsRemaining%60).ToString().Trim();
                if(seconds.Length==1)
                    seconds = "0" + seconds;
                return seconds;
            }
        }

        public override void GoBack()
        {
            if (_deck.CardsAnswered == 0)
                DeleteAndGoBack();
            else
                Run.Coroutine(GoBackCoroutine());
        }

        private IEnumerable<IResult> GoBackCoroutine()
        {
            var cmds = new List<MessageDialogCommand>
                           {
                               new MessageDialogCommand("Yes", x => DeleteAndGoBack()),
                               new MessageDialogCommand("No", x => { })
                           };
            yield return new MessageDialogResult("Are you sure you want to go back?  You will lose all your progress!", "Are you sure?", cmds);
        }

        private void DeleteAndGoBack()
        {
            _events.Unsubscribe(this);
            using (var repo = IoC.Get<IDeckRepository>())
            {
                repo.Delete(_deck.Id);
            }
            StopTimer();
            NavigationService.NavigateToViewModel<MainViewModel>();
        }

        // Navigation Parameter
        public int Parameter { get; set; }

        public IEnumerable<IResult> SizeChanged(FrameworkElement e)
        {
            if(e.ActualWidth < 650)
                StopTimer();
            else
                StartTimer();
            yield return new VisualStateResult(e.ActualWidth<650 ? "Paused" : "Running", true);
        }
    }
}
