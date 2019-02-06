using System;
using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Comparers
{
    internal class TransactionTypeComparer : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            if (x == null || y == null)
                throw new NullReferenceException("Unable to compare Transaction to null.");
            
            return x.Type.CompareTo(y.Type);
        }
    }
}