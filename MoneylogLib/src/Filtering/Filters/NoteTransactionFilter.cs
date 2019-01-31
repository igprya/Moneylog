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
            var noteIsNullOrEmpty = string.IsNullOrEmpty(transactionProperty);
            bool noteContainsValue;
            
            if (noteIsNullOrEmpty)
            {
                noteContainsValue = FilteringValue == "";
                return ComparisonOperation == ComparisonOperation.Equal ? noteContainsValue : !noteContainsValue;
            }

            if (FilteringValue == "")
                return ComparisonOperation != ComparisonOperation.Equal;

            noteContainsValue = transactionProperty.Contains(FilteringValue);
            return ComparisonOperation == ComparisonOperation.Equal ? noteContainsValue : !noteContainsValue;
        }    
    }
}