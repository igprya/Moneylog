namespace MoneylogLib.Filtering.Filters
{
    internal class AmountTransactionFilter : TransactionFilter<decimal>
    {
        private const string PropertyName = "Amount";
        
        public AmountTransactionFilter(decimal filteringValue, ComparisonOperation comparisonOperation, ChainingMode chainingMode) 
            : base(PropertyName, filteringValue, comparisonOperation, chainingMode)
        {
        }
    }
}