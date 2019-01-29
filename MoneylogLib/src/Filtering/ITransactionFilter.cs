using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering
{
    internal interface ITransactionFilter
    {
        List<Transaction> Apply(IEnumerable<Transaction> transactions);

        ChainingMode ChainingMode { get; }
    }
}