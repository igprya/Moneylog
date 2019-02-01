using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MoneylogLib.Filtering.Filters;
using MoneylogLib.Filtering.Ordering;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering
{
    internal static class QueryParser
    {
        private class QueryElements
        {
            public string ChainingMode = "AND";
            public string PropertyName;
            public string ComparisonOperation;
            public string TargetValue;
        }

        public static IEnumerable<ITransactionFilter> Parse(string filteringQuery)
        {
            if (string.IsNullOrWhiteSpace(filteringQuery))
                throw new ArgumentException("Query string is empty.");
            
            var queries = GetQueriesFromInputString(filteringQuery);
            var filters = new List<ITransactionFilter>();
            
            if (queries.Count == 0)
                throw new ArgumentException("Unable to obtain individual queries from query string.");
            
            foreach (var query in queries)
                filters.Add(CreateFilterFromQuery(query));

            return filters;
        }
        
        private static ITransactionFilter CreateFilterFromQuery(string filteringQuery)
        {
            var queryElements = GetQueryElements(filteringQuery);
            ValidateQueryElements(queryElements);
            return CreateFilter(queryElements);
        }

        private static QueryElements GetQueryElements(string filteringQuery)
        {
            var splitRegex = new Regex("[^\\s\"']+|\"([^\"]*)\"|'([^']*)'");
            var queryItems = splitRegex.Matches(filteringQuery);
         
            if (queryItems.Count < 3 || queryItems.Count > 4)
                throw new ArgumentException($"Invalid query: {filteringQuery} contains {queryItems.Count} elements; 3 or 4 elements expected.");
           
            if (queryItems[0].Value.ToUpper() == "OR" || queryItems[0].Value.ToUpper() == "AND")
            {
                return new QueryElements
                {
                    ChainingMode = queryItems[0].Value.ToUpper(),
                    PropertyName = queryItems[1].Value,
                    ComparisonOperation = queryItems[2].Value,
                    TargetValue = queryItems[3].Value.Replace("\"", "")
                };
            }

            if (queryItems[0].Value.ToUpper() == "ORDERBY")
            {
                return new QueryElements
                {
                    ChainingMode = queryItems[0].Value.ToUpper(),
                    PropertyName = queryItems[1].Value,
                    ComparisonOperation = queryItems[2].Value.ToUpper()
                };
            }
            
            return new QueryElements
            {
                PropertyName = queryItems[0].Value,
                ComparisonOperation = queryItems[1].Value,
                TargetValue = queryItems[2].Value.Replace("\"", "")
            };
        }

        private static void ValidateQueryElements(QueryElements queryElements)
        {
            var validChainingModes = new[] {"OR", "AND", "ORDERBY"};
            var validPropertyNames = new[] {"Id", "Date", "Type", "Amount", "Note", "Tags"};
            var validComparisonOperations = new[] {"<", "<=", "=", ">", ">=", "!="};
            var validOrderingOperations = new[] {"ASC", "DESC"};

            if (queryElements.ChainingMode == "ORDERBY")
            {
                if (!validPropertyNames.Contains(queryElements.PropertyName))
                    throw new ArgumentException($"{queryElements.PropertyName} is not a valid Transaction property.");
                
                if (!validOrderingOperations.Contains(queryElements.ComparisonOperation))
                    throw new ArgumentException($"{queryElements.ComparisonOperation} is not a valid ordering operation.");

                return;
            }
            
            if (!validChainingModes.Contains(queryElements.ChainingMode))
                throw new ArgumentException($"{queryElements.ChainingMode} is not a valid filter chaining mode.");
            
            if (!validPropertyNames.Contains(queryElements.PropertyName))
                throw new ArgumentException($"{queryElements.PropertyName} is not a valid Transaction property.");
            
            if (queryElements.PropertyName == "Note" || queryElements.PropertyName == "Tags")
                validComparisonOperations = new[] {"=", "!="};
            
            if (!validComparisonOperations.Contains(queryElements.ComparisonOperation))
                throw new ArgumentException($"{queryElements.ComparisonOperation} is not a valid comparison operation for property {queryElements.PropertyName}.");

            bool validTargetValue = false;
            
            switch (queryElements.PropertyName)
            {
                case "Id": validTargetValue = int.TryParse(queryElements.TargetValue, out _); 
                    break;
                case "Date": validTargetValue = DateTime.TryParse(queryElements.TargetValue, out _);
                    break;
                case "Type": validTargetValue = queryElements.TargetValue == "Income" || queryElements.TargetValue == "Expense";
                    break;
                case "Amount": validTargetValue = decimal.TryParse(queryElements.TargetValue, out _);
                    break;
                case "Note": validTargetValue = queryElements.TargetValue != null;
                    break;
                case "Tags": validTargetValue = queryElements.TargetValue != null;
                    break;
                case "CreatedTimestamp": validTargetValue = DateTime.TryParse(queryElements.TargetValue, out _);
                    break;
            }
            
            if (!validTargetValue)
                throw new ArgumentException(
                    $"{queryElements.TargetValue} is not a valid target value for property {queryElements.PropertyName}.");
        }

        private static ITransactionFilter CreateFilter(QueryElements queryElements)
        {
            if (queryElements.ChainingMode == "ORDERBY")
                return new OrderingFilter(queryElements.PropertyName, queryElements.ComparisonOperation);
            
            var chainingMode = ChainingMode.And;

            if (queryElements.ChainingMode == "OR")
                chainingMode = ChainingMode.Or;
            
            var comparisonOperation = ComparisonOperation.Equal;

            switch (queryElements.ComparisonOperation)
            {
                case "<": comparisonOperation = ComparisonOperation.Less;
                    break;
                case "<=": comparisonOperation = ComparisonOperation.LessOrEqual;
                    break;
                case ">": comparisonOperation = ComparisonOperation.Greater;
                    break;
                case ">=": comparisonOperation = ComparisonOperation.GreaterOrEqual;
                    break;
                case "!=": comparisonOperation = ComparisonOperation.NotEqual;
                    break;
            }

            switch (queryElements.PropertyName)
            {
                case "Id": 
                    return new IdTransactionFilter(int.Parse(queryElements.TargetValue)
                        ,comparisonOperation
                        ,chainingMode);
                case "Date": 
                    return new DateTransactionFilter("Timestamp"
                        ,DateTime.Parse(queryElements.TargetValue)
                        ,comparisonOperation
                        ,chainingMode);
                case "Type": 
                    return new TypeTransactionFilter(queryElements.TargetValue == "Income" ? TransactionType.Income : TransactionType.Expense
                        ,comparisonOperation
                        ,chainingMode);
                case "Amount": 
                    return new AmountTransactionFilter(decimal.Parse(queryElements.TargetValue)
                        ,comparisonOperation
                        ,chainingMode);
                case "Note":
                    return new NoteTransactionFilter(queryElements.TargetValue
                        ,comparisonOperation
                        ,chainingMode);
                case "Tags":
                    return new TagsTransactionFilter(queryElements.TargetValue
                        ,comparisonOperation
                        ,chainingMode);
            }
            
            throw new InvalidOperationException("Unexpected error while attempting to create a filter.");
        }
        
        private static List<string> GetQueriesFromInputString(string filteringQuery)
        {
            filteringQuery = filteringQuery.Trim();
            
            var queries = new List<string>();

            var lastOrderByIndex = filteringQuery.LastIndexOf("ORDERBY");
            if (lastOrderByIndex != -1)
            {
                var query = filteringQuery.Substring(lastOrderByIndex, filteringQuery.Length - lastOrderByIndex);
                filteringQuery = filteringQuery.Remove(lastOrderByIndex, filteringQuery.Length - lastOrderByIndex).Trim();
                queries.Add(query);
            }

            while (filteringQuery.Length != 0)
            {
                var lastOrIndex = filteringQuery.ToUpper().LastIndexOf("OR");
                var lastAndIndex = filteringQuery.ToUpper().LastIndexOf("AND");

                var lastOperatorIndex = (lastOrIndex > lastAndIndex) ? lastOrIndex : lastAndIndex;

                if (lastOperatorIndex != -1)
                {
                    var query = filteringQuery.Substring(lastOperatorIndex, filteringQuery.Length - lastOperatorIndex);
                    filteringQuery = filteringQuery.Remove(lastOperatorIndex, filteringQuery.Length - lastOperatorIndex).Trim();
                    
                    queries.Add(query);
                    continue;
                }
                
                queries.Add(filteringQuery);
                filteringQuery = filteringQuery.Remove(0);
            }

            // Restore filter order
            queries.Reverse();
            
            return queries;
        }
    }
}