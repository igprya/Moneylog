using System;
using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Providers
{
    internal interface IStorageProvider
    {
        
        IEnumerable<Transaction> GetAll();
        Transaction Get(int id);
        Transaction Edit(int transactionId, DateTime newTimeStamp, TransactionType newType, decimal newAmount, string newNote = null, string newTags = null);
        void Remove(int id);
        int Stage(Transaction transaction);
        IEnumerable<Transaction> GetStaged();
        void UnstageAll();
        void CommitAll();
    }
}