using System;

namespace MoneylogLib.Filtering.Filters
{
    internal class DateTransactionFilter : TransactionFilter<DateTime>
    {
        private static string PropertyName = "Date";
        
        public DateTransactionFilter(DateTime filteringValue, ComparisonOperation comparisonOperation, ChainingMode chainingMode)
            : base(PropertyName, filteringValue, comparisonOperation, chainingMode)
        {
        }
        
        protected override bool Filter(DateTime transactionDate)
        {
            switch (ComparisonOperation)
            {
                case ComparisonOperation.Greater:
                    return transactionDate.Date.CompareTo(FilteringValue.Date) > 0;
                case ComparisonOperation.GreaterOrEqual:
                    return transactionDate.Date.CompareTo(FilteringValue.Date) >= 0;
                case ComparisonOperation.Equal:                     
                    return transactionDate.Date.CompareTo(FilteringValue.Date) == 0;
                case ComparisonOperation.Less:
                    return transactionDate.Date.CompareTo(FilteringValue.Date) < 0;
                case ComparisonOperation.LessOrEqual:
                    return transactionDate.Date.CompareTo(FilteringValue.Date) <= 0;
                case ComparisonOperation.NotEqual:
                    return transactionDate.Date.CompareTo(FilteringValue.Date) != 0;
            }

            throw new InvalidOperationException("Filter has failed to compare property values.");
        }
    }
}