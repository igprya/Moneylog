using System;
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
            bool tagsMatch;
            
            if (filteringTags == null)
                throw new ArgumentException("Filtering tags are null.");
            
            if (transactionTags == null)
            {
                tagsMatch = FilteringValue == "";
                return ComparisonOperation == ComparisonOperation.Equal ? tagsMatch : !tagsMatch;
            }

            if (FilteringValue == "")
                return ComparisonOperation != ComparisonOperation.Equal;
            
            tagsMatch = false;

            foreach (var tag in filteringTags)
                tagsMatch = transactionTags.Contains(tag);

            if (ComparisonOperation == ComparisonOperation.NotEqual)
                tagsMatch = !tagsMatch;
                
            return tagsMatch;
        }
    }
}