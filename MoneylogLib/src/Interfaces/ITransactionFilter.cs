using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Interfaces
{
    interface ITransactionFilter
    {
        IEnumerable<Transaction> Apply(IEnumerable<Transaction> transactions);
    }
}