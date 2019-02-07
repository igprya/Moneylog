using System;
using Newtonsoft.Json;

namespace MoneylogLib.Models
{
    internal class Transaction : ITransaction
    {
        public int? Id { get; }
        public DateTime Date { get; }
        public TransactionType Type { get; }
        public decimal Amount { get; }
        public string Note { get; }
        public string Tags { get; }

        [JsonConstructor]
        public Transaction(int id, DateTime date, TransactionType type, decimal amount, string note, string tags)
            : this((int?)id, date, type, amount, note, tags)
        {}
        
        public Transaction(int? id, DateTime date, TransactionType type, decimal amount, string note, string tags)
        {
            Id = id;
            Date = date;
            Type = type;
            Amount = amount;
            Note = note;
            Tags = tags;
        }
        
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