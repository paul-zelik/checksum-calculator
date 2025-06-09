using System;
using System.Globalization;
using System.Linq;

namespace LibrairieSAE
{
    public class Program
    {
        // Calcule le checksum à partir d'une trame hexadécimale.
        // Si estEnteteIP = true, met à zéro les octets 10 et 11 (champ checksum IP)
        public static ushort CalculerChecksum(string trameHexa, bool estEnteteIP = true)
        {
            // Nettoyage des caractères non-hexa
            string cleaned = new string(trameHexa.Where(c => !char.IsWhiteSpace(c) && c != '-').ToArray());

            if (cleaned.Length % 2 != 0)
                throw new ArgumentException("Trame hexadécimale incomplète.");

            // Conversion en octets
            byte[] octets = new byte[cleaned.Length / 2];
            for (int i = 0; i < cleaned.Length; i += 2)
            {
                octets[i / 2] = byte.Parse(cleaned.Substring(i, 2), NumberStyles.HexNumber);
            }

            // Mise à zéro du champ checksum si c'est un en-tête IP
            if (estEnteteIP && octets.Length >= 20)
            {
                octets[10] = 0;
                octets[11] = 0;
            }

            // Calcul du checksum
            uint somme = 0;
            int j = 0;
            for (; j + 1 < octets.Length; j += 2)
            {
                somme += (uint)((octets[j] << 8) | octets[j + 1]);
            }

            // Si impair
            if (j < octets.Length)
                somme += (uint)(octets[j] << 8);

            // Retraitement des dépassements (addition des retenues)
            while ((somme >> 16) != 0)
                somme = (somme & 0xFFFF) + (somme >> 16);

            // Complément à un
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
