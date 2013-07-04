using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using FlashCards.Data;
using SQLite;

namespace FlashCards.Model
{
    public class Deck : Entity
    {
        public int CardCount { get; set; }
        public int TimeLimit { get; set; }
        public int UserId { get; set; }
        public DateTime DateStarted { get; set; }
        public int CardsAnswered { get; set; }
        public int CardsCorrect { get; set; }
        public string OperationsIncluded { get; set; }
        public bool PresetOnly { get; set; }
        public int PresetId { get; set; }
        public string PresetName { get; set; }
        [Ignore]
        public List<Card> Cards { get; set; }

        public static Deck FromPreset(Deck preset)
        {
            var deck = new Deck
                           {
                               CardCount = preset.CardCount,
                               TimeLimit = preset.TimeLimit,
                               UserId = preset.UserId,
                               DateStarted = DateTime.Now,
                               OperationsIncluded = preset.OperationsIncluded,
                               PresetOnly = false,
                               PresetId = preset.Id,
                               PresetName = preset.PresetName
                           };
            return deck;
        }

        public void GenerateCards()
        {
            Cards = new List<Card>();

            var r = new Random((int) DateTime.Now.Ticks);

            var mathFacts = new List<MathFact>();

            if(OperationsIncluded.Contains("A"))
            {
                var num = GetNumberFromOperationsString("A");
                for (int i = 0; i <= num; i++)
                    for (int j = 0; j <= num; j++)
                    {
                        if (!mathFacts.Any(x => x.Operand1 == i && x.Operand2 == j && x.Operation=="+"))
                            mathFacts.Add(new MathFact {Operand1 = i, Operand2 = j, Operation = "+", Resultant = i + j});
                    }
            }

            if(OperationsIncluded.Contains("S"))
            {
                var num = GetNumberFromOperationsString("S");
                for (int i = 1; i <= num * 2; i++)
                    for (int j = 0; j <= Math.Min(num,i); j++)
                        if(!mathFacts.Any(x => x.Operand1 == i && x.Operand2 == j && x.Operation==((char) 45).ToString()))
                            mathFacts.Add(new MathFact{Operand1 = i,Operand2 = j,Operation = ((char) 45).ToString(),Resultant = i - j});
            }

            if(OperationsIncluded.Contains("M"))
            {
                var num = GetNumberFromOperationsString("M");
                for (int i = 0; i <= num; i++)
                    for (int j = 0; j <= num; j++)
                        if (!mathFacts.Any(x => x.Operand1 == i && x.Operand2 == j && x.Operation == ((char)215).ToString()))
                            mathFacts.Add(new MathFact
                                              {Operand1 = i, Operand2 = j, Operation = ((char) 215).ToString(), Resultant = i*j});
            }

            if(OperationsIncluded.Contains("D"))
            {
                var num = GetNumberFromOperationsString("D");
                for (int i = 0; i <= num; i++)
                    for (int j = 1; j <= num; j++)
                        if (!mathFacts.Any(x => x.Operand1 == i && x.Operand2 == j && x.Operation == ((char)247).ToString()))
                            mathFacts.Add(new MathFact {Operand1 = i*j, Operand2 = j, Resultant = i, Operation = ((char) 247).ToString()});
            }

            var useHistory = OperationsIncluded.Contains("T");
            List<Answer> history = null;
            if (useHistory)
            {
                using (var repo = IoC.Get<IAnswerRepository>())
                {
                    history = repo.All.Where(x => x.UserId == UserId).ToList();
                }
            }

            foreach (var fact in mathFacts)
                fact.Priority = GetPriority(history, fact, useHistory, r);
            mathFacts = mathFacts.OrderBy(x => x.Priority).ToList();
            int idx = 0;
            for (int i = 0; i < CardCount;i++)
            {
                if (idx >= mathFacts.Count)
                    idx = 0;
                var card = new Card {MathFact = mathFacts[idx]};
                
                var answers = new int[4];
                answers[0] = mathFacts[idx].Resultant;

                for (var j = 1; j <= 3;j++)
                {
                    var answer = GetOffByOneWrongAnswer(card, r);
                    while (answers.Contains(answer))
                        answer++;
                    answers[j] = answer;
                }

                card.Answers = Reorderanswers(answers, r);
                Cards.Add(card);
                idx++;
            }
        }

        private int GetOffByOneWrongAnswer(Card card, Random r)
        {
            int answer;
            int diff = 1;
            var factor = r.Next(21) - 10;
            if (Math.Abs(factor) > 5)
                diff = 2;
            if (Math.Abs(factor) > 8)
                diff = 3;
            diff = factor < 0 ? diff*-1 : diff;
            if (card.MathFact.Operation == ((char) 215).ToString())
            {
                if (r.Next(10) > 5)
                {
                    var op1 = Math.Max(card.MathFact.Operand1 + diff, 0);
                    answer = MathFact.CalculateResult(op1, card.MathFact.Operand2, card.MathFact.Operation);
                }
                else
                {
                    var op2 = Math.Max(card.MathFact.Operand1 + diff, 0);
                    answer = MathFact.CalculateResult(op2, card.MathFact.Operand1, card.MathFact.Operation);
                }
            }
            else
            {
                var result = Math.Max(card.MathFact.Resultant + diff, 0);
                if (result == card.MathFact.Resultant)
                    result++;
                answer = result;
            }
            return answer;
        }


        private int[] Reorderanswers(IEnumerable<int> answers, Random r)
        {
            var reordered = answers.ToDictionary(answer => answer, answer => r.Next());
            return reordered.OrderBy(x => x.Value).Select(x => x.Key).ToArray();
        }

        private int GetNumberFromOperationsString(string operation)
        {
            var startIdx = OperationsIncluded.IndexOf(operation);
            if (startIdx < 0)
                return 0;
            var endIdx = OperationsIncluded.IndexOf("]", startIdx);
            return int.Parse(OperationsIncluded.Substring(startIdx+1, endIdx-startIdx-1));
        }


        private int GetPriority(IEnumerable<Answer> history, MathFact fact, bool useHistory, Random r)
        {
            var priority = 500 + r.Next(250);

            if(useHistory)
            {
                string op = fact.Operation;
                if(op == ((char) 45).ToString()) op = "-";
                if(op == ((char) 215).ToString()) op = "*";
                if(op == ((char) 247).ToString()) op = "/";

                var answer = history.SingleOrDefault(x => x.Operand1 == fact.Operand1 && x.Operand2 == fact.Operand2 && x.Operation == op);

                if(answer != null)
                {
                    var percentage = (decimal)answer.NumberCorrect/(decimal)answer.NumberAnswered;
                    percentage = percentage - 0.85m;
                    percentage = percentage*Math.Min(20,answer.NumberAnswered);

                    priority += (int)(percentage*(r.Next(25)));
                }
            }

            return priority;
        }
    }
}