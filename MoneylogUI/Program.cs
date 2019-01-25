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

          
            moneylog.Test();

            Console.WriteLine("Done");
        }
    }
}