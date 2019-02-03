using System;

namespace MoneylogLib.Models
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