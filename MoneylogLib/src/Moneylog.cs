using System;
using System.Collections.Generic;
using MoneylogLib.Controllers;
using MoneylogLib.Models;
using MoneylogLib.Filtering;
using MoneylogLib.Providers;
using MoneylogLib.Reporting;

namespace MoneylogLib
{
    public class Moneylog
    {
        private readonly MoneylogSettings _settings;
        private readonly TransactionController _transactionController;

        public Moneylog(MoneylogSettings settings)
        {
            _settings = settings;
            _transactionController = new TransactionController( new JsonStorageProvider(_settings.StorageFilePath) );
        }

        public void AddTransaction(DateTime timeStamp, TransactionType type, decimal amount, string note = null, string tags = null)
        {
            _transactionController.Create(timeStamp, type, amount, note, tags);
        }
        
        public ITransaction EditTransaction(int transactionId, DateTime newTimeStamp, TransactionType newType, decimal newAmount, 
            string newNote = null, string newTags = null)
        {
            return _transactionController.Edit(transactionId, newTimeStamp, newType, newAmount, newNote, newTags);
        }

        public IEnumerable<ITransaction> GetTransactions(string filteringQuery = null)
        {
            var result = _transactionController.GetAllTransactions();

            if (!string.IsNullOrEmpty(filteringQuery))
                result = Filter.ExecuteQuery(result, filteringQuery).ConvertAll(c => (TransactionDto)c);

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
        
        public IEnumerable<ITransaction> GetStagedTransactions()
        {
            return _transactionController.GetStagedTransactions();
        }

        public IEnumerable<ITransaction> UnstageAllTransactions()
        {
            return _transactionController.UnstageAll();
        }
        
        public IEnumerable<ITransaction> CommitTransactions()
        {
            return _transactionController.CommitAll();
        }

        public Report GenerateReport(ReportType type, DateTime startDate, DateTime endDate, string filteringQuery)
        {
            IEnumerable<ITransaction> transactions = _transactionController.GetAllTransactions();
            
            return ReportGenerator.CreateReport(type,
                transactions,
                startDate,
                endDate,
                filteringQuery);
        }
    }
}