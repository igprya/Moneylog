using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoneylogLib.Models;
using Newtonsoft.Json;

namespace MoneylogLib.Providers
{
    internal class JsonStorageProvider : IStorageProvider
    {
       private Dictionary<int, Transaction> _transactionStorage = new Dictionary<int, Transaction>();
       private readonly string _storageFilePath;
       
        public JsonStorageProvider(string storageFilePath)
        {
            _storageFilePath = storageFilePath;
            ReadStorage();            
        }
        
        public IEnumerable<Transaction> GetAll()
        {
            return _transactionStorage.Count > 0 ? _transactionStorage.Values : null;
        }

        public IEnumerable<Transaction> GetStaged()
        {
            return _transactionStorage.Count > 0 ? _transactionStorage.Values.Where(t => t.Committed == false) : null;
        }

        public Transaction Get(int id)
        {
            return _transactionStorage.ContainsKey(id) ? _transactionStorage[id] : null;
        }

        public Transaction Edit(int transactionId, DateTime newTimeStamp, TransactionType newType, decimal newAmount, 
            string newNote = null, string newTags = null)
        {
            var transaction = Get(transactionId);

            if (transaction != null)
            {
                transaction.Committed = false;
                transaction.Date = newTimeStamp;
                transaction.Type = newType;
                transaction.Amount = newAmount;
                transaction.Tags = newTags;
                transaction.Note = newNote;

                return transaction;
            }

            throw new ArgumentException($"A transaction with an Id {transactionId} doesn't exist.");
        }

        public void Remove(int id)
        {
            if (_transactionStorage.ContainsKey(id))
            {
                if (_transactionStorage[id].Committed)
                {
                    _transactionStorage[id].Deleted = true;
                    _transactionStorage[id].Committed = false;
                }
                else
                {
                    _transactionStorage.Remove(id);
                }
            }
            else
            {
                throw new ArgumentException($"A transaction with and Id of {id} does not exist.");
            }
        }    
        
        public int Stage(Transaction transaction)
        {
            int transactionId = GetNewId();
            transaction.Id = transactionId;
            transaction.Committed = false;
            
            _transactionStorage.Add(transactionId, transaction);
            
            return transactionId;
        }
        
        public void UnstageAll()
        {
            var stagedTransactions = _transactionStorage.Where(t => !t.Value.Committed && !t.Value.Deleted).Select(t => t.Key).ToArray();
            var deletedTransactions = _transactionStorage.Where(t => !t.Value.Committed && t.Value.Deleted).Select(t => t.Value).ToArray();

            foreach (var id in stagedTransactions)
            {
                _transactionStorage.Remove(id);
            }

            foreach (var t in deletedTransactions)
            {
                t.Deleted = false;
                t.Committed = true;
            }
        }
        
        public void CommitAll()
        {
            var deletedTransactionIds = _transactionStorage.Where(t => !t.Value.Committed && t.Value.Deleted).Select(t => t.Key).ToArray();
            foreach (var id in deletedTransactionIds)
            {
                _transactionStorage.Remove(id);
            }

            var transactionList = _transactionStorage.Values.OrderBy(t => t.Date).ToList();

            try
            {
                string storageFileContents = JsonConvert.SerializeObject(transactionList, Formatting.Indented);
                File.WriteAllText(_storageFilePath, storageFileContents);
                
                MakeAllTransactionsCommitted();
            }
            catch (Exception e)
            {
                throw new IOException($"Unable to commit transactions into file {_storageFilePath}. Error: {e}");
            }
        }

        private void ReadStorage()
        {
            try
            {
                if (File.Exists(_storageFilePath)) 
                {    
                    string storageFileContents = File.ReadAllText(_storageFilePath);
                    var transactionList = JsonConvert.DeserializeObject<List<Transaction>>(storageFileContents);

                    foreach (var transaction in transactionList)
                    {
                        _transactionStorage.Add((int) transaction.Id, transaction);
                    }

                    MakeAllTransactionsCommitted();
                }
                else 
                {
                    _transactionStorage = new Dictionary<int, Transaction>();
                }

            }
            catch (Exception e)
            {
                throw new IOException($"Unable to initialize transaction storage from file {_storageFilePath}. Error: {e}.");
            }
        }
        
        private int GetNewId()
        {
            if (_transactionStorage.Count == 0) return 0;
            int lastId = _transactionStorage.Last().Key;
            return lastId + 1;
        }

        private void MakeAllTransactionsCommitted()
        {
            foreach (var IdTransactionPair in _transactionStorage)
            {
                if (!IdTransactionPair.Value.Committed)
                {
                    IdTransactionPair.Value.Committed = true;
                }
            }
        }
        
    }
}