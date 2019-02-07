using System;
using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Providers
{
    internal interface IStorageProvider
    {
        
        IEnumerable<Transaction> GetAll();
        Transaction Get(int id);
        Transaction Edit(int id, Transaction transaction);
        void Remove(int id);
        int Stage(Transaction transaction);
        IEnumerable<Transaction> GetStaged();
        void ClearStaged();
        void CommitAll();
    }
}