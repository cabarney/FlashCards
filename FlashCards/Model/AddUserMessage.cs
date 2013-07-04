namespace FlashCards.Model
{
    public class AddUserMessage:UserMessage
    {
        public AddUserMessage(int userId) : base(userId)
        {
        }
    }
}