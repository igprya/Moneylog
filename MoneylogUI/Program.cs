using System;
using MoneylogLib;
using MoneylogLib.Helpers;

namespace MoneylogUI
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var moneylog = new Moneylog(new MoneylogSettings());
//            var all = moneylog.GetAllTransactions();
//            var filtered = moneylog.FilterTransactions("");
            moneylog.Test();

            Console.WriteLine("Done");
        }
    }
}