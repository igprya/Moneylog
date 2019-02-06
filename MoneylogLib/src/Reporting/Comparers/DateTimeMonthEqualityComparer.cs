using System;
using System.Collections.Generic;

namespace MoneylogLib.Reporting.Comparers
{
    internal class DateTimeMonthEqualityComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y)
        {
            bool result = x.Month.Equals(y.Month) && x.Year.Equals(y.Year);
            return result;
        }

        public int GetHashCode(DateTime obj)
        {
            int hash = 17;
            hash = (hash * 7) + obj.Year.GetHashCode();
            hash = (hash * 7) + obj.Month.GetHashCode();
            return hash;
        }
    }
}