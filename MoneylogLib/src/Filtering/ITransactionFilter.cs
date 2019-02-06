using System.Collections.Generic;
using MoneylogLib.Models;

namespace MoneylogLib.Filtering
{
    internal interface ITransactionFilter
    {
        List<ITransaction> Apply(IEnumerable<ITransaction> transactions);

        ChainingMode ChainingMode { get; }
    }
}