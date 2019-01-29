using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Filters
{
    internal class TypeTransactionFilter : TransactionFilter<TransactionType>
    {
        private static readonly string PropertyName = "Type";
        
        public TypeTransactionFilter(TransactionType filteringValue, ComparisonOperation comparisonOperation, ChainingMode chainingMode) 
            : base(PropertyName, filteringValue, comparisonOperation, chainingMode)
        {
        }
    }
}