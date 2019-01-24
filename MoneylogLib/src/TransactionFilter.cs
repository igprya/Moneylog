using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Interfaces;
using MoneylogLib.Models;

namespace MoneylogLib
{
    enum FilteringOption
    {
        Greater, GreaterOrEqual, Equal, Less, LessOrEqual, NotEqual
    }
    
    class TransactionFilter<T> : ITransactionFilter
        where T : IComparable
    {
        private readonly T _filteringValue;
        private readonly FilteringOption _filteringOption;
        private readonly string _filteringPropertyName;
        
        public TransactionFilter(string filteringPropertyName, T filteringValue, FilteringOption filteringOption)
        {
            _filteringPropertyName = filteringPropertyName;
            _filteringValue = filteringValue;
            _filteringOption = filteringOption;
        }

        public IEnumerable<Transaction> Apply(IEnumerable<Transaction> transactions)
        {
            var result = new List<Transaction>();

            foreach (var transaction in transactions)
            {
                T transactionProperty = GetFilteringPropertyValue(transaction);
                
                if (Filter(transactionProperty))
                    result.Add(transaction);
            }

            return result;
        }

        private T GetFilteringPropertyValue(ITransaction transaction)
        {
            if (_filteringPropertyName == "Tags")
                return (T) (object)string.Join(",", transaction.Tags);
            
            return (T) transaction?.GetType().GetProperty(_filteringPropertyName).GetValue(transaction, null);
        }

        private bool Filter(T transactionProperty)
        {
            if (_filteringPropertyName == "Tags")
                return FilterTags(transactionProperty);
            
            switch (_filteringOption)
            {
                case FilteringOption.Greater:
                    return transactionProperty.CompareTo(_filteringValue) > 0;
                case FilteringOption.GreaterOrEqual:
                    return transactionProperty.CompareTo(_filteringValue) >= 0;
                case FilteringOption.Equal:                     
                    return transactionProperty.CompareTo(_filteringValue) == 0;
                case FilteringOption.Less:
                    return transactionProperty.CompareTo(_filteringValue) < 0;
                case FilteringOption.LessOrEqual:
                    return transactionProperty.CompareTo(_filteringValue) <= 0;
                case FilteringOption.NotEqual:
                    return transactionProperty.CompareTo(_filteringValue) != 0;
            }

            throw new InvalidOperationException("Filter has failed to compare property values.");
        }

        private bool FilterTags(T transactionProperty)
        {
            string[] transactionTags = (transactionProperty as string).Split(',');
            string[] filteringTags = (_filteringValue as string).Split(',');

            bool tagsMatch = false;

            foreach (var tag in filteringTags)
                tagsMatch = transactionTags.Contains(tag);

            if (_filteringOption == FilteringOption.NotEqual)
                tagsMatch = !tagsMatch;
                
            return tagsMatch;
        }
    }
}