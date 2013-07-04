namespace FlashCards.Model
{
    public class RemoveUserMessage:UserMessage
    {
        public RemoveUserMessage(int userId) : base(userId)
        {
        }
    }
}