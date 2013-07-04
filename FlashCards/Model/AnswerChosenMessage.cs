namespace FlashCards.Model
{
    public class AnswerChosenMessage
    {
        public AnswerChosenMessage(int answer)
        {
            Answser = answer;
        }

        public int Answser { get; set; }
    }
}