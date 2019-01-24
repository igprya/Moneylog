using System;
using System.Collections.Generic;

using MoneylogLib.Helpers;
using MoneylogLib.Interfaces;
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

        public IEnumerable<ITransaction> FilterTransactions(IEnumerable<string> filterQueries)
        {
            var filters = new List<ITransactionFilter>();

            foreach (var query in filterQueries) {
                var f = FilterQueryParser.CreateFilter(query);
                filters.Add(f);
            }
            
            return _transactionController.Filter(filters);
        }

        public void Test()
        {
            var f = FilterQueryParser.CreateFilter("Note == test note");
            var filters = new List<ITransactionFilter> {f};
            var r = _transactionController.Filter(filters);
        }
    }
}