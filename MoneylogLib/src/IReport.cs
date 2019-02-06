using System;
using System.Collections.Generic;
using MoneylogLib.Reporting;

namespace MoneylogLib
{
    public interface IReport
    {
        List<IReport> Subreports { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        decimal Income { get; }
        decimal AverageIncome { get; }
        int IncomeTransactionsCount { get; }
        decimal Expense { get; }
        decimal AverageExpense { get; }
        int ExpenseTransactionsCount { get; }
        decimal Balance { get; }
        ReportType Type { get; }

    }
}