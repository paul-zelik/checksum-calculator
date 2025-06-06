using System;
using System.Globalization;

namespace LibrairieSAE
{
    public class Program
    {
        // Calcule le checksum à partir d'une trame hexadécimale.
        // Si estEnteteIP = true, met à zéro les octets 10 et 11 (champ checksum IP)
        public static ushort CalculerChecksum(string trameHexa, bool estEnteteIP = true)
        {
            Span<byte> octets = stackalloc byte[trameHexa.Length / 2];
            int octetIndex = 0;

            for (int i = 0; i < trameHexa.Length; i++)
            {
                if (char.IsWhiteSpace(trameHexa[i]) || trameHexa[i] == '-')
                    continue;

                if (i + 1 >= trameHexa.Length)
                    throw new ArgumentException("Trame hexadécimale incomplète.");

                octets[octetIndex++] = byte.Parse(trameHexa.AsSpan(i, 2), NumberStyles.HexNumber);
                i++;
            }

            octets = octets.Slice(0, octetIndex);

            // Mise à zéro du champ checksum si c'est un en-tête IP complet
            if (estEnteteIP && octets.Length >= 12)
            {
                octets[10] = 0;
                octets[11] = 0;
            }

            uint somme = 0;
            int j = 0;

            for (; j + 1 < octets.Length; j += 2)
            {
                somme += (uint)((octets[j] << 8) | octets[j + 1]);
            }

            if (j < octets.Length)
                somme += (uint)(octets[j] << 8); // padding si impair

            while ((somme >> 16) != 0)
                somme = (somme & 0xFFFF) + (somme >> 16);

            return (ushort)~somme;
        }

        public static void Main()
        {
            Console.WriteLine("Entrez la trame hexadécimale (ex : 45 00 ... ou 9A A5 84 0E) :");
            string? trame = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(trame))
            {
                Console.WriteLine("Trame vide.");
                return;
            }

            Console.WriteLine("Est-ce un en-tête IP complet ? (o/n)");
            string? estEntete = Console.ReadLine();

            bool estEnteteIP = estEntete?.Trim().ToLower() == "o";

            try
            {
                ushort checksum = CalculerChecksum(trame, estEnteteIP);
                Console.WriteLine($"Checksum calculé : 0x{checksum:X4} ({checksum})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }
    }
}
