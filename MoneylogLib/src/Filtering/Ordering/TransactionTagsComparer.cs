using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Ordering
{
    internal class TransactionTagsComparer : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            if (x.Tags == null && y.Tags == null)
                return 0;
            
            if (x.Tags == null)
                return -1;

            if (x.Tags == null)
                return 1;
            
            return x.Tags.CompareTo(y.Tags);
        }
    }
}