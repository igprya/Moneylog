using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Ordering
{
    internal class TransactionNoteComparer : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            if (x.Note == null && x.Note == null)
                return 0;
            
            if (x.Note == null)
                return -1;

            if (y.Note == null)
                return 1;
            
            return x.Note.CompareTo(y.Note);
        }
    }
}