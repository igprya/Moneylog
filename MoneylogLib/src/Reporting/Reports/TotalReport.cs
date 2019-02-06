using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Models;
using MoneylogLib.Reporting.Comparers;

namespace MoneylogLib.Reporting.Reports
{
    internal class TotalReport : Report
    {
        public TotalReport(List<Transaction> transactions)
            : base(ReportType.Total)
        {
            if (transactions == null)
                throw new ArgumentException("Transaction list is null.");
    
            if (!transactions.Any())
                throw new ArgumentException("Transaction list is empty.");
    
            Generate(transactions);
        }
    
        private void Generate(List<Transaction> transactions)
        {
            Subreports = new List<IReport>();

            var orderedTransactions = transactions.OrderBy(t => t.Date).ToList();
            var firstTransaction = orderedTransactions.First().Date;
            var lastTransaction = orderedTransactions.Last().Date;
            
            StartDate = firstTransaction;
            EndDate = lastTransaction;
    
            var months = transactions.Select(t => t.Date).Distinct(new DateTimeMonthEqualityComparer()).ToList();
    
            foreach (var m in months)
            {
                var monthReport = new MonthReport(transactions, m);
                Subreports.Add(monthReport);
            }
    
            GenerateFromSubreports();
        }
    }
}