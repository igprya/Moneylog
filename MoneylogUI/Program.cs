using MoneylogLib;
using MoneylogLib.Helpers;

namespace MoneylogUI
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var moneylog = new Moneylog(new MoneylogSettings());
            var ui = new TuiEngine(moneylog);

            ui.Run();
        }
    }
}