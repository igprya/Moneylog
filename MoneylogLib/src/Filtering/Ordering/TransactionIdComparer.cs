using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Ordering
{
    internal class TransactionIdComparer : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            int xId = (int) x.Id;
            int yId = (int) y.Id;

            return xId.CompareTo(yId);
        }
    }
}