using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class Form1 : Form
    {
        PictureBox pictureBox1;
        Button btnOpen, btnSave;
        Button btnGrayscale, btnNegative;
        Button btnRed, btnGreen, btnBlue;
        Button btnHistEqual;
        TrackBar tbBrightness, tbContrast;
        Label lblBrightness, lblContrast;
        Bitmap img;

        public Form1()
        {
            this.Text = "Image Processing Suite";
            this.Size = new Size(1000, 700);

            pictureBox1 = new PictureBox();
            pictureBox1.Location = new Point(20, 20);
            pictureBox1.Size = new Size(700, 600);
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(pictureBox1);

            int startX = 740;
            int yPos = 20;

            btnOpen = new Button() { Text = "Open Image", Location = new Point(startX, yPos), Size = new Size(100, 30) };
            btnOpen.Click += btnOpen_Click;
            this.Controls.Add(btnOpen);

            btnSave = new Button() { Text = "Save Image", Location = new Point(startX + 110, yPos), Size = new Size(100, 30) };
            btnSave.Click += btnSave_Click;
            this.Controls.Add(btnSave);

            yPos += 50;
            btnGrayscale = new Button() { Text = "Grayscale", Location = new Point(startX, yPos), Size = new Size(210, 30) };
            btnGrayscale.Click += btnGrayscale_Click;
            this.Controls.Add(btnGrayscale);

            yPos += 40;
            btnNegative = new Button() { Text = "Negative", Location = new Point(startX, yPos), Size = new Size(210, 30) };
            btnNegative.Click += btnNegative_Click;
            this.Controls.Add(btnNegative);

            yPos += 40;
            btnRed = new Button() { Text = "Red Channel", Location = new Point(startX, yPos), Size = new Size(65, 30) };
            btnRed.Click += btnRed_Click;
            this.Controls.Add(btnRed);

            btnGreen = new Button() { Text = "Green", Location = new Point(startX + 70, yPos), Size = new Size(65, 30) };
            btnGreen.Click += btnGreen_Click;
            this.Controls.Add(btnGreen);

            btnBlue = new Button() { Text = "Blue", Location = new Point(startX + 140, yPos), Size = new Size(70, 30) };
            btnBlue.Click += btnBlue_Click;
            this.Controls.Add(btnBlue);

            yPos += 40;
            btnHistEqual = new Button() { Text = "Histogram Equalization", Location = new Point(startX, yPos), Size = new Size(210, 30) };
            btnHistEqual.Click += btnHistEqual_Click;
            this.Controls.Add(btnHistEqual);

            yPos += 60;
            lblBrightness = new Label() { Text = "Brightness: 0", Location = new Point(startX, yPos), AutoSize = true };
            this.Controls.Add(lblBrightness);
            
            yPos += 20;
            tbBrightness = new TrackBar() { Location = new Point(startX, yPos), Size = new Size(210, 45), Minimum = -255, Maximum = 255, Value = 0, TickFrequency = 51 };
            tbBrightness.Scroll += TbBrightness_Scroll;
            this.Controls.Add(tbBrightness);

            yPos += 50;
            lblContrast = new Label() { Text = "Contrast: 0", Location = new Point(startX, yPos), AutoSize = true };
            this.Controls.Add(lblContrast);

            yPos += 20;
            tbContrast = new TrackBar() { Location = new Point(startX, yPos), Size = new Size(210, 45), Minimum = -100, Maximum = 100, Value = 0, TickFrequency = 20 };
            tbContrast.Scroll += TbContrast_Scroll;
            this.Controls.Add(tbContrast);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (img != null) img.Dispose();
                    img = new Bitmap(ofd.FileName);
                    SetImage(new Bitmap(img));
                    
                    tbBrightness.Value = 0;
                    tbContrast.Value = 0;
                    lblBrightness.Text = "Brightness: 0";
                    lblContrast.Text = "Contrast: 0";
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image to save!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat format = ImageFormat.Png;
                    if (sfd.FilterIndex == 2) format = ImageFormat.Jpeg;
                    else if (sfd.FilterIndex == 3) format = ImageFormat.Bmp;
                    
                    pictureBox1.Image.Save(sfd.FileName, format);
                }
            }
        }

        private Bitmap Get32BppImage(Bitmap original)
        {
            Bitmap clone = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(original, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            return clone;
        }

        private void SetImage(Bitmap newImg)
        {
            if (pictureBox1.Image != null && pictureBox1.Image != img) 
            {
                pictureBox1.Image.Dispose();
            }
            pictureBox1.Image = newImg;
        }

        private unsafe void btnGrayscale_Click(object sender, EventArgs e)
        {
            if (img == null) return;
            Bitmap res = Get32BppImage(img);
            BitmapData bmpData = res.LockBits(new Rectangle(0, 0, res.Width, res.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            
            byte* ptr = (byte*)bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * res.Height;

            for (int i = 0; i < bytes; i += 4)
            {
                byte gray = (byte)(ptr[i + 2] * 0.299 + ptr[i + 1] * 0.587 + ptr[i] * 0.114);
                ptr[i] = gray;
                ptr[i + 1] = gray;
                ptr[i + 2] = gray;
            }

            res.UnlockBits(bmpData);
            SetImage(res);
        }

        private unsafe void btnNegative_Click(object sender, EventArgs e)
        {
            if (img == null) return;
            Bitmap res = Get32BppImage(img);
            BitmapData bmpData = res.LockBits(new Rectangle(0, 0, res.Width, res.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            
            byte* ptr = (byte*)bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * res.Height;

            for (int i = 0; i < bytes; i += 4)
            {
                ptr[i] = (byte)(255 - ptr[i]);
                ptr[i + 1] = (byte)(255 - ptr[i + 1]);
                ptr[i + 2] = (byte)(255 - ptr[i + 2]);
            }

            res.UnlockBits(bmpData);
            SetImage(res);
        }

        private void btnRed_Click(object sender, EventArgs e) => ExtractChannel(2);
        private void btnGreen_Click(object sender, EventArgs e) => ExtractChannel(1);
        private void btnBlue_Click(object sender, EventArgs e) => ExtractChannel(0);

        private unsafe void ExtractChannel(int channelOffset)
        {
            if (img == null) return;
            Bitmap res = Get32BppImage(img);
            BitmapData bmpData = res.LockBits(new Rectangle(0, 0, res.Width, res.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* ptr = (byte*)bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * res.Height;
            
            for (int i = 0; i < bytes; i += 4)
            {
                byte val = ptr[i + channelOffset];
                ptr[i] = val;     // B
                ptr[i + 1] = val; // G
                ptr[i + 2] = val; // R
            }
            res.UnlockBits(bmpData);
            SetImage(res);
        }

        private unsafe void btnHistEqual_Click(object sender, EventArgs e)
        {
            if (img == null) return;
            Bitmap res = Get32BppImage(img);
            BitmapData bmpData = res.LockBits(new Rectangle(0, 0, res.Width, res.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* ptr = (byte*)bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * res.Height;
            int totalPixels = res.Width * res.Height;

            int[] histogram = new int[256];
            
            for (int i = 0; i < bytes; i += 4)
            {
                byte gray = (byte)(ptr[i + 2] * 0.299 + ptr[i + 1] * 0.587 + ptr[i] * 0.114);
                ptr[i] = gray;
                ptr[i + 1] = gray;
                ptr[i + 2] = gray;
                histogram[gray]++;
            }

            int[] cdf = new int[256];
            cdf[0] = histogram[0];
            for (int i = 1; i < 256; i++) cdf[i] = cdf[i - 1] + histogram[i];

            int cdfMin = 0;
            for (int i = 0; i < 256; i++) {
                if (cdf[i] > 0) {
                    cdfMin = cdf[i];
                    break;
                }
            }

            byte[] map = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                double val = (double)(cdf[i] - cdfMin) / (totalPixels - cdfMin) * 255.0;
                map[i] = (byte)Math.Clamp((int)Math.Round(val), 0, 255);
            }

            for (int i = 0; i < bytes; i += 4)
            {
                byte mapped = map[ptr[i]];
                ptr[i] = mapped;
                ptr[i + 1] = mapped;
                ptr[i + 2] = mapped;
            }

            res.UnlockBits(bmpData);
            SetImage(res);
        }

        private void TbBrightness_Scroll(object sender, EventArgs e)
        {
            lblBrightness.Text = $"Brightness: {tbBrightness.Value}";
            ApplyBrightnessContrast();
        }

        private void TbContrast_Scroll(object sender, EventArgs e)
        {
            lblContrast.Text = $"Contrast: {tbContrast.Value}";
            ApplyBrightnessContrast();
        }

        private unsafe void ApplyBrightnessContrast()
        {
            if (img == null) return;
            
            int brightness = tbBrightness.Value; 
            int contrast = tbContrast.Value;     

            double contrastFactor = (100.0 + contrast) / 100.0;
            contrastFactor *= contrastFactor;

            Bitmap res = Get32BppImage(img);
            BitmapData bmpData = res.LockBits(new Rectangle(0, 0, res.Width, res.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* ptr = (byte*)bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * res.Height;

            for (int i = 0; i < bytes; i += 4)
            {
                int b = ptr[i] + brightness;
                int g = ptr[i + 1] + brightness;
                int r = ptr[i + 2] + brightness;

                int finalB = (int)(((b / 255.0 - 0.5) * contrastFactor + 0.5) * 255.0);
                int finalG = (int)(((g / 255.0 - 0.5) * contrastFactor + 0.5) * 255.0);
                int finalR = (int)(((r / 255.0 - 0.5) * contrastFactor + 0.5) * 255.0);

                ptr[i] = (byte)Math.Max(0, Math.Min(255, finalB));
                ptr[i + 1] = (byte)Math.Max(0, Math.Min(255, finalG));
                ptr[i + 2] = (byte)Math.Max(0, Math.Min(255, finalR));
            }

            res.UnlockBits(bmpData);
            SetImage(res);
        }
    }
}