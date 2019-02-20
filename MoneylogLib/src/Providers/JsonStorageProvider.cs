using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using MoneylogLib.Models;
using Newtonsoft.Json;

namespace MoneylogLib.Providers
{
    internal class JsonStorageProvider : IStorageProvider
    {
       private Dictionary<int, Transaction> _transactionStorage = new Dictionary<int, Transaction>();
       private readonly List<int> _stagedIds = new List<int>();
       private readonly List<int> _deletedIds = new List<int>();
       
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
            return _transactionStorage.Count > 0 ? _transactionStorage.Values.Where(t => _stagedIds.Contains((int) t.Id)) : null;
        }

        public Transaction Get(int id)
        {
            return _transactionStorage.ContainsKey(id) ? _transactionStorage[id] : null;
        }

        public Transaction Edit(int id, Transaction transaction)
        {
            if (!_transactionStorage.ContainsKey(id))
                throw new ArgumentException($"The transaction with an id {id} does not exist.");

            _transactionStorage[id] = transaction;
            return _transactionStorage[id];
        }

        public void Remove(int id)
        {
            if (_transactionStorage.ContainsKey(id))
            {
                if (!_stagedIds.Contains(id)) {
                    _stagedIds.Add(id);
                }
                else {
                    _stagedIds.Remove(id);
                }

                _deletedIds.Add(id);
                return;
            }
            
            throw new ArgumentException($"A transaction with and Id of {id} does not exist.");
        }    
        
        public int Stage(Transaction transaction)
        {
            int id = GetNewId();
            
            var t = new Transaction(id, transaction.Date, transaction.Type, transaction.Amount, transaction.Note, transaction.Tags);
            
            _transactionStorage.Add(id, t);
            _stagedIds.Add(id);
            
            return id;
        }
        
        public void CommitAll()
        {
            foreach (var id in _deletedIds)
                _transactionStorage.Remove(id);

            var transactionList = _transactionStorage.Values.OrderBy(t => t.Date).ToList();

            try
            {
                string storageFileContents = JsonConvert.SerializeObject(transactionList, Formatting.Indented);
                File.WriteAllText(_storageFilePath, storageFileContents);
                
                ClearStaged();
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

                    ClearStaged();
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
        
        public void ClearStaged()
        {
            _stagedIds.Clear();
            _deletedIds.Clear();
        }
    }
}