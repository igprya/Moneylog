using System;

namespace MoneylogLib.Models
{
    internal class Transaction : ITransaction
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
        public string Tags { get; set; }

        [NonSerialized] public bool Committed = false;
        [NonSerialized] public bool Deleted = false;

        public Transaction() { }
        
        public Transaction(ITransaction transaction)
        {
            Id = transaction.Id;
            Date = transaction.Date;
            Type = transaction.Type;
            Amount = transaction.Amount;
            Note = transaction.Note;
            Tags = transaction.Tags;
        }

        public override string ToString()
        {
            return $"{Id}\t{Date}\t{Type}\t{Amount}\t{Note}\t{Tags}";
        }
    }
}