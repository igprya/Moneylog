using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Models;
using MoneylogLib.Reporting.Helpers;

namespace MoneylogLib.Reporting.Reports
{
    internal class MonthReport : Report
    {
        public MonthReport(List<Transaction> transactions, DateTime month)
            : base(ReportType.Month)
        {
            if (transactions == null)
                throw new ArgumentException("Transaction list is null.");

            if (!transactions.Any())
                throw new ArgumentException("Transaction list is empty.");

            Generate(transactions, month);
        }

        private void Generate(List<Transaction> transactions, DateTime date)
        {
            Subreports = new List<Report>();
            
            StartDate = new DateTime(date.Year, date.Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);

            var monthTransactions = transactions.Where(t => t.Timestamp >= StartDate && t.Timestamp <= EndDate);
            var days = monthTransactions.Select(t => t.Timestamp).Distinct(new DateTimeDayEqualityComparer()).ToList();

            foreach (var d in days)
            {
                var dailyReport = new DayReport(transactions, d);
                Subreports.Add(dailyReport);
            }

            GenerateFromSubreports();
        }
    }
}