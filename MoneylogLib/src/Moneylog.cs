using System;
using System.Collections.Generic;
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

        public void AddTransaction()
        {
            _transactionController.Create(DateTime.UtcNow, TransactionType.Income, 500, "kek,quack", "none");
            _transactionController.Commit();
        }

        public IEnumerable<ITransaction> GetAllTransactions()
        {
            return _transactionController.GetAllTransactions();
        }

        public IEnumerable<ITransaction> FilterTransactions(string filteringQuery)
        {
            return _transactionController.Filter(filteringQuery);
        }

        public void Test()
        {
            var f1 = FilterTransactions("Amount == 600 OR Amount >= 1000 AND Tags == lol");
        }
    }
}