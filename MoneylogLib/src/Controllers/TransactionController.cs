using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MoneylogLib.Models;
using MoneylogLib.Providers;

namespace MoneylogLib.Controllers
{
    internal class TransactionController
    {
        private readonly IStorageProvider _transactionStorage;

        public TransactionController(IStorageProvider transactionStorageProvider)
        {
            _transactionStorage = transactionStorageProvider;
        }

        public Transaction Create(DateTime timeStamp, TransactionType type, decimal amount, string note = null, string tags = null)
        {
            if (tags != null)
                tags = Regex.Replace(tags, @"\s+", "");
            
            var t = new Transaction
            {
                Date = timeStamp,
                Type = type,
                Amount = amount,
                Note = note,
                Tags = tags
            };

            _transactionStorage.Stage(t);

            return t;
        }

        public Transaction GetTransaction(int transactionId)
        {
            return _transactionStorage.Get(transactionId);
        }

        public List<Transaction> GetAllTransactions()
        {
            return _transactionStorage.GetAll().ToList();
        }

        public Transaction Edit(int transactionId, DateTime newTimeStamp, TransactionType newType, decimal newAmount,
            string newNote = null, string newTags = null)
        {
            return _transactionStorage.Edit(transactionId, newTimeStamp, newType, newAmount, newNote, newTags);
        }
        
        public List<Transaction> Remove(int id)
        {
            _transactionStorage.Remove(id);
            return GetAllTransactions();
        }

        public IEnumerable<Transaction> CommitAll()
        {
            _transactionStorage.CommitAll();  
            return GetAllTransactions();
        }

        public IEnumerable<Transaction> GetStagedTransactions()
        {
            return _transactionStorage.GetStaged();
        }
        
        public List<Transaction> UnstageAll()
        {
            _transactionStorage.UnstageAll();
            return GetAllTransactions();
        }
    }
}