using FlashCards.Model;

namespace FlashCards.Data
{
    public class SqliteDbInitializer : SqliteDbComponent
    {
        public void Initialize()
        {
            Db.CreateTable<User>();
            Db.CreateTable<Answer>();
            Db.CreateTable<Deck>();
        }
    }
}