using System;

namespace MoneylogLib.Models
{
    internal class Transaction : ITransaction
    {
        public int? Id { get; set; }
        public DateTime Timestamp { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedTimestampUtc { get; set; }

        [NonSerialized] public bool Committed = false;
        [NonSerialized] public bool Deleted = false;

        public Transaction() { }
        
        public Transaction(ITransaction transaction)
        {
            Id = transaction.Id;
            Timestamp = transaction.Timestamp;
            Type = transaction.Type;
            Amount = transaction.Amount;
            Note = transaction.Note;
            Tags = transaction.Tags;
            CreatedTimestampUtc = transaction.CreatedTimestampUtc;
        }

        public override string ToString()
        {
            return $"{Id}\t{Timestamp}\t{Type}\t{Amount}\t{Note}\t{Tags}";
        }
    }
}