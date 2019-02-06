using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib.Filtering.Comparers;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Filters
{
    internal class OrderingFilter : ITransactionFilter
    {
        protected readonly string PropertyName;
        protected readonly bool Ascending;

        public ChainingMode ChainingMode { get; }
        
        public OrderingFilter(string property, string direction)
        {
            ChainingMode = ChainingMode.And;
            PropertyName = property;
            Ascending = direction == "ASC";
        }

        public List<Transaction> Apply(IEnumerable<Transaction> transactions)
        {
            var transactionList = transactions.ToList();
            transactionList.Sort(GetComparer());

            if (!Ascending)
                transactionList.Reverse();

            return transactionList;
        }

        private IComparer<Transaction> GetComparer()
        {
            switch (PropertyName)
            {
                case "Id" : return new TransactionIdComparer();
                case "Date": return new TransactionDateComparer();
                case "Amount": return new TransactionAmountComparer();
                case "Note": return new TransactionNoteComparer();
                case "Tags": return new TransactionTagsComparer();
            }
            
            throw new ArgumentException($"Unable to select a comparer for {PropertyName}.");
        }
        
        
    }
}