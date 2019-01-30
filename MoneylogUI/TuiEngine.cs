using System;
using System.ComponentModel;
using System.Linq;
using MoneylogLib;
using MoneylogLib.Models;

namespace MoneylogUI
{
    using static Console;

    internal class TuiEngine
    {
        private sealed class Transaction
        {
            public DateTime Timestamp;
            public TransactionType Type;
            public decimal Amount;
            public string Tags;
            public string Note;
            public override string ToString() => $"P\t{Timestamp}\t{Type}\t{Amount}\t{Note}\t{Tags}";
        }
        
        private readonly Moneylog _moneyLog;
        
        public TuiEngine(Moneylog moneylog)
        {
            _moneyLog = moneylog;
        }

        public void Run()
        {
            string input;
            
            PrintSplash();

            do
            {
                PrintPendingTransactionsNotice();
                input = Prompt();

                try
                {
                    switch (input)
                    {
                        case "a":
                            AddTransaction();
                            break;
                        case "e":
                            EditTransaction();
                            break;
                        case "r":
                            RemoveTransaction();
                            break;
                        case "q":
                            Query();
                            break;
                        case "d":
                            DropQueue();
                            break;
                        case "c": 
                            CommitTransactions();
                            break;
                        default:
                            AddTransaction();
                            break;
                    }
                }
                catch (Exception e)
                {
                    WriteLine(e);
                }

            } while (input != "x");
        }

        private void AddTransaction()
        {
            WriteLine("*** ADDING TRANSACTION ***");
            WriteLine();
            
            var t = ReadTransaction();
            _moneyLog.AddTransaction(t.Timestamp, t.Type, t.Amount, t.Tags, t.Note);
            
            WriteLine("Done.");
        }
        
        private void EditTransaction()
        {
            WriteLine("*** EDITING TRANSACTION ***");
            WriteLine();
            
            var id = Read<int>("transaction Id", "0");

            var oldT = _moneyLog.GetTransaction(id);

            if (oldT != null)
            {
                WriteLine(oldT);
                WriteLine();
                
                WriteLine("Enter new values");
                var newT = ReadTransaction(oldT.Timestamp.ToString(), oldT.Amount.ToString(), oldT.Type.ToString(), oldT.Tags, oldT.Note);
                _moneyLog.EditTransaction(id, newT.Timestamp, newT.Type, newT.Amount, newT.Tags, newT.Note);
                
                WriteLine("Done.");
            }
            else
            {
                WriteLine($"Transaction with an Id of {id} could not be found.");    
            }
        }

        private void RemoveTransaction()
        {
            WriteLine("*** REMOVING TRANSACTION ***");
            WriteLine();

            int id = Read<int>("transaction Id", "0");
            var t = _moneyLog.GetTransaction(id);

            if (t != null)
            {
                WriteLine("Are you sure you want to remove following transaction:");
                WriteLine(t);
                
                Write("Y/N [N]: ");
                var input = ReadLine();

                if (input == "Y")
                {
                    _moneyLog.RemoveTransaction(id);
                    WriteLine("Done.");
                }
                else
                {
                    WriteLine("Aborted.");
                }
            }
            else
            {
                WriteLine($"Transaction with an Id of {id} could not be found.");    
            }
        }
        
        private void Query()
        {
            WriteLine("*** QUERYING ***");
            WriteLine();
            
            Write("Query [all]: ");
            string query = ReadLine();

            var transactions = _moneyLog.GetTransactions(query);

            foreach (var t in transactions)
                WriteLine(t);
            
            decimal totalExpense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            decimal totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            decimal balance = totalIncome - totalExpense;
 
            WriteLine("- Summary ---");
            WriteLine($"Income:\t\t{totalIncome}");
            WriteLine($"Expenses:\t{totalExpense}");
            WriteLine($"Balance:\t{balance}");
            WriteLine();
            
            WriteLine("Done.");
        }
        
        private void DropQueue()
        {
            WriteLine("*** DROPPING QUEUE ***");
            WriteLine();

            var uncommitedTransactions = _moneyLog.GetPendingTransactions();

            if (uncommitedTransactions?.Count() > 0)
            {
                WriteLine($"Are you sure you want to drop following {uncommitedTransactions.Count()} transactions:");

                foreach (var t in uncommitedTransactions)
                    WriteLine(t);
                
                Write("Y/N [N]: ");
                var input = ReadLine();

                if (input == "Y")
                {
                    _moneyLog.DropQueue();
                    WriteLine("Done.");
                }
                else
                {
                    WriteLine("Aborted.");
                }
            }
            else
            {
                WriteLine("There are no pending transactions.");
            }
        }

        private void CommitTransactions()
        {
            WriteLine("*** COMMITTING QUEUE ***");
            WriteLine();
            
            var uncommittedTransactions = _moneyLog.GetPendingTransactions();

            if (uncommittedTransactions.Count() > 0)
            {
                WriteLine($"Are you sure you want to commit following {uncommittedTransactions.Count()} transactions:");

                foreach (var t in uncommittedTransactions)
                    WriteLine(t);
                
                Write("Y/N [Y]: ");
                var input = ReadLine();

                if (input != "N")
                {
                    _moneyLog.CommitTransactions();
                    WriteLine("Done.");
                }
                else
                {
                    WriteLine("Aborted.");
                }
                    
            }
            else
            {
                WriteLine("No pending transactions to commit.");
            }
        }


        private Transaction ReadTransaction(string dDate = null, string dAmount = null, string dType = null, string dTags = null,
            string dNote = null)
        {
            var date = Read<DateTime>("Date", dDate ?? DateTime.Now.Date.ToString());
            var amount = Read<decimal>("Amount", dAmount ?? "0");
            var type = Read<TransactionType>("Type", dType ?? "Expense");
            string note = null;
            string tags = null;

            Write("Enqueue now? [Yes]: ");
            var commit = ReadLine();
            
            if (!string.IsNullOrEmpty(commit))
            {
                note = Read<string>("Note", dNote ?? "");
                tags = Read<string>("Tags", dTags ?? "");
            }

            return new Transaction
            {
                Timestamp = date,
                Amount = amount,
                Type = type,
                Note = note,
                Tags = tags
            };
        }
        
        private T Read<T>(string valueName, string defaultValue = null)
        {
            while (true)
            {
                Write($"Enter {valueName} [{defaultValue} ?]: ");
            
                string input = ReadLine();

                if (String.IsNullOrEmpty(input))
                    input = defaultValue;

                // See https://stackoverflow.com/questions/2961656/generic-tryparse
                var typeConverter = TypeDescriptor.GetConverter(typeof(T));
                if (typeConverter.CanConvertFrom(typeof(string)))
                    return (T) typeConverter.ConvertFromString(input);
                
                throw new Exception($"{typeof(T).Name} cannot be initialized from string.");
            }
        }

        private string Prompt()
        {
            Write("> ");
            var input = ReadLine();

            Clear();
            return input;
        }
        
        private void PrintPendingTransactionsNotice()
        {
            var pending = _moneyLog.GetPendingTransactions();
            if (pending?.Count() > 0)
                WriteLine($"{pending.Count()} uncommitted transactions.");
        }

        private void PrintSplash()
        {
            WriteLine("Moneylog");
            WriteLine("=========================================================");
            WriteLine("Available commands:");
            WriteLine("\ta - add new transaction.");
            WriteLine("\te - edit transaction.");
            WriteLine("\tr - remove transaction.");
            WriteLine("\tq - query transactions.");
            WriteLine("\td - drop pending transaction queue.");
            WriteLine("\tc - commit pending transactions.");
            WriteLine();
        }
        
    }
}