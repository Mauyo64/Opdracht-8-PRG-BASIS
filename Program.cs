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

//OPSLAG VAN ENKELE ITEMS//
public static class ProductCatalogus
{
    public static List<TicketItem> Producten = new()
        {
            new TicketItem { Barcode = "541001", Naam = "Doritos 250G", Prijs = 2.99m, Btw = 21 },
            new TicketItem { Barcode = "541002", Naam = "Pannekoeken", Prijs = 3.25m, Btw = 21 },
            new TicketItem { Barcode = "541003", Naam = "Bulldog Gin 500ml", Prijs = 20.00m, Btw = 21 }
        };
}

public class Program

{
    public static void Main(string[] args)
    {
        //STARTSCHERM + TICKET P-ADD//
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        List<TicketItem> ticket = new List<TicketItem>();
        TicketItem lastItem = null;
        while (true)
        {
            Console.Clear();
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
                    Console.WriteLine($"    {item.Aantal}x {item.Naam} € {item.Prijs:F2} {item.Btw}%");
                }
                Console.WriteLine("------------------------------------------");

                decimal TotaalminBTW = 0;
                decimal BTW = 0;

                foreach (var item in ticket)
                {
                    TotaalminBTW += item.Prijs * item.Aantal;
                    BTW += (item.Prijs * item.Aantal) * (item.Btw / 100);
                }

                decimal Totaal = TotaalminBTW + BTW;

                Console.WriteLine($"  Subtotaal excl. BTW:  € {TotaalminBTW:F2}");
                Console.WriteLine($"    BTW:                € {BTW:F2}");
                Console.WriteLine($"  TOTAAL:               € {Totaal:F2}");
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
            
            string invoer = Console.ReadLine();


            
            //BARCODE LEZEN//

            if (string.IsNullOrWhiteSpace(invoer))
                continue;
            
            var product = ProductCatalogus.Producten.FirstOrDefault(p => p.Barcode == invoer);
            if (product != null)
            {
                var existing = ticket.FirstOrDefault(x => x.Barcode == product.Barcode);
                if (existing != null)
                {
                    existing.Aantal++;
                    lastItem = existing;
                }
                else
                {
                    var newItem = new TicketItem
                    {
                        Barcode = product.Barcode,
                        Naam = product.Naam,
                        Aantal = 1,
                        Prijs = product.Prijs,
                        Btw = product.Btw
                    };
                    ticket.Add(newItem);
                    lastItem = newItem;
                }
                continue;
            }

            // AANTAL EXTRA TOEVOEGEN//
            if (int.TryParse(invoer, out int extraAantal))
            {
                if (lastItem != null)
                {
                    lastItem.Aantal += extraAantal;
                }
                else
                {
                    Console.WriteLine("Geen laatst gescand product om aantal aan te passen.");
                    Console.ReadKey();
                }
                continue;
            }



        }
    }
}
