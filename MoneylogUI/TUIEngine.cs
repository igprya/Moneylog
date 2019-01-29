using System;
using System.Collections.Generic;
using System.Linq;
using MoneylogLib;
using MoneylogLib.Models;

namespace MoneylogUI
{
    using static Console;
    
    class TuiEngine
    {
        private readonly Moneylog _moneyLog;
        
        public TuiEngine(Moneylog moneylog)
        {
            _moneyLog = moneylog;
        }

        public void Run()
        {
            string input;

            do
            {
                Write("> ");
                input = ReadLine();

                Clear();

                switch (input)
                {
                    case "e" : AddExpense();
                        break;
                    case "i" : AddIncome();
                        break;
                    case "d" : AddTransactionDetails();
                        break;
                    case "r" : GenerateReport();
                        break;
                    case "c" : Save();
                        break;
                    case "q" : break;
                    default  : AddExpense();
                        break;
                }
            } while (input != "q");
        }

        private void AddIncome()
        {
            WriteLine("*** INCOME **");
            WriteLine();
            
            DateTime date;
            decimal amount;
            
            string input;
            
            Write("Date: ");
            input = ReadLine();
            date = input == "" ? DateTime.Now : DateTime.Parse(input);
            
            Write("Amount: ");
            input = ReadLine();
            amount = Decimal.Parse(input);
            
            _moneyLog.AddTransaction(date, TransactionType.Income, amount);
        }

        private void AddExpense()
        {
            WriteLine("*** EXPENSE **");
            WriteLine();
            
            DateTime date;
            decimal amount;
            
            string input;
            
            Write("Date: ");
            input = ReadLine();
            date = input == "" ? DateTime.Now : DateTime.Parse(input);
            
            Write("Amount: ");
            input = ReadLine();
            amount = decimal.Parse(input);
            
            _moneyLog.AddTransaction(date, TransactionType.Expense, amount);
        }

        private void Save()
        {
            _moneyLog.CommitTransactions();
        }

        private void GenerateReport()
        {
            Clear();

            WriteLine("*** REPORT GENERATION ***");
            WriteLine();
            
            Write("Special query [y/n]: ");
            string isSpecialQuery = ReadLine();

            IEnumerable<ITransaction> transactions;

            if (isSpecialQuery == "y")
            {
                Write("Query: ");
                string query = ReadLine();
                transactions = _moneyLog.FilterTransactions(query);
            }
            else
            {
                transactions = _moneyLog.GetAllTransactions();
            }
            
            foreach (var t in transactions)
                WriteLine($"{t.Id}\t{t.Timestamp.ToLocalTime().Date}\t{t.Type}\t{t.Amount}\t{t.Note}\t{t.Tags}");

            decimal totalExpense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            decimal totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            decimal balance = totalIncome - totalExpense;

            WriteLine();
            WriteLine($"Income:\t\t{totalIncome}");
            WriteLine($"Expenses:\t{totalExpense}");
            WriteLine($"Balance:\t{balance}");
        }

        private void AddTransactionDetails()
        {
            WriteLine("*** DETAILED TRANSACTION **");
            WriteLine();
            
            DateTime date;
            TransactionType type;
            decimal amount;
            string note;
            string tags;
            
            string input;
            
            Write("Date: ");
            input = ReadLine();
            date = input == "" ? DateTime.Now : DateTime.Parse(input);
            
            Write("Amount: ");
            input = ReadLine();
            amount = decimal.Parse(input);
            
            Write("Type [e/i]: ");
            input = ReadLine();
            type = input == "i" ? TransactionType.Income : TransactionType.Expense;
            
            Write("Note: ");
            input = ReadLine();
            note = input != "" ? input : null;
            
            Write("Tags:");
            input = ReadLine();
            tags = input != "" ? input : null;
            
            _moneyLog.AddTransaction(date, type, amount, tags, note);
        }
    }
}