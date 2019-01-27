using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering
{
    static class FilterQueryParser
    {
        private static readonly Dictionary<string, Type> PropertyNameTypeMap = new Dictionary<string, Type>()
        {
            {"Id", typeof(int)},
            {"Timestamp", typeof(DateTime)},
            {"Type", typeof(FilteringOption)},
            {"Amount", typeof(decimal)},
            {"Note", typeof(string)},
            {"Tags", typeof(string)},
            {"CreatedTimestamp", typeof(DateTime)}
        };

        public static List<ITransactionFilter> Parse(string filteringQuery)
        {
            var queries = GetQueriesFromInputString(filteringQuery);
            var filters = new List<ITransactionFilter>();
            
            foreach (var query in queries)
                filters.Add(CreateFilterFromQuery(query));

            return filters;
        }
        
        private static ITransactionFilter CreateFilterFromQuery(string filteringQuery)
        {
            string[] queryElements = filteringQuery.Split(' ');
           
            if (queryElements.Length < 3 || queryElements.Length > 4)
                throw new ArgumentException($"Invalid query: {filteringQuery} contains {queryElements.Length} elements; 3 or 4 elements expected.");

            string chainingMode = "AND";
            string propertyName;
            string comparisonOperation;
            string targetValue;
            
            if (queryElements[0].ToUpper() == "OR" || queryElements[0].ToUpper() == "AND")
            {
                chainingMode = queryElements[0].ToUpper();
                propertyName = queryElements[1];
                comparisonOperation = queryElements[2];
                targetValue = queryElements[3];
            }
            else
            {
                propertyName = queryElements[0];
                comparisonOperation = queryElements[1];
                targetValue = queryElements[2];
            }

            ValidateQueryElements(chainingMode, propertyName, comparisonOperation, targetValue);

            return CreateFilter(chainingMode, propertyName, comparisonOperation, targetValue);
        }

        private static void ValidateQueryElements(string chainingMode, string propertyName, string comparisonOperation, string targetValue)
        {
            string[] validChainingModes = new[] {"OR", "AND"};
            string[] validPropertyNames = new[] {"Id", "Timestamp", "Type", "Amount", "Note", "Tags", "CreatedTimestamp"};
            string[] validOperations = new[] {"<", "<=", "==", ">", ">=", "!="};
            
            if (!validChainingModes.Contains(chainingMode))
                throw new ArgumentException($"{chainingMode} is not a valid filter chaining mode.");
            
            if (!validPropertyNames.Contains(propertyName))
                throw new ArgumentException($"{propertyName} is not a valid Transaction property.");
            
            if (propertyName == "Note" || propertyName == "Tags")
                validOperations = new[] {"==", "!="};
            
            if (!validOperations.Contains(comparisonOperation))
                throw new ArgumentException($"{comparisonOperation} is not a valid comparison operation for property {propertyName}.");

            bool validTargetValue = false;
            
            switch (propertyName)
            {
                case "Id": validTargetValue = int.TryParse(targetValue, out _); 
                    break;
                case "Timestamp": validTargetValue = DateTime.TryParse(targetValue, out _);
                    break;
                case "Type": validTargetValue = (targetValue == "Income" || targetValue == "Expense");
                    break;
                case "Amount": validTargetValue = decimal.TryParse(targetValue, out _);
                    break;
                case "Note": validTargetValue = !string.IsNullOrEmpty(targetValue);
                    break;
                case "Tags": validTargetValue = !string.IsNullOrEmpty(targetValue);
                    break;
                case "CreatedTimestamp": validTargetValue = DateTime.TryParse(targetValue, out _);
                    break;
            }
            
            if (!validTargetValue)
                throw new ArgumentException($"{targetValue} is not a valid target value for property {propertyName}.");
        }

        private static ITransactionFilter CreateFilter(string chainingModeString, string propertyName, string comparisonOperation, string targetValue)
        {
            FilterChainingMode chainingMode = FilterChainingMode.And;

            if (chainingModeString == "OR")
                chainingMode = FilterChainingMode.Or;
            
            FilteringOption filteringOption = FilteringOption.Equal;

            switch (comparisonOperation)
            {
                case "<": filteringOption = FilteringOption.Less;
                    break;
                case "<=": filteringOption = FilteringOption.LessOrEqual;
                    break;
                case ">": filteringOption = FilteringOption.Greater;
                    break;
                case ">=": filteringOption = FilteringOption.GreaterOrEqual;
                    break;
                case "!=": filteringOption = FilteringOption.NotEqual;
                    break;
            }

            Object targetValueObj = null;

            switch (propertyName)
            {
                case "Id": targetValueObj = int.Parse(targetValue); 
                    break;
                case "Timestamp": targetValueObj = DateTime.Parse(targetValue);
                    break;
                case "Type":
                    targetValueObj = targetValue == "Income" ? TransactionType.Income : TransactionType.Expense;
                    break;
                case "Amount": targetValueObj = decimal.Parse(targetValue);
                    break;
                case "Note": targetValueObj = targetValue;
                    break;
                case "Tags": targetValueObj = targetValue;
                    break;
                case "CreatedTimestamp": targetValueObj = DateTime.Parse(targetValue);
                    break;
            }
            
            Type genericTypeParameter = PropertyNameTypeMap[propertyName];
            Type genericFilterClass = typeof(TransactionFilter<>);
            Type filterType = genericFilterClass.MakeGenericType(genericTypeParameter);

            Object filterObj = Activator.CreateInstance(filterType, propertyName, targetValueObj, filteringOption, chainingMode);

            return (ITransactionFilter)filterObj;
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
                    string query = filteringQuery.Substring(lastOperatorIndex, filteringQuery.Length - lastOperatorIndex);
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