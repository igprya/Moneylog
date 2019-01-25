using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering
{
    interface ITransactionFilter
    {
        List<Transaction> Apply(IEnumerable<Transaction> transactions);

        FilterChainingMode ChainingMode { get; }
    }
}