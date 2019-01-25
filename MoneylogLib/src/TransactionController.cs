using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Filters;
using MoneylogLib.Interfaces;
using MoneylogLib.Models;

namespace MoneylogLib
{
    class TransactionController
    {
        private readonly ITransactionStorageProvider _transactionStorage;

        public TransactionController(ITransactionStorageProvider transactionStorageProvider)
        {
            _transactionStorage = transactionStorageProvider;
        }

        public Transaction Create(DateTime timeStampUtc, TransactionType type, decimal amount, string tagString = null, string note = null)
        {
            // Remove spaces from tags
            tagString = tagString?.Replace(' ', '');
            
            var t = new Transaction()
            {
                CreatedTimestampUtc = DateTime.UtcNow,
                TimestampUtc = timeStampUtc,
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

        public List<Transaction> Remove(int id)
        {
            _transactionStorage.Remove(id);
            return GetAllTransactions();
        }

        public List<Transaction> Commit()
        {
            _transactionStorage.Commit();  
            return GetAllTransactions();
        }

        public List<Transaction> DropQueue()
        {
            _transactionStorage.DropQueue();
            return GetAllTransactions();
        }

        public List<Transaction> Filter(string filteringQuery)
        {
            return FilterQueryExecutor.ExecuteQuery(GetAllTransactions(), filteringQuery);
        }
    }
}