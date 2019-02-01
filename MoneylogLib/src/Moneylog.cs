using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Helpers;
using MoneylogLib.Models;
using MoneylogLib.StorageProviders;
using MoneylogLib.Filtering;

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
        
        public ITransaction EditTransaction(int transactionId, DateTime newTimeStamp, TransactionType newType, decimal newAmount, 
            string newTags = null, string newNote = null)
        {
            return _transactionController.Edit(transactionId, newTimeStamp, newType, newAmount, newTags, newNote);
        }

        public IEnumerable<ITransaction> GetTransactions(string query = null)
        {
            var result = _transactionController.GetAllTransactions();

            if (!string.IsNullOrEmpty(query))
                result = Filter.ExecuteQuery(result, query);

            return result;
        }

        public ITransaction GetTransaction(int transactionId)
        {
            return _transactionController.GetTransaction(transactionId);
        }

        public void RemoveTransaction(int transactionId)
        {
            _transactionController.Remove(transactionId);
        }
        
        public IEnumerable<ITransaction> GetPendingTransactions()
        {
            return _transactionController.GetPendingTransactions().OrderBy(o => o.Timestamp);
        }

        public IEnumerable<ITransaction> DropQueue()
        {
            return _transactionController.DropQueue();
        }
        
        public IEnumerable<ITransaction> CommitTransactions()
        {
            return _transactionController.Commit();
        }
    }
}