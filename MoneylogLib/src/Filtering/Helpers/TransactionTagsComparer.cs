using System;
using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Helpers
{
    internal class TransactionTagsComparer : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            if (x == null || y == null)
                throw new NullReferenceException("Unable to compare Transaction to null.");
            
            if (x.Tags == null && y.Tags == null)
                return 0;
            
            if (x.Tags == null)
                return -1;

            if (x.Tags == null)
                return 1;
            
            return String.Compare(x.Tags, y.Tags, StringComparison.Ordinal);
        }
    }
}