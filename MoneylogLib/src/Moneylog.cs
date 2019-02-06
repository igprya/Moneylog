using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoneylogLib.Controllers;
using MoneylogLib.Models;
using MoneylogLib.Filtering;
using MoneylogLib.Providers;
using MoneylogLib.Reporting;

namespace MoneylogLib
{
    public class Moneylog
    {
        private readonly Settings _settings;
        private readonly TransactionController _transactionController;

        public Moneylog(Settings settings)
        {
            _settings = settings;
            _transactionController = new TransactionController( new JsonStorageProvider(_settings.StorageFilePath) );
        }

        public Task AddTransaction(DateTime timeStamp, TransactionType type, decimal amount, string note = null, string tags = null)
        {
            return Task.Run(() => { _transactionController.Create(timeStamp, type, amount, note, tags); });
        }
        
        public Task<ITransaction> EditTransaction(int transactionId, DateTime newTimeStamp, TransactionType newType, decimal newAmount, 
            string newNote = null, string newTags = null)
        {
            return Task.Run(() => (_transactionController.Edit(transactionId, newTimeStamp, newType, newAmount, newNote, newTags) as ITransaction));
        }

        public Task<IEnumerable<ITransaction>> GetTransactions(string filteringQuery = null)
        {
            return Task<IEnumerable<ITransaction>>.Run(() => {             
                    
                    var result = _transactionController.GetAllTransactions();

                    if (!string.IsNullOrEmpty(filteringQuery))
                        result = Filter.ExecuteQuery(result, filteringQuery);

                    return result as IEnumerable<ITransaction>;
                }
            );
        }

        public Task<ITransaction> GetTransaction(int transactionId)
        {
            return Task<ITransaction>.Run(() => _transactionController.GetTransaction(transactionId) as ITransaction);
        }

        public Task RemoveTransaction(int transactionId)
        {
            return Task.Run(() => _transactionController.Remove(transactionId));
        }
        
        public Task<IEnumerable<ITransaction>> GetStagedTransactions()
        {
            return Task<IEnumerable<ITransaction>>.Run(() => _transactionController.GetStagedTransactions() as IEnumerable<ITransaction>);
        }

        public Task<IEnumerable<ITransaction>> UnstageAllTransactions()
        {
            return Task<IEnumerable<ITransaction>>.Run(() => _transactionController.UnstageAll() as IEnumerable<ITransaction>);
        }
        
        public Task<IEnumerable<ITransaction>> CommitTransactions()
        {
            return Task<IEnumerable<ITransaction>>.Run(() => _transactionController.CommitAll() as IEnumerable<ITransaction>);
        }

        public Task<IReport> GenerateReport(ReportType type, DateTime startDate, DateTime endDate, string filteringQuery)
        {
            return Task<IReport>.Run(() =>
            {
                var transactions = _transactionController.GetAllTransactions();
            
                var report = ReportGenerator.CreateReport(type,
                    transactions,
                    startDate,
                    endDate,
                    filteringQuery);
                
                return (IReport)report;
            });
        }
    }
}