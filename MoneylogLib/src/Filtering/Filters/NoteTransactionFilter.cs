namespace MoneylogLib.Filtering.Filters
{
    internal class NoteTransactionFilter : TransactionFilter<string>
    {
        private static string PropertyName = "Note";
        
        public NoteTransactionFilter(string filteringValue, ComparisonOperation comparisonOperation, ChainingMode chainingMode) 
            : base(PropertyName, filteringValue, comparisonOperation, chainingMode)
        {
        }

        protected override bool Filter(string transactionProperty)
        {
            if (transactionProperty == null)
                return false;

            return transactionProperty.Contains(FilteringValue);
        }    
    }
}