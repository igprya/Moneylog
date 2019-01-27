using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Operations;
using MoneylogLib.Helpers;
using MoneylogLib.Models;
using MoneylogLib.Providers;

namespace MoneylogLib
{
    public class Moneylog
    {
        private MoneylogSettings _settings;
        private TransactionController _transactionController;

        public Moneylog(MoneylogSettings settings)
        {
            _settings = settings;
            _transactionController = new TransactionController( new JsonTransactionStorageProvider(_settings.StorageFilePath) );
        }

        public void AddTransaction(DateTime timeStamp, TransactionType type, decimal amount, string tags = null, string note = null)
        {
            _transactionController.Create(timeStamp, type, amount, tags, note);
        }

        public IEnumerable<ITransaction> GetAllTransactions()
        {
            return _transactionController.GetAllTransactions();
        }

        public IEnumerable<ITransaction> FilterTransactions(string filteringQuery)
        {
            return _transactionController.Filter(filteringQuery);
        }

        public IEnumerable<ITransaction> CommitTransactions()
        {
            return _transactionController.Commit();
        }

        public void Test()
        {
            var f1 = FilterTransactions("Amount == 600 OR Amount >= 1000 AND Tags == lol");
        }
    }
}