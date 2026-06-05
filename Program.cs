using WinkelDomein;
using BetaalSysteemMock;

namespace KassaApp;

//ITEMS OP HET TICKET//
public class TicketItem
{
    public string Barcode { get; set; }
    public string Naam { get; set; }
    public int Aantal { get; set; }
    public decimal Prijs { get; set; }
    public decimal Btw { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        List<string> ticket = new();
        Console.WriteLine("==========================================");
        Console.WriteLine("            WARENHUIS OVERFLOW");
        Console.WriteLine("         Stapelplein 1, 9000 Gent");
        Console.WriteLine("            Tel: 09 234 56 78");
        Console.WriteLine("           BTW: BE 0123.456.789");
        Console.WriteLine("==========================================");
        Console.WriteLine($"  Ticket: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        Console.WriteLine($"  Datum:  {DateTime.Now:yyyy-MM-dd HH:mm}");
        Console.WriteLine("------------------------------------------");

        if (ticket.Count == 0)
        {
            Console.WriteLine("  (leeg)");
        }
        else
        {
            foreach (var item in ticket)
            {
                Console.WriteLine(item);
            }
        }

        Console.WriteLine("==========================================");
        Console.WriteLine("");

        Console.ForegroundColor = ConsoleColor.DarkGray;

        Console.WriteLine(" <scan barcode> of [barcode]<Enter> | [aantal extra]<Enter>");
        Console.WriteLine(" [D]<Enter> = verwijderen | [Z]<Enter> = undo-laatste");
        Console.WriteLine(" [K]<Enter> = betalen met Kaart | [C]<Enter> = betaald met Cash");
        Console.WriteLine(" [P]<Enter> = parkeren | [H]<Enter> = hervatten | [A]<Enter> = afbreken");

        Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine();
        Console.Write("> ");
        // ... 
    }
    // ... 
}
// ... 
