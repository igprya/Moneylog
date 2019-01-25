using System;

namespace MoneylogLib.Models
{
    public interface ITransaction
    {
        int? Id { get; }
        DateTime TimestampUtc { get; }
        TransactionType Type { get; }
        decimal Amount { get; }
        string Note { get; }
        string Tags { get; }
        DateTime CreatedTimestampUtc { get; }
    }
}