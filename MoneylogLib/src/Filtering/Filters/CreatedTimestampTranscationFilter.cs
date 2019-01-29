using System;

namespace MoneylogLib.Filtering.Filters
{
    internal class CreatedTimestampTransactionFilter : TransactionFilter<DateTime>
    {
        private const string PropertyName = "CreatedTimestamp";

        public CreatedTimestampTransactionFilter(DateTime filteringValue, ComparisonOperation comparisonOperation, ChainingMode chainingMode)
            : base(PropertyName, filteringValue, comparisonOperation, chainingMode)
        {
        }
        
        protected override bool Filter(DateTime transactionTimestamp)
        {
            return true;
        }
    }
}