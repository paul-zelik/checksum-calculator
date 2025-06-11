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

        public static (string, string) getIpv4(string trameHexa)
        {
            // Convertir la chaîne hexadécimale en tableau de bytes
            byte[] ipHeader = Enumerable.Range(0, trameHexa.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(trameHexa.Substring(x, 2), 16))
                .ToArray();

            // Extraire les adresses IP source et destination
            byte[] sourceIpBytes = new byte[4];
            byte[] destIpBytes = new byte[4];

            Array.Copy(ipHeader, 12, sourceIpBytes, 0, 4); // Adresse IP source à l'offset 12
            Array.Copy(ipHeader, 16, destIpBytes, 0, 4);   // Adresse IP destination à l'offset 16

            // Convertir les bytes en adresses IP lisibles
            string sourceIp = string.Join(".", sourceIpBytes);
            string destIp = string.Join(".", destIpBytes);

            return (sourceIp, destIp);

        }

        public static void Main()
        {
            Console.WriteLine("Main Inutile."); // Utile pour le démarage.
        }
    }
}
