using System.Threading.Tasks;
using MoneylogLib;

namespace MoneylogUI
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            var moneylog = new Moneylog(new Settings());
            var ui = new TuiEngine(moneylog);

            await ui.Run();
        }
        
    }
}