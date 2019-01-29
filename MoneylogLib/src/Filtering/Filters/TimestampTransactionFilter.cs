using System;

namespace MoneylogLib.Filtering.Filters
{
    internal class TimestampTransactionFilter : TransactionFilter<DateTime>
    {
        private const string PropertyName = "Timestamp";

        public TimestampTransactionFilter(DateTime filteringValue, ComparisonOperation comparisonOperation, ChainingMode chainingMode)
            : base(PropertyName, filteringValue, comparisonOperation, chainingMode)
        {
        }
        
        protected override bool Filter(DateTime transactionTimestamp)
        {
            return true;
        }
    }
}