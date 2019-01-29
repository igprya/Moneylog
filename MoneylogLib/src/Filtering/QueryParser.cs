using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Filtering.Filters;
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
            string[] queryItems = filteringQuery.Split(' ');
           
            if (queryItems.Length < 3 || queryItems.Length > 4)
                throw new ArgumentException($"Invalid query: {filteringQuery} contains {queryItems.Length} elements; 3 or 4 elements expected.");
           
            if (queryItems[0].ToUpper() == "OR" || queryItems[0].ToUpper() == "AND")
            {
                return new QueryElements
                {
                    ChainingMode = queryItems[0].ToUpper(),
                    PropertyName = queryItems[1],
                    ComparisonOperation = queryItems[2],
                    TargetValue = queryItems[3]
                };
            }

            return new QueryElements
            {
                PropertyName = queryItems[0],
                ComparisonOperation = queryItems[1],
                TargetValue = queryItems[2]
            };
        }

        private static void ValidateQueryElements(QueryElements queryElements)
        {
            string[] validChainingModes = new[] {"OR", "AND"};
            string[] validPropertyNames = new[] {"Id", "Timestamp", "Type", "Amount", "Note", "Tags", "CreatedTimestamp"};
            string[] validOperations = new[] {"<", "<=", "==", ">", ">=", "!="};
            
            if (!validChainingModes.Contains(queryElements.ChainingMode))
                throw new ArgumentException($"{queryElements.ChainingMode} is not a valid filter chaining mode.");
            
            if (!validPropertyNames.Contains(queryElements.PropertyName))
                throw new ArgumentException($"{queryElements.PropertyName} is not a valid Transaction property.");
            
            if (queryElements.PropertyName == "Note" || queryElements.PropertyName == "Tags")
                validOperations = new[] {"==", "!="};
            
            if (!validOperations.Contains(queryElements.ComparisonOperation))
                throw new ArgumentException($"{queryElements.ComparisonOperation} is not a valid comparison operation for property {queryElements.PropertyName}.");

            bool validTargetValue = false;
            
            switch (queryElements.PropertyName)
            {
                case "Id": validTargetValue = int.TryParse(queryElements.TargetValue, out _); 
                    break;
                case "Timestamp": validTargetValue = DateTime.TryParse(queryElements.TargetValue, out _);
                    break;
                case "Type": validTargetValue = queryElements.TargetValue == "Income" || queryElements.TargetValue == "Expense";
                    break;
                case "Amount": validTargetValue = decimal.TryParse(queryElements.TargetValue, out _);
                    break;
                case "Note": validTargetValue = !string.IsNullOrEmpty(queryElements.TargetValue);
                    break;
                case "Tags": validTargetValue = !string.IsNullOrEmpty(queryElements.TargetValue);
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
                case "Timestamp": 
                    return new TimestampTransactionFilter(DateTime.Parse(queryElements.TargetValue)
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
                case "CreatedTimestamp":
                    return new CreatedTimestampTransactionFilter(DateTime.Parse(queryElements.TargetValue)
                        ,comparisonOperation
                        ,chainingMode);
            }
            
            throw new InvalidOperationException("Unexpected error while attempting to create a filter.");
        }

        private static List<string> GetQueriesFromInputString(string filteringQuery)
        {
            filteringQuery = filteringQuery.Trim();
            
            var queries = new List<string>();

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