using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Ordering
{
    internal class TransactionDateComparer : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            return x.Timestamp.CompareTo(y.Timestamp);
        }
    }
}