using System;
using System.Collections.Generic;
using MoneylogLib.Filtering;
using MoneylogLib.Models;
using MoneylogLib.Reporting.Reports;

namespace MoneylogLib.Reporting
{
    public static class ReportGenerator
    {
        public static Report CreateReport(ReportType reportType, List<Transaction> transactions, DateTime startDate, DateTime endDate
            , string filteringQuery)
        {
            if (!string.IsNullOrEmpty(filteringQuery))
                transactions = Filter.ExecuteQuery(transactions, filteringQuery);
            
            switch (reportType)
            {
                case ReportType.Day :           return new DayReport(transactions, startDate);
                case ReportType.Month :         return new MonthReport(transactions, startDate);
                case ReportType.Year:           return new YearReport(transactions, startDate);
                case ReportType.RangeDaily :    return new RangeDailyReport(transactions, startDate, endDate);
            }
            
            throw new InvalidOperationException("Unable to determine report type.");
        }
        
    }
}