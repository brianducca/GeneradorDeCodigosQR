using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QRCodeGenerator
{
    public partial class GeneradorQR : Form
    {
        private int desde = 1;
        private int hasta= 99999;
        private int height=300;
        private int width=300;
        private PdfDocument QrCodes;
        public GeneradorQR()
        {
            InitializeComponent();
            txtHasta.Text = hasta.ToString();
            txtDesde.Text = desde.ToString();
            txtHeight.Text = height.ToString();
            txtWidth.Text = width.ToString();
        }

        private void btnBuscarCarpeta_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                lblPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
           
            if (string.IsNullOrEmpty(txtCantidadQR.Text) || (string.IsNullOrEmpty(lblPath.Text)) || string.IsNullOrEmpty(txtWidth.Text) || string.IsNullOrEmpty(txtHeight.Text)
                || string.IsNullOrEmpty(txtHasta.Text) || string.IsNullOrEmpty(txtDesde.Text))
            {
                MessageBox.Show("Necesitas completar todos los datos para poder generar los QR", this.Text);
                return;
            }
            if (int.Parse(txtHasta.Text)<= int.Parse(txtDesde.Text))
            {
                MessageBox.Show("El valor desde no puede ser mayor al valor hasta", this.Text);
                return;
            }
            if (int.Parse(txtDesde.Text)+ int.Parse(txtCantidadQR.Text)> int.Parse(txtHasta.Text))
            {
                MessageBox.Show("Cantidad de codigos qr insuficientes para el desde y hasta especificado", this.Text);
                return;
            }
            progressBar1.Maximum = int.Parse(txtCantidadQR.Text);
            progressBar1.Step = 1;
            //CREATE THE .PDF
            QrCodes= new PdfDocument();

            var result = Enumerable.Range(int.Parse(txtDesde.Text), (int.Parse(txtHasta.Text)- int.Parse(txtDesde.Text))).OrderBy(g => Guid.NewGuid()).Take(int.Parse(txtCantidadQR.Text)).ToArray();
            for (int i = 0; i < int.Parse(txtCantidadQR.Text); i++)
            {

                Bitmap b =GenerateQR(int.Parse(txtWidth.Text), int.Parse(txtHeight.Text), result.GetValue(i).ToString());
                string completePath = lblPath.Text + "\\QRCodeNumber" + result.GetValue(i).ToString() + ".jpg";
                b.Save(completePath);
                AddImageToPDF(completePath,result.GetValue(i).ToString());
                progressBar1.PerformStep();
            }
            progressBar1.Value = 0;
            // Save and start View
            QrCodes.Save(lblPath.Text + "\\QrCodes.pdf");
            MessageBox.Show("Se completo la operación con éxito",this.Text);
            Process.Start(lblPath.Text + "\\QrCodes.pdf");
        }

        private void cantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
                return;
            }
        }

        public Bitmap GenerateQR(int width, int height, string text)
        {
            var bw = new ZXing.BarcodeWriter();
            var encOptions = new ZXing.Common.EncodingOptions() { Width = width, Height = height, Margin = 0 };
            bw.Options = encOptions;
            bw.Format = ZXing.BarcodeFormat.QR_CODE;
            var result = new Bitmap(bw.Write(text));
            return result;
        }

        private void AddImageToPDF(string imageLoc,string qrNumber)
        {
            // Create an empty page 
            PdfPage page = QrCodes.AddPage();
            page.Size = PdfSharp.PageSize.A4;

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);
            DrawImage(gfx, imageLoc, 50, 50,qrNumber);
        }

        void DrawImage(XGraphics gfx, string jpegSamplePath, int x, int y,string qrNumber)
        {
            XImage image = XImage.FromFile(jpegSamplePath);
            gfx.DrawImage(image, x+100,y+175, 300,300);
            XFont font = new XFont("Arial", 30, XFontStyle.Regular);
            gfx.DrawString(qrNumber, font, XBrushes.Black,x,y, XStringFormats.Center);
            
        }
    }
}
