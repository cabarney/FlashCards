using System;

namespace FlashCards.Model
{
    public class Answer : Entity
    {
        public int UserId { get; set; }
        
        public int Operand1 { get; set; }
        public int Operand2 { get; set; }
        public int Resultant { get; set; }
        public string Operation { get; set; }

        public int NumberCorrect { get; set; }
        public int NumberAnswered { get; set; }
    }
}