using System;

namespace MoneylogLib.Models
{
    public class TransactionDto : ITransaction
    {
        public int? Id { get; }
        public DateTime Date { get; }

        public TransactionType Type { get; }

        public decimal Amount { get; }

        public string Note { get; }

        public string Tags { get; }

        private TransactionDto(int? id, DateTime date, TransactionType type, decimal amount, string note, string tags)
        {
            Id = id;
            Date = date;
            Type = type;
            Amount = amount;
            Note = note;
            Tags = tags;
        }
        
        
        public static implicit operator TransactionDto(Transaction t)
        {
            return new TransactionDto(t.Id,
                t.Date,
                t.Type,
                t.Amount,
                t.Note,
                t.Tags);
        }
    }
}