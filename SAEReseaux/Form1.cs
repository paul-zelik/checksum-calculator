using System;
using System.Linq;
using System.Windows.Forms;
using LibrairieSAE;

namespace SAEReseaux
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Click_Calcule(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Veuillez entrer une adresse IP.");
                return;
            }

            try
            {
                // Convertit l'IP en hexadécimal (sans séparateurs)
                string valHexaIp = IPversHexa(textBox1.Text);

                // Calcule le checksum (mode brut, pas un en-tête IP)
                ushort checksum = LibrairieSAE.Program.CalculerChecksum(valHexaIp, estEnteteIP: false);
                // Affichage
                textBox2.Text = checksum.ToString();         // Décimal
                textBox3.Text = valHexaIp.ToUpper();         // IP en hexadécimal
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        public static string IPversHexa(string ip)
        {
            string[] octets = ip.Split('.');
            if (octets.Length != 4)
                throw new FormatException("L'adresse IP doit avoir 4 octets (ex : 192.168.0.1).");

            return string.Concat(octets.Select(o =>
            {
                if (!byte.TryParse(o, out byte b))
                    throw new FormatException($"Octet invalide : {o}");
                return b.ToString("X2");
            }));
        }

        public static int HexaVersDecimal(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("La chaîne hexadécimale est vide.");

            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex.Substring(2);

            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
