using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.StorageProviders
{
    internal interface ITransactionStorageProvider
    {
        int Enqueue(Transaction transaction);
        IEnumerable<Transaction> GetAll();
        Transaction Get(int id);
        void Remove(int id);
        void Commit();
        void DropQueue();
    }
}