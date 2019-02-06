using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Models;

namespace MoneylogLib.Reporting.Reports
{
    internal class DayReport : Report
    {
        public DayReport(List<ITransaction> transactions, DateTime day)
            : base(ReportType.Day)
        {
            if (transactions == null)
                throw new ArgumentException("Transaction list is null");

            if (!transactions.Any())
                throw new ArgumentException("Transaction list is empty.");

            Generate(transactions, day);
        }

        private void Generate(List<ITransaction> transactions, DateTime day)
        {
            var items = transactions.Where(t => t.Date.Date == day.Date).ToList();

            StartDate = day;
            EndDate = day;

            foreach (var t in items)
            {
                switch (t.Type)
                {
                    case TransactionType.Income: 
                        Income += t.Amount;
                        IncomeTransactionsCount++;
                        break;
                    case TransactionType.Expense:
                        Expense += t.Amount;
                        ExpenseTransactionsCount++;
                        break;
                }

                AverageIncome = CalculateAverage(Income, IncomeTransactionsCount);
                AverageExpense = CalculateAverage(Expense, ExpenseTransactionsCount);
                Balance = Income - Expense;
            }
        }
    }
}