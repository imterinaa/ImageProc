using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        Bitmap image;
        Bitmap rimage;
        Bitmap gamma_image;
        public Form1()
        {
            InitializeComponent();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.png; *.jpg; *.bmp | All Files (*.*) | *.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                pictureBox2.Image = image;
                pictureBox2.Refresh();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {

                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        rimage.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GrayScale grayscale = new GrayScale();
            rimage = grayscale.Execute(image);
            pictureBox2.Image = rimage;
      
            pictureBox2.Refresh();
        }

        private void autoContrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoContrast ac = new AutoContrast();
            Bitmap resultImage = ac.Execute(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void averageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Average av = new Average();
            Bitmap resultImage = av.Execute(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void globalBorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globalpoint av = new Globalpoint();
            Bitmap resultImage = av.Execute(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void niblackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Niblack av = new Niblack();
            Bitmap resultImage = av.Execute(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();

        }

        private void globalHistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Global_Gistogramma gh = new Global_Gistogramma();
            Bitmap resultImage = gh.Execute(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void gaussToolStripMenuItem_Click(object sender, EventArgs e)
        {
           Gaussovshum g = new Gaussovshum();
            gamma_image = g.Execute(image);
           pictureBox3.Image = gamma_image;
            pictureBox3.Refresh();
        }

        private void saltAndPaperToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Salt g = new Salt();
            Bitmap resultImage = g.Execute(image);
            pictureBox3.Image = resultImage;
            pictureBox3.Refresh();
        }

        private void bilaterialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bilater g = new Bilater();
            Bitmap resultImage = g.Execute(image);
            pictureBox3.Image = resultImage;
            pictureBox3.Refresh();
        }

        private void midpointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Midpoint g = new Midpoint();
            Bitmap resultImage = g.Execute(image);
            pictureBox3.Image = resultImage;
            pictureBox3.Refresh();
        }

        private void pSNRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PSNR_SSIM g = new PSNR_SSIM();
            int res = g.PSNR(image,gamma_image);
            label3.Text = res.ToString();
            pictureBox3.Refresh();
        }

        private void sSIMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PSNR_SSIM g = new PSNR_SSIM();
            double res = g.SSIM(image, gamma_image);
            label4.Text = res.ToString();
            pictureBox2.Refresh();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }












































    internal class PSNR
    {
        public static float Execute(Bitmap compareImage, Bitmap perfImage)
        {
            if (compareImage.Size != perfImage.Size) return -1;

            float mse = ComputeMSE(compareImage, perfImage);

            float psnr = (float)(20 * Math.Log10(255f / Math.Sqrt(mse)));
            return psnr;

        }

        private static float ComputeMSE(Bitmap compareImage, Bitmap perfImage)
        {
            float sum = 0f;
            for (int i = 0; i < perfImage.Height; i++)
                for (int j = 0; j < perfImage.Width; j++)
                {
                    Color perfColor = perfImage.GetPixel(j, i);
                    Color compareColor = compareImage.GetPixel(j, i);
                    sum += CompareColors(compareColor, perfColor);
                }
            return sum / (compareImage.Width * compareImage.Height);

        }

        private static float CompareColors(Color a, Color b)
        {
            return (float)Math.Pow(GetBrightness(a) - GetBrightness(b), 2);
        }

        private static float GetBrightness(Color color)
        {
            return (byte)(.299 * color.R + .587 * color.G + .114 * color.B);
        }

    }



    internal class SSIM
    {
        public static float Execute(Bitmap compareImage, Bitmap perfImage)
        {
            const int L = 8; // bpp
            const float k1 = 0.01f;
            const float k2 = 0.03f;

            float c1 = (float)Math.Pow(L * k1, 2);
            float c2 = (float)Math.Pow(L * k2, 2);

            float comM = ComputeMean(compareImage);
            float perfM = ComputeMean(perfImage);

            float comVar = ComputeVariance(compareImage, comM);
            float perfVar = ComputeVariance(perfImage, perfM);

            float covar = ComputeCovariance(compareImage, perfImage, comM, perfM);

            float up = (2 * comM * perfM + c1) * (2 * covar + c2);
            float down = (comM * comM + perfM * perfM + c1) *
                    (comVar * comVar + perfVar * perfVar + c2);

            float ssim = up / down;

            return ssim;
        }

        private static float ComputeMean(Bitmap image)
        {
            float sum = 0f;
            for (int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width; j++)
                {
                    Color color = image.GetPixel(j, i);
                    sum += GetBrightness(color);
                }
            return sum / (image.Width * image.Height);
        }

        private static float ComputeVariance(Bitmap image, float mean)
        {
            float sum = 0f;
            for (int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width; j++)
                {
                    Color color = image.GetPixel(j, i);
                    sum += (float)Math.Pow((GetBrightness(color) - mean), 2);
                }
            return (float)Math.Sqrt(sum / ((image.Width * image.Height - 1)));
        }

        private static float ComputeCovariance(Bitmap image1, Bitmap image2, float var1, float var2)
        {
            float sum = 0f;
            for (int i = 0; i < image1.Height; i++)
                for (int j = 0; j < image1.Width; j++)
                {
                    Color color1 = image1.GetPixel(j, i);
                    Color color2 = image2.GetPixel(j, i);
                    sum += (GetBrightness(color1) - var1) * (GetBrightness(color2) - var2);
                }
            return sum / ((image1.Width * image1.Height - 1));
        }

        private static float GetBrightness(Color color)
        {
            return (byte)(.299 * color.R + .587 * color.G + .114 * color.B);
        }
    }




    internal class Gaussian
    {
        public  Bitmap Execute(Bitmap sourceImage)
        {
            Bitmap resImage = new Bitmap(sourceImage);


            var mean = ComputeMean(resImage);
            var mse = ComputeMSE(resImage, mean);

            for (int y = 0; y < sourceImage.Height; y++)
                for (int x = 0; x < sourceImage.Width; x++)
                {
                    Color color = sourceImage.GetPixel(x, y);
                    resImage.SetPixel(x, y, CalculateColor(color, mean, mse));
                }
            return resImage;
        }

        public  Color CalculateColor(Color color, float mean, float mse)
        {
            float firstHalf = 1 / (float)Math.Sqrt(2 * Math.PI * mse);
            float pow = (float)(-(Math.Pow(GetBrightness(color) - mean, 2)) / (2 * Math.Pow(mse, 2)));
            float secondHalf = (float)Math.Pow(Math.E, pow);

            byte resV = (byte)(firstHalf * secondHalf);

            return Color.FromArgb(resV);
        }

        private float ComputeMean(Bitmap image)
        {
            float sum = 0f;
            for (int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width; j++)
                {
                    Color color = image.GetPixel(j, i);
                    sum += GetBrightness(color);
                }
            return sum / (image.Width * image.Height);
        }

        private  float ComputeMSE(Bitmap image, float mean)
        {
            float sum = 0f;
            for (int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width; j++)
                {
                    Color color = image.GetPixel(i, j);

                    sum += (float)Math.Pow(GetBrightness(color) - mean, 2);
                }
            return sum / (image.Width * image.Height);

        }

        public  byte GetBrightness(Color color)
        {
            return (byte)(.299 * color.R + .587 * color.G + .114 * color.B);
        }
    }


    internal class SaltAndPaper
    {
        public  Bitmap Execute(Bitmap sourceImage, byte upBorder = 255, byte downBorder = 0)
        {
            Bitmap resImage = new Bitmap(sourceImage);

            for (int y = 0; y < sourceImage.Height; y++)
                for (int x = 0; x < sourceImage.Width; x++)
                {
                    Color color = sourceImage.GetPixel(x, y);
                    resImage.SetPixel(x, y, CalculateColor(color, upBorder, downBorder));
                }
            return resImage;
        }

        public  Color CalculateColor(Color color, byte up, byte down)
        {
            if (GetBrightness(color) == up ||
                GetBrightness(color) == down)
                return color;
            else
                return Color.Black;
        }

        public  byte GetBrightness(Color color)
        {
            return (byte)(.299 * color.R + .587 * color.G + .114 * color.B);
        }
    }


    
}
