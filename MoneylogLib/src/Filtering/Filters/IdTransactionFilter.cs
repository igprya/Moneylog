namespace MoneylogLib.Filtering.Filters
{
    internal class IdTransactionFilter : TransactionFilter<int>
    {
        private static string PropertyName = "Id";
        
        public IdTransactionFilter(int filteringValue, ComparisonOperation comparisonOperation, ChainingMode chainingMode) 
            : base(PropertyName, filteringValue, comparisonOperation, chainingMode)
        {
        }
    }
}