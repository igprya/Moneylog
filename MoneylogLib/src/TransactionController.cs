using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MoneylogLib.Models;
using MoneylogLib.StorageProviders;

namespace MoneylogLib
{
    internal class TransactionController
    {
        private readonly ITransactionStorageProvider _transactionStorage;

        public TransactionController(ITransactionStorageProvider transactionStorageProvider)
        {
            _transactionStorage = transactionStorageProvider;
        }

        public Transaction Create(DateTime timeStamp, TransactionType type, decimal amount, string tagString = null, string note = null)
        {
            if (tagString != null)
                tagString = Regex.Replace(tagString, @"\s+", "");
            
            var t = new Transaction
            {
                CreatedTimestampUtc = DateTime.UtcNow,
                Timestamp = timeStamp,
                Type = type,
                Amount = amount,
                Note = note,
                Tags = tagString
            };

            _transactionStorage.Enqueue(t);

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
            string newTags = null, string newNote = null)
        {
            return _transactionStorage.Edit(transactionId, newTimeStamp, newType, newAmount, newTags, newNote);
        }
        
        public List<Transaction> Remove(int id)
        {
            _transactionStorage.Remove(id);
            return GetAllTransactions();
        }

        public IEnumerable<Transaction> Commit()
        {
            _transactionStorage.Commit();  
            return GetAllTransactions();
        }

        public IEnumerable<Transaction> GetPendingTransactions()
        {
            return _transactionStorage.GetPending();
        }
        
        public List<Transaction> DropQueue()
        {
            _transactionStorage.DropQueue();
            return GetAllTransactions();
        }
    }
}