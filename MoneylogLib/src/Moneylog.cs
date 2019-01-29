using System;
using System.Collections.Generic;
using MoneylogLib.Helpers;
using MoneylogLib.Models;
using MoneylogLib.StorageProviders;

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

        public ITransaction GetTransaction(int transactionId)
        {
            return _transactionController.GetTransaction(transactionId);
        }

        public ITransaction EditTransaction(int transactionId, DateTime newTimeStamp, TransactionType newType, decimal newAmount, 
            string newTags = null, string newNote = null)
        {
            return _transactionController.Edit(transactionId, newTimeStamp, newType, newAmount, newTags, newNote);
        }
        
        public IEnumerable<ITransaction> FilterTransactions(string filteringQuery)
        {
            return _transactionController.Filter(filteringQuery);
        }

        public IEnumerable<ITransaction> CommitTransactions()
        {
            return _transactionController.Commit();
        }
    }
}