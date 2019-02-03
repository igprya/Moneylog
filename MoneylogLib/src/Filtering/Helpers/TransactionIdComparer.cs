using System;
using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Helpers
{
    internal class TransactionIdComparer : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            if (x == null || y == null)
                throw new NullReferenceException("Unable to compare Transaction to null.");
            
            if (x.Id == null && y.Id == null)
                return 0;
            
            if (x.Id == null)
                return -1;

            if (x.Id == null)
                return 1;
            
            int xId = (int) x.Id;
            int yId = (int) y.Id;

            return xId.CompareTo(yId);
        }
    }
}