using System;
using System.Collections.Generic;

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
            string[] tags = null;

            if (tagString?.Length > 0)
            {
                tagString = tagString.Trim();
                tags = tagString.Split(',');
            }

            var t = new Transaction()
            {
                CreatedTimestampUtc = DateTime.UtcNow,
                TimestampUtc = timeStampUtc,
                Type = type,
                Amount = amount,
                Note = note,
                Tags = tags ?? null
            };

            _transactionStorage.Enqueue(t);

            return t;
        }

        public Transaction GetTransaction(int transactionId)
        {
            return _transactionStorage.Get(transactionId);
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return _transactionStorage.GetAll();
        }

        public IEnumerable<Transaction> Remove(int id)
        {
            _transactionStorage.Remove(id);
            return GetAllTransactions();
        }

        public IEnumerable<Transaction> Commit()
        {
            _transactionStorage.Commit();  
            return GetAllTransactions();
        }

        public IEnumerable<Transaction> DropQueue()
        {
            _transactionStorage.DropQueue();
            return GetAllTransactions();
        }

        public IEnumerable<ITransaction> Filter(IEnumerable<ITransactionFilter> filterList)
        {
            var result = GetAllTransactions();

            foreach (var filter in filterList)
            {
                result = filter.Apply(result);
            }

            return result;
        }

    }
}