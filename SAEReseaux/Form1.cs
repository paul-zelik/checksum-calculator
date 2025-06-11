using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibrairieSAE;

namespace SAEReseaux
{
    public partial class Form1 : Form
    {
        private bool buttonHovered = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += Form1_Paint;

            panel1.BackColor = Color.Transparent;
            panel1.Paint += Panel1_Paint;

            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = Color.Transparent;
            button1.ForeColor = Color.White;
            button1.Font = new Font("Segoe UI Semibold", 12); 
            button1.Paint += Button1_Paint;

            button1.MouseEnter += (s, e) => { buttonHovered = true; button1.Invalidate(); };
            button1.MouseLeave += (s, e) => { buttonHovered = false; button1.Invalidate(); };

            button1.Width *= 2;

            button1.Parent = panel1;
            button1.Location = new Point(button1.Location.X, button1.Location.Y);
            button1.Size = new Size(button1.Width, button1.Height);
            button1.BringToFront();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            var rect = this.ClientRectangle;

            using (var brush = new LinearGradientBrush(rect, Color.Black, Color.Black, LinearGradientMode.Vertical))
            {
                var blend = new ColorBlend(3);
                blend.Colors = new Color[]
                {
                    ColorTranslator.FromHtml("#0f172a"),
                    ColorTranslator.FromHtml("#1e3a8a"),
                    ColorTranslator.FromHtml("#312e81")
                };
                blend.Positions = new float[] { 0f, 0.5f, 1f };
                ((LinearGradientBrush)brush).InterpolationColors = blend;
                graphics.FillRectangle(brush, rect);
            }
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = panel1.ClientRectangle;
            int radius = 20;
            int shadowOffset = 10;

            GraphicsPath CreateRoundedRect(Rectangle r, int rad)
            {
                var path = new GraphicsPath();
                path.AddArc(r.X, r.Y, rad, rad, 180, 90);
                path.AddArc(r.Right - rad, r.Y, rad, rad, 270, 90);
                path.AddArc(r.Right - rad, r.Bottom - rad, rad, rad, 0, 90);
                path.AddArc(r.X, r.Bottom - rad, rad, rad, 90, 90);
                path.CloseFigure();
                return path;
            }

            Rectangle shadowRect = new Rectangle(
                rect.X + shadowOffset,
                rect.Y + shadowOffset,
                rect.Width,
                rect.Height
            );

            using (var shadowPath = CreateRoundedRect(shadowRect, radius))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(38, 59, 130, 246)))
            {
                graphics.FillPath(shadowBrush, shadowPath);
            }

            using (var mainPath = CreateRoundedRect(rect, radius))
            using (var mainBrush = new LinearGradientBrush(
                rect,
                Color.FromArgb(153, 30, 41, 59),
                Color.FromArgb(102, 30, 64, 175),
                LinearGradientMode.Vertical))
            {
                graphics.FillPath(mainBrush, mainPath);

                using (var pen = new Pen(Color.FromArgb(77, 59, 130, 246), 2))
                {
                    graphics.DrawPath(pen, mainPath);
                }
            }
        }

        private void Button1_Paint(object sender, PaintEventArgs e)
        {
            var btn = sender as Button;
            var g = e.Graphics;
            var rect = btn.ClientRectangle;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            int radius = 15;

            GraphicsPath GetRoundedRect(Rectangle r, int rad)
            {
                var path = new GraphicsPath();
                path.AddArc(r.X, r.Y, rad, rad, 180, 90);
                path.AddArc(r.Right - rad, r.Y, rad, rad, 270, 90);
                path.AddArc(r.Right - rad, r.Bottom - rad, rad, rad, 0, 90);
                path.AddArc(r.X, r.Bottom - rad, rad, rad, 90, 90);
                path.CloseFigure();
                return path;
            }

            Rectangle glowRect = Rectangle.Inflate(rect, 15, 15);
            using (var glowPath = GetRoundedRect(glowRect, radius + 15))
            using (var brush = new PathGradientBrush(glowPath))
            {
                brush.CenterColor = buttonHovered
                    ? Color.FromArgb(200, 100, 150, 255)  
                    : Color.FromArgb(100, 100, 150, 255); 

                brush.SurroundColors = new Color[] { Color.FromArgb(0, 100, 150, 255) };
                g.FillPath(brush, glowPath);
            }

            using (var path = GetRoundedRect(rect, radius))
            using (var brush = new SolidBrush(Color.FromArgb(220, 220, 220)))
            {
                g.FillPath(brush, path);
            }

            TextRenderer.DrawText(
                g,
                btn.Text,
                btn.Font,
                rect,
                Color.Black,
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        private void Click_Calcule(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Veuillez entrer une trame hexadecimale.");
                return;
            }

            try
            {
                string trameHexa = textBox1.Text;
                ushort checksum = LibrairieSAE.Program.CalculerChecksum(trameHexa, estEnteteIP: true);
                string env = LibrairieSAE.Program.getIpv4(trameHexa).Item1;
                string dest = LibrairieSAE.Program.getIpv4(trameHexa).Item2;
                textBox2.Text = checksum.ToString();
                textBox3.Text = dest;
                textBox4.Text = env;
                button1.Enabled = true;



            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        public static int HexaVersDecimal(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("La chaine hexadecimale est vide.");

            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex.Substring(2);

            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        private void label5_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
