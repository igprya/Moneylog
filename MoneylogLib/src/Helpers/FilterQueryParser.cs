using System;
using System.Collections.Generic;
using System.Linq;

using MoneylogLib.Interfaces;
using MoneylogLib.Models;

namespace MoneylogLib.Helpers
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
        
        public static ITransactionFilter CreateFilter(string filteringQuery)
        {
            string[] queryElements = filteringQuery.Split(' ');

            string propertyName = queryElements[0];
            string comparisonOperation = queryElements[1];
            string targetValue = queryElements[2];
            
            ValidateQueryElements(propertyName, comparisonOperation, targetValue);

            return CreateFilter(propertyName, comparisonOperation, targetValue);
        }

        private static void ValidateQueryElements(string propertyName, string comparisonOperation, string targetValue)
        {
            string[] validPropertyNames = new[] {"Id", "Timestamp", "Type", "Amount", "Note", "Tags", "CreatedTimestamp"};
            string[] validOperations = new[] {"<", "<=", "==", ">", ">=", "!="};
            
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

        private static ITransactionFilter CreateFilter(string propertyName, string comparisonOperation, string targetValue)
        {
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

            Object filterObj = Activator.CreateInstance(filterType, propertyName, targetValueObj, filteringOption);

            return (ITransactionFilter)filterObj;
        }
        
    }
}