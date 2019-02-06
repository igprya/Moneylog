using System;
using System.Collections.Generic;
using System.Linq;

namespace MoneylogLib.Reporting
{
    internal abstract class Report : IReport
    {
        public List<IReport> Subreports { get; protected set; }
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }
        public decimal Income { get; protected set; }
        public decimal AverageIncome { get; protected set; }
        public int IncomeTransactionsCount { get; protected set; }
        public decimal Expense { get; protected set; }
        public decimal AverageExpense { get; protected set; }
        public int ExpenseTransactionsCount { get; protected set; }
        public decimal Balance { get; protected set; }
        public ReportType Type { get; protected set; }

        protected Report(ReportType reportType)
        {
            Type = reportType;
        }

        protected decimal CalculateAverage(decimal value, int count)
        {
            if (count == 0)
                return 0;
            
            if (value == 0)
                throw new InvalidOperationException($"value is 0 but count is {count}.");

            return value / count;
        }

        protected void GenerateFromSubreports()
        {
            if (Subreports == null || !Subreports.Any())
                throw new InvalidOperationException("No subreports to generate report from."); 
            
            foreach (var r in Subreports)
            {
                Income += r.Income;
                IncomeTransactionsCount += r.IncomeTransactionsCount;
                Expense += r.Expense;
                ExpenseTransactionsCount += r.ExpenseTransactionsCount;
            }

            AverageIncome = CalculateAverage(Income, IncomeTransactionsCount);
            AverageExpense = CalculateAverage(Expense, ExpenseTransactionsCount);
            Balance = Income - Expense;
        }

        public override string ToString()
        {
            var date = StartDate != EndDate ? $"{StartDate.ToShortDateString()} > {EndDate.ToShortDateString()}" : StartDate.ToShortDateString();
            return $"{date}\t+{Income} ({IncomeTransactionsCount}ts)\t\t-{Expense} ({ExpenseTransactionsCount}ts)\t\tAI {AverageIncome:N2}\t\t\tAE {AverageExpense:N2}\t\tT {Balance}";
        }
    }
}