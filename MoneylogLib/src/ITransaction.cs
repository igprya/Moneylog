using System;
using MoneylogLib.Models;

namespace MoneylogLib
{
    public interface ITransaction
    {
        int? Id { get; }
        DateTime Date { get; }
        TransactionType Type { get; }
        decimal Amount { get; }
        string Note { get; }
        string Tags { get; }
    }
}