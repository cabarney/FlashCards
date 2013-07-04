using System;
using System.IO;
using SQLite;

namespace FlashCards.Data
{
    public abstract class SqliteDbComponent : IDisposable
    {
        protected SQLite.SQLiteConnection Db;

        protected SqliteDbComponent()
        {
            Db = OpenConnection();
        }

        protected SQLiteConnection OpenConnection()
        {
            var dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "flashcards.sqlite");
            return new SQLiteConnection(dbPath);
        }

        public virtual void Dispose()
        {
            Db.Dispose();
        }
    }
}