using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Ordering
{
    internal class TransactionAmountComparer : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            return x.Amount.CompareTo(y.Amount);
        }
    }
}