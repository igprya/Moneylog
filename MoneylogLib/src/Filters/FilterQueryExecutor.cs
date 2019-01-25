using System;
using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filters
{
    static class FilterQueryExecutor
    {
        public static List<Transaction> ExecuteQuery(List<Transaction> transactions, string query)
        {
            var filters = FilterQueryParser.Parse(query);
            return ApplyFilters(transactions, filters);
        }
        
        private static List<Transaction> ApplyFilters(List<Transaction> transactions, IEnumerable<ITransactionFilter> filters)
        {
            if (transactions == null) throw new ArgumentNullException(nameof(transactions));
            var result = transactions;

            foreach (var filter in filters)
            {
                if (filter.ChainingMode == FilterChainingMode.And)
                {
                    result = filter.Apply(result);
                }
                else
                {
                    result.AddRange(filter.Apply(transactions));
                }
            }    

            return result;
        }
    }
}