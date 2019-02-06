using System;
using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering.Comparers
{
    internal class TransactionNoteComparer : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            if (x == null || y == null)
                throw new NullReferenceException("Unable to compare Transaction to null.");
        
            if (x.Note == null && x.Note == null)
                return 0;
            
            if (x.Note == null)
                return -1;

            if (y.Note == null)
                return 1;
            
            return String.Compare(x.Note, y.Note, StringComparison.Ordinal);
        }
    }
}