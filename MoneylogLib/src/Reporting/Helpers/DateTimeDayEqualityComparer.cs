using System;
using System.Collections.Generic;

namespace MoneylogLib.Reporting.Helpers
{
    internal class DateTimeDayEqualityComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y)
        {
            return x.Day.Equals(y.Day) && x.Month.Equals(y.Month) && x.Year.Equals(y.Year);
        }

        public int GetHashCode(DateTime obj)
        {
            int hash = 17;
            hash = (hash * 7) + obj.Year.GetHashCode();
            hash = (hash * 7) + obj.Month.GetHashCode();
            hash = (hash * 7) + obj.Day.GetHashCode();
            return hash;
        }
    }
}