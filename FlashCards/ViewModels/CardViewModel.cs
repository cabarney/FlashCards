using System.Collections.Generic;
using Caliburn.Micro;
using FlashCards.Model;
using FlashCards.Results;

namespace FlashCards.ViewModels
{
    public class CardViewModel : PropertyChangedBase
    {
        public Card Card { get; set; }
        private readonly bool _isTimed;
        private readonly IEventAggregator _events;
        private AnswerState _answer1State;
        private AnswerState _answer2State;
        private AnswerState _answer3State;
        private AnswerState _answer4State;

        public CardViewModel(Card card, bool isTimed, IEventAggregator events)
        {
            Card = card;
            _isTimed = isTimed;
            _events = events;
        }

        public int Operand1 { get  { return Card.MathFact.Operand1; } }
        public int Operand2 { get  { return Card.MathFact.Operand2; } }
        public string Operation { get { return Card.MathFact.Operation; } }
        public int Resultant { get { return Card.MathFact.Resultant; } }

        public int Answer1 { get { return Card.Answers[0]; } }
        public int Answer2 { get { return Card.Answers[1]; } }
        public int Answer3 { get { return Card.Answers[2]; } }
        public int Answer4 { get { return Card.Answers[3]; } }

        private bool _answerChosen;

        public IEnumerable<IResult> SelectAnswer(int answer)
        {
            if (_answerChosen)
                yield break;
            _answerChosen = true;
            int idx;
            for (idx = 1; idx < 4 ; idx++)
            {
                if (Card.Answers[idx-1] == answer)
                    break;
            }
            
            yield return new ControlVisualStateResult("AnswerChosen", "Answer"+idx, !_isTimed);
            for (int i = 1; i <= 4; i++ )
                if(idx != i)
                    yield return new ControlVisualStateResult("AnswerHidden", "Answer" + i, !_isTimed);
            yield return new VisualStateResult("Selected", !_isTimed);
            if (!_isTimed) yield return new DelayResult(1000);
            yield return new VisualStateResult("BackVisible", !_isTimed);
            if (!_isTimed) yield return new DelayResult(1000);
            if(answer == Card.MathFact.Resultant)
                yield return new ControlVisualStateResult("AnswerCorrect", "Answer" + idx, !_isTimed);
            else
                yield return new ControlVisualStateResult("AnswerIncorrect", "Answer" + idx, !_isTimed);

            _events.Publish(new AnswerChosenMessage(answer));
        }
    }
}