using System;
using System.Collections.Generic;
using FlashCards.Model;

namespace FlashCards.Data
{
    public interface IRepository<T> : IDisposable where T : Entity
    {
        T Find(int id);
        IEnumerable<T> All { get; }
        int InsertOrUpdate(T item);
        void Delete(int id);
        void Delete(T item);
    }
}