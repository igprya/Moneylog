using System;
using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.StorageProviders
{
    internal interface ITransactionStorageProvider
    {
        
        IEnumerable<Transaction> GetAll();
        IEnumerable<Transaction> GetPending();
        Transaction Get(int id);
        Transaction Edit(int transactionId, DateTime newTimeStamp, TransactionType newType, decimal newAmount,
            string newTags = null, string newNote = null);
        void Remove(int id);
        int Enqueue(Transaction transaction);
        void DropQueue();
        void Commit();
    }
}