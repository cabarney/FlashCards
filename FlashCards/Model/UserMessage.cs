namespace FlashCards.Model
{
    public class UserMessage
    {
        public UserMessage(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; set; }
    }
}