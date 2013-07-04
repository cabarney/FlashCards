namespace FlashCards.Model
{
    public class Entity
    {
        [SQLite.AutoIncrement, SQLite.PrimaryKey]
        public int Id { get; set; }
    }
}