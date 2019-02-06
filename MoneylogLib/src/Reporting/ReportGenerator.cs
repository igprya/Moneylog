using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Filtering;
using MoneylogLib.Models;
using MoneylogLib.Reporting.Reports;

namespace MoneylogLib.Reporting
{
    public static class ReportGenerator
    {
        public static Report CreateReport(ReportType reportType, IEnumerable<ITransaction> transactions, DateTime startDate, DateTime endDate
            , string filteringQuery)
        {
            var transactionList = transactions.ToList();
            
            if (!string.IsNullOrEmpty(filteringQuery))
                transactions = Filter.ExecuteQuery(transactionList, filteringQuery);
            
            switch (reportType)
            {
                case ReportType.Day :           return new DayReport(transactionList, startDate);
                case ReportType.Month :         return new MonthReport(transactionList, startDate);
                case ReportType.Year:           return new YearReport(transactionList, startDate);
                case ReportType.RangeDaily :    return new RangeDailyReport(transactionList, startDate, endDate);
                case ReportType.Total:          return new TotalReport(transactionList);
            }
            
            throw new InvalidOperationException("Unable to determine report type.");
        }
        
    }
}