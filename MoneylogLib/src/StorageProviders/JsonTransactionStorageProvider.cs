using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoneylogLib.Interfaces;
using MoneylogLib.Models;

using Newtonsoft.Json;

namespace MoneylogLib.Providers
{
    class JsonTransactionStorageProvider : ITransactionStorageProvider
    {
       private Dictionary<int, Transaction> _transactionStorage = new Dictionary<int, Transaction>();
       private readonly string _storageFilePath;
       
        public JsonTransactionStorageProvider(string storageFilePath)
        {
            _storageFilePath = storageFilePath;
            ReadStorage();            
        }
        
        public int Enqueue(Transaction transaction)
        {
            int transactionId = GetNewId();
            transaction.Id = transactionId;
            
            _transactionStorage.Add(transactionId, transaction);
            
            return transactionId;
        }

        public IEnumerable<Transaction> GetAll()
        {
            return _transactionStorage.Count > 0 ? _transactionStorage.Values : null;
        }

        public Transaction Get(int id)
        {
            return _transactionStorage.ContainsKey(id) ? _transactionStorage[id] : null;
        }

        public void Remove(int id)
        {
            if (_transactionStorage.ContainsKey(id))
                _transactionStorage.Remove(id);
            else
                throw new ArgumentException($"A transaction with and Id of {id} does not exist.");
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
                        _transactionStorage.Add((int)transaction.Id, transaction);
                    
                    MakeAllTransactionsCommitted();
                }
                else 
                {
                    _transactionStorage = new Dictionary<int, Transaction>();
                }

            }
            catch (Exception e)
            {
                throw new IOException($"Unable to initialize transaction storage from file {_storageFilePath}");
            }
        }

        public void Commit()
        {
            var transactionList = _transactionStorage.Values.ToList();
        
            try
            {
                string storageFileContents = JsonConvert.SerializeObject(transactionList, Formatting.Indented);
                File.WriteAllText(_storageFilePath, storageFileContents);
                
                MakeAllTransactionsCommitted();
            }
            catch (Exception e)
            {
                throw new IOException($"Unable to commit transactions into file {_storageFilePath}");
            }
        }

        public void DropQueue()
        {
            var uncommittedTransactionIds = _transactionStorage.Where(t => !t.Value.Committed).Select(t => t.Key);

            foreach (var id in uncommittedTransactionIds)
                _transactionStorage.Remove(id);
        }

        private int GetNewId()
        {
            if (_transactionStorage.Count == 0) return 0;
            int? lastId = _transactionStorage.Last().Key;
            return lastId + 1 ?? 0;
        }

        private void MakeAllTransactionsCommitted()
        {
            foreach (var IdTransactionPair in _transactionStorage)
                if (!IdTransactionPair.Value.Committed)
                    IdTransactionPair.Value.Committed = true;
        }
        
    }
}