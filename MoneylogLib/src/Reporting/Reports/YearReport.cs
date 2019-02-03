using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Models;
using MoneylogLib.Reporting.Helpers;

namespace MoneylogLib.Reporting.Reports
{
    internal class YearReport : Report
    {
        public YearReport(List<Transaction> transactions, DateTime year)
            : base(ReportType.Year)
        {
            
            if (transactions == null)
                throw new ArgumentException("Transaction list is null.");

            if (!transactions.Any())
                throw new ArgumentException("Transaction list is empty.");

            Generate(transactions, year);
        }

        private void Generate(List<Transaction> transactions, DateTime date)
        {
            Subreports = new List<Report>();
            
            StartDate = new DateTime(date.Year, 1, 1);
            EndDate = StartDate.AddYears(1).AddDays(-1);

            var yearTransactions = transactions.Where(t => t.Date >= StartDate && t.Date <= EndDate);
            var months = yearTransactions.Select(t => t.Date).Distinct(new DateTimeMonthEqualityComparer()).ToList();

            foreach (var m in months)
            {
                var monthReport = new MonthReport(transactions, m);
                Subreports.Add(monthReport);
            }

            GenerateFromSubreports();
        }
    }
}