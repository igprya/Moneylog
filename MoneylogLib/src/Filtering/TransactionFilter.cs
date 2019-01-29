using System;
using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering
{
    internal abstract class TransactionFilter<T> : ITransactionFilter where T : IComparable
    {
        protected readonly T FilteringValue;
        protected readonly ComparisonOperation ComparisonOperation;
        protected readonly string FilteringPropertyName;

        public ChainingMode ChainingMode { get; }

        protected TransactionFilter(string filteringPropertyName, T filteringValue, ComparisonOperation comparisonOperation, ChainingMode chainingMode)
        {
            FilteringPropertyName = filteringPropertyName;
            FilteringValue = filteringValue;
            ComparisonOperation = comparisonOperation;
            ChainingMode = chainingMode;
        }

        public List<Transaction> Apply(IEnumerable<Transaction> transactions)
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
            return (T) transaction?.GetType().GetProperty(FilteringPropertyName)?.GetValue(transaction, null);
        }

        protected virtual bool Filter(T transactionProperty)
        {
            switch (ComparisonOperation)
            {
                case ComparisonOperation.Greater:
                    return transactionProperty.CompareTo(FilteringValue) > 0;
                case ComparisonOperation.GreaterOrEqual:
                    return transactionProperty.CompareTo(FilteringValue) >= 0;
                case ComparisonOperation.Equal:                     
                    return transactionProperty.CompareTo(FilteringValue) == 0;
                case ComparisonOperation.Less:
                    return transactionProperty.CompareTo(FilteringValue) < 0;
                case ComparisonOperation.LessOrEqual:
                    return transactionProperty.CompareTo(FilteringValue) <= 0;
                case ComparisonOperation.NotEqual:
                    return transactionProperty.CompareTo(FilteringValue) != 0;
            }

            throw new InvalidOperationException("Filter has failed to compare property values.");
        }
    }
}