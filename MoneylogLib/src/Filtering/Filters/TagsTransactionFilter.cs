using System.Linq;

namespace MoneylogLib.Filtering.Filters
{
    internal class TagsTransactionFilter : TransactionFilter<string>
    {
        private const string PropertyName = "Tags";
        
        public TagsTransactionFilter(string filteringValue, ComparisonOperation comparisonOperation, ChainingMode chainingMode) 
            : base(PropertyName, filteringValue, comparisonOperation, chainingMode)
        {
        }

        protected override bool Filter(string transactionTagString)
        {
            string[] transactionTags = transactionTagString?.Split(',');
            string[] filteringTags = FilteringValue?.Split(',');

            if (transactionTags == null || filteringTags == null)
                return false;

            bool tagsMatch = false;

            foreach (var tag in filteringTags)
                tagsMatch = transactionTags.Contains(tag);

            if (ComparisonOperation == ComparisonOperation.NotEqual)
                tagsMatch = !tagsMatch;
                
            return tagsMatch;
        }
    }
}