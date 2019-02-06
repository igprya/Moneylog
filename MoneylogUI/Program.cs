using MoneylogLib;

namespace MoneylogUI
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var moneylog = new Moneylog(new Settings());
            var ui = new TuiEngine(moneylog);

            ui.Run();
        }
    }
}