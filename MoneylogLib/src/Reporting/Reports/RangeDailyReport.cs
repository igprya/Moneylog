using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Models;
using MoneylogLib.Reporting.Comparers;

namespace MoneylogLib.Reporting.Reports
{
    internal class RangeDailyReport : Report
    {
        public RangeDailyReport(List<ITransaction> transactions, DateTime startDate, DateTime endDate)
            : base(ReportType.RangeDaily)
        {
            
            if (transactions == null)
                throw new ArgumentException("Transaction list is null.");

            if (!transactions.Any())
                throw new ArgumentException("Transaction list is empty.");
            
            if (endDate < startDate)
                throw new ArgumentException($"Report end date {endDate} is less than starting date {startDate}");

            Generate(transactions, startDate, endDate);
        }

        private void Generate(List<ITransaction> transactions, DateTime startDate, DateTime endDate)
        {
            Subreports = new List<Report>();
            
            StartDate = startDate;
            EndDate = endDate;

            var rangeTransactions = transactions.Where(t => t.Date >= StartDate && t.Date <= EndDate);
            var days = rangeTransactions.Select(t => t.Date).Distinct(new DateTimeDayEqualityComparer()).ToList();

            foreach (var d in days)
            {
                var dailyReport = new DayReport(transactions, d);
                Subreports.Add(dailyReport);
            }

            GenerateFromSubreports();
        }
    }
}