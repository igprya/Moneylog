using System;
using System.Collections.Generic;

namespace MoneylogLib.Reporting.Comparers
{
    internal class DateTimeYearEqualityComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y)
        {
            return x.Year.Equals(y.Year);
        }

        public int GetHashCode(DateTime obj)
        {
            int hash = 17;
            hash = (hash * 7) + obj.Year.GetHashCode();
            return hash;
        }
    }
}