using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Ordering
{
    internal class TransactionTypeComparer : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            return x.Type.CompareTo(y.Type);
        }
    }
}