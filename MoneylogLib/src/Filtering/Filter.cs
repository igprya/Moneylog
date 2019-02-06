using System;
using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering
{
    internal static class Filter
    {
        public static List<Transaction> ExecuteQuery(List<Transaction> transactions, string query)
        {
            var filters = FilteringQueryParser.Parse(query);
            return ApplyFilters(transactions, filters);
        }
        
        private static List<Transaction> ApplyFilters(List<Transaction> transactions, IEnumerable<ITransactionFilter> filters)
        {
            if (transactions == null) throw new ArgumentNullException(nameof(transactions));
            var result = transactions;

            foreach (var filter in filters)
            {
                if (filter.ChainingMode == ChainingMode.And)
                {
                    result = filter.Apply(result);
                }
                else
                {
                    var tResult = filter.Apply(transactions);
                    
                    foreach (var t in tResult)
                        if (!result.Contains(t))
                            result.Add(t);
                }
            }

            return result;
        }
    }
}