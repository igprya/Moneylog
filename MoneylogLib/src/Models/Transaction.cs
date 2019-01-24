using System;
using System.Collections.Generic;

namespace MoneylogLib.Models
{
    class Transaction : ITransaction
    {
        public int? Id { get; set; }
        public DateTime TimestampUtc { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public DateTime CreatedTimestampUtc { get; set; }

        [NonSerialized] public bool Committed = false;

        public Transaction() { }
        
        public Transaction(ITransaction transaction)
        {
            Id = transaction.Id;
            TimestampUtc = transaction.TimestampUtc;
            Type = transaction.Type;
            Amount = transaction.Amount;
            Note = transaction.Note;
            Tags = transaction.Tags;
            CreatedTimestampUtc = transaction.CreatedTimestampUtc;
        }
    }
}