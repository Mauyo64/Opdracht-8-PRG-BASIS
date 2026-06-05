using WinkelDomein;
using BetaalSysteemMock;

namespace KassaApp;

public class Program
{
    public static void Main(string[] args)
    {
        List<string> ticket = new();
        Console.WriteLine("======================================");
        Console.WriteLine("            WARENHUIS OVERFLOW");
        Console.WriteLine("         Stapelplein 1, 9000 Gent");
        Console.WriteLine("         Tel: 09 234 56 78");
        Console.WriteLine("======================================");
        Console.WriteLine($"Ticket:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        Console.WriteLine($"Datum: {DateTime.Now:yyyy-MM-dd HH:mm}");
        Console.WriteLine("--------------------------------------");

        if (ticket.Count == 0)
        {
            Console.WriteLine("(Leeg)");
        }
        else
        { foreach (var item in ticket)
            { 
                Console.WriteLine(item); 
            }
        }

        Console.WriteLine("======================================");



        // ... 
    }
    // ... 
}
// ... 