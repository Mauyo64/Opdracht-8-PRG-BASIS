using WinkelDomein;
using BetaalSysteemMock;

namespace KassaApp;

// ITEMS OP HET TICKET //
public class TicketItem
{
    public string Barcode { get; set; }
    public string Naam { get; set; }
    public int Aantal { get; set; }
    public decimal Prijs { get; set; }
    public decimal Btw { get; set; }
}

// OPSLAG VAN ENKELE ITEMS //
public static class ProductCatalogus
{
    public static List<TicketItem> Producten = new()
        {
            new TicketItem { Barcode = "541001", Naam = "Doritos 250G", Prijs = 2.99m, Btw = 21 },
            new TicketItem { Barcode = "541002", Naam = "Pannekoeken", Prijs = 3.25m, Btw = 21 },
            new TicketItem { Barcode = "541003", Naam = "Bulldog Gin 500ml", Prijs = 20.00m, Btw = 21 }
        };
}

// BIJHOUDEN VAN UITGEVOERDE ACTIES //
public static class Logger
{
    private static readonly string logBestand = "system.log";

    public static void Log(string actie)
    {
        string lijn = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {actie}";
        File.AppendAllText(logBestand, lijn + Environment.NewLine);
    }
}

public class Program
{

    // KAART TICKET //
    static void PrintBon(List<TicketItem> ticket, IBetaalTerminal terminal)
    {
        decimal subtotal = ticket.Sum(x => x.Prijs * x.Aantal);
        decimal btw = ticket.Sum(x => x.Prijs * x.Aantal * (x.Btw / 100));
        decimal totaal = subtotal + btw;

        Console.WriteLine("╔══════════════════════════════╗");
        Console.WriteLine("║       BETAALTERMINAL        ║");
        Console.WriteLine("╠══════════════════════════════╣");
        Console.WriteLine($"║ Bedrag: € {totaal,10:F2}      ║");
        Console.WriteLine("║ Bied uw kaart aan...        ║");
        Console.WriteLine("╚══════════════════════════════╝");

        Thread.Sleep(500);

        BetaalDetails? resultaat = terminal.VerzoekBetaling(totaal, "Warenhuis Overflow");

        if (resultaat == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Betaling geweigerd.");
            Console.ResetColor();
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.WriteLine("==========================================");
        Console.WriteLine("            WARENHUIS OVERFLOW");
        Console.WriteLine("         Stapelplein 1, 9000 Gent");
        Console.WriteLine("           BTW: BE 0123.456.789");
        Console.WriteLine("==========================================");
        Console.WriteLine($"  Ticket: {DateTime.Now:yyyy.MM.dd.HH.mm.ss.fff}");
        Console.WriteLine($"  Datum:  {DateTime.Now:yyyy-MM-dd HH:mm}");
        Console.WriteLine("------------------------------------------");

        foreach (var item in ticket)
        {
            Console.WriteLine($"  {item.Aantal}x {item.Naam}   € {item.Prijs * item.Aantal:F2} {item.Btw}%");
        }

        Console.WriteLine("------------------------------------------");
        Console.WriteLine($"  Subtotaal excl. BTW:  € {subtotal:F2}");
        Console.WriteLine($"    BTW 21%:            € {btw:F2}");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine($"  TOTAAL:               € {totaal:F2}");
        Console.WriteLine("==========================================");
        Console.WriteLine($"  {resultaat.KaartType} {resultaat.KaartVariant}");
        Console.WriteLine($"  ****{resultaat.GemaskerdKaartnummer[^4..]}");
        Console.WriteLine("  Chip + PIN");
        Console.WriteLine($"  Bedrag:               € {totaal:F2}");
        Console.WriteLine($"  Ref: {resultaat.TransactieReferentie}");
        Console.WriteLine("==========================================");
        Console.WriteLine(" ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✓ Betaling ontvangen - €{totaal:F2}");
        Console.WriteLine(" ");
        Console.WriteLine(" ");
        Console.ResetColor();
    }
    // CASH TICKET //
    static void PrintCashBon(List<TicketItem> ticket)
    {
        decimal subtotal = ticket.Sum(x => x.Prijs * x.Aantal);
        decimal btw = ticket.Sum(x => x.Prijs * x.Aantal * (x.Btw / 100));
        decimal totaal = subtotal + btw;

        string referentie = $"CASH-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(1000000, 9999999)}";

        Console.WriteLine();
        Console.WriteLine("==========================================");
        Console.WriteLine("            WARENHUIS OVERFLOW");
        Console.WriteLine("         Stapelplein 1, 9000 Gent");
        Console.WriteLine("           BTW: BE 0123.456.789");
        Console.WriteLine("==========================================");
        Console.WriteLine($"  Ticket: {DateTime.Now:yyyy.MM.dd.HH.mm.ss.fff}");
        Console.WriteLine($"  Datum:  {DateTime.Now:yyyy-MM-dd HH:mm}");
        Console.WriteLine("------------------------------------------");

        foreach (var item in ticket)
        {
            Console.WriteLine($"  {item.Aantal}x {item.Naam}   € {(item.Prijs * item.Aantal):F2} {item.Btw}%");
        }

        Console.WriteLine("------------------------------------------");
        Console.WriteLine($"  Subtotaal excl. BTW:  € {subtotal:F2}");
        Console.WriteLine($"    BTW 21%:            € {btw:F2}");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine($"  TOTAAL:               € {totaal:F2}");
        Console.WriteLine("==========================================");
        Console.WriteLine("  Contante betaling");
        Console.WriteLine($"  Bedrag:               € {totaal:F2}");
        Console.WriteLine($"  Ref: {referentie}");
        Console.WriteLine("==========================================");
        Console.WriteLine("");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✓ Betaling ontvangen - €{totaal:F2}");
        Console.WriteLine("");
        Console.WriteLine("");
        Console.ResetColor();
    }
    public static void Main(string[] args)
    {
        // STARTSCHERM + TICKET P-ADD //
        IBetaalTerminal terminal = new MockBetaalTerminal();
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        List<TicketItem> ticket = new List<TicketItem>();
        TicketItem lastItem = null;
        List<List<TicketItem>> geparkeerdeTickets = new();
        Stack<Action> undoStack = new();

        while (true)
        {
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
                Console.WriteLine("");

            if (geparkeerdeTickets.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"  [{geparkeerdeTickets.Count} geparkeerd]");
                Console.ResetColor();

            }
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(" <scan barcode> of [barcode]<Enter> | [aantal extra]<Enter>");
                Console.WriteLine(" [D]<Enter> = verwijderen | [Z]<Enter> = undo-laatste");
                Console.WriteLine(" [K]<Enter> = betalen met Kaart | [C]<Enter> = betaald met Cash");
                Console.WriteLine(" [P]<Enter> = parkeren | [H]<Enter> = hervatten | [A]<Enter> = afbreken");

                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine();
                Console.Write("> ");

                string invoer = Console.ReadLine();

                // VERWIJDER ITEM //
                if (invoer.Equals("D", StringComparison.OrdinalIgnoreCase))
                {
                    if (ticket.Count == 0)
                    {
                        Console.WriteLine("Het ticket is leeg, er is niets om te verwijderen.");
                        Console.ReadKey();
                        continue;
                    }

                    Console.Write("Barcode: ");
                    string barcodeVerwijder = Console.ReadLine();

                    var item = ticket.FirstOrDefault(x => x.Barcode == barcodeVerwijder);
                    if (item != null)
                    {
                        item.Aantal--;
                        Logger.Log($"VERWIJDER: {item.Naam} ({item.Barcode}) - Aantal nu {item.Aantal}");
                        if (item.Aantal <= 0)
                        {
                            ticket.Remove(item);
                            Logger.Log($"ITEM VERWIJDERD: {item.Naam} ({item.Barcode}) van ticket");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Geen item met die barcode gevonden op het ticket.");
                    }

                    Console.ReadKey();
                    continue;
                }

                // BETALEN MET KAART //
                if (invoer.Equals("K", StringComparison.OrdinalIgnoreCase))
                {
                    if (ticket.Count == 0)
                    {
                        Console.WriteLine("Ticket is leeg.");
                        Console.ReadKey();
                        continue;
                    }

                    PrintBon(ticket, terminal);
                    decimal totaal = ticket.Sum(x => x.Prijs * x.Aantal * (1 + x.Btw / 100));
                    Logger.Log($"KAARTBETALING: €{totaal:F2} - {ticket.Count} items op ticket");

                    ticket = new List<TicketItem>();
                    lastItem = null;
                    continue;
                }

                // BETALEN MET CASH //
                if (invoer.Equals("C", StringComparison.OrdinalIgnoreCase))
                {
                    if (ticket.Count == 0)
                    {
                        Console.WriteLine("Ticket is leeg.");
                        Console.ReadKey();
                        continue;
                    }

                    PrintCashBon(ticket);
                    decimal totaal = ticket.Sum(x => x.Prijs * x.Aantal * (1 + x.Btw / 100));
                    Logger.Log($"CASHBETALING: €{totaal:F2} - {ticket.Count} items op ticket");

                    ticket = new List<TicketItem>();
                    lastItem = null;

                    continue;
                }

                // TICKET PARKEREN //
                if (invoer.Equals("P", StringComparison.OrdinalIgnoreCase))
                {
                    if (ticket.Count == 0)
                    {
                        Console.WriteLine("Geen ticket om te parkeren.");
                        Console.ReadKey();
                        continue;
                    }

                    geparkeerdeTickets.Add(
                        ticket.Select(x => new TicketItem
                        {
                            Barcode = x.Barcode,
                            Naam = x.Naam,
                            Aantal = x.Aantal,
                            Prijs = x.Prijs,
                            Btw = x.Btw
                        }).ToList()
                    );
                    Logger.Log($"PARKEREN: Ticket geparkeerd met {ticket.Count} items");

                    ticket = new List<TicketItem>();
                    lastItem = null;

                    Console.ReadKey();
                    continue;
                }
                
                // TICKET HERVATTEN //
                if (invoer.Equals("H", StringComparison.OrdinalIgnoreCase))
                {
                    if (geparkeerdeTickets.Count == 0)
                    {
                        Console.WriteLine("Geen geparkeerde tickets.");
                        Console.ReadKey();
                        continue;
                    }

                    Console.WriteLine("  Geparkeerde tickets:");

                    for (int i = 0; i < geparkeerdeTickets.Count; i++)
                    {
                        int totaalAantal = geparkeerdeTickets[i].Sum(x => x.Aantal);
                        Console.WriteLine($"     {i + 1}. #{DateTime.Now:yyyy.MM.dd.HH.mm.ss.fff} ({totaalAantal} producten)");
                    }

                    Console.Write("  Keuze: ");

                    if (int.TryParse(Console.ReadLine(), out int keuze)
                        && keuze >= 1
                        && keuze <= geparkeerdeTickets.Count)
                    {
                        ticket = geparkeerdeTickets[keuze - 1];
                        geparkeerdeTickets.RemoveAt(keuze - 1);

                        lastItem = ticket.LastOrDefault();
                        Logger.Log($"HERVATTEN: Ticket #{keuze} teruggehaald met {ticket.Count} items");

                    }
                    else
                    {
                        Console.WriteLine("Ongeldige keuze.");
                        Console.ReadKey();
                    }

                    continue;
                }
                
                // BARCODE LEZEN //
                if (string.IsNullOrWhiteSpace(invoer))
                    continue;

                var product = ProductCatalogus.Producten.FirstOrDefault(p => p.Barcode == invoer);
                if (product != null)
                    {
                    var existing = ticket.FirstOrDefault(x => x.Barcode == product.Barcode);

                    if (existing != null)
                    {
                        existing.Aantal++;

                        undoStack.Push(() =>
                        {
                            existing.Aantal--;
                            if (existing.Aantal <= 0)
                                ticket.Remove(existing);
                        });

                        lastItem = existing;
                        Logger.Log($"SCAN: {existing.Naam} (+1)");
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

                        undoStack.Push(() =>
                        {
                            ticket.Remove(newItem);
                        });

                        lastItem = newItem;
                        Logger.Log($"SCAN: {newItem.Naam} toegevoegd");
                    }

                    continue;
                }

                // AANTAL EXTRA TOEVOEGEN//
                if (int.TryParse(invoer, out int extraAantal))
                    {
                        if (lastItem != null)
                        {
                            int before = lastItem.Aantal;
                            lastItem.Aantal += extraAantal;

                            undoStack.Push(() =>
                        {
                            lastItem.Aantal = before;
                            });
                        }
                        else
                        {
                            Console.WriteLine("Geen laatst gescand product om aantal aan te passen.");
                            Console.ReadKey();
                        }
                        continue;
                    }

                // TICKET AFBREKEN //
                if (invoer.Equals("A", StringComparison.OrdinalIgnoreCase))
                {
                    if (ticket.Count == 0)
                    {
                        Console.WriteLine("Geen actief ticket om af te breken.");
                        Console.ReadKey();
                        continue;
                    }

                    ticket.Clear();
                    lastItem = null;

                    Console.ReadKey();

                    continue;
                }

                // UNDO //
                if (invoer.Equals("Z", StringComparison.OrdinalIgnoreCase))
                {
                    if (undoStack.Count == 0)
                    {
                        Console.WriteLine("Niets om ongedaan te maken.");
                        Console.ReadKey();
                        continue;
                    }

                    undoStack.Pop().Invoke();

                    Logger.Log("UNDO uitgevoerd (Z)");
                    continue;
                }



            }
        }
    }



