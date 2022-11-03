using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AForge.Imaging;

namespace WindowsFormsApp5
{
    abstract class Filters

    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);
        public Bitmap Execute(Bitmap sourceimage)
        {
            Bitmap resultImage = new Bitmap(sourceimage.Width, sourceimage.Height);
            for (int l = 0; l < sourceimage.Width; l++)
            {
                for (int k = 0; k < sourceimage.Height; k++)
                {
                    resultImage.SetPixel(l, k, calculateNewPixelColor(sourceimage, l, k));
                }
            }
            return resultImage;
        }
        public int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;

            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }

    }
    internal class GrayScale : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {


            Color sourceColor = sourceImage.GetPixel(x, y);
            var intensity = (int)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B);
            Color resultColor = Color.FromArgb(intensity, intensity, intensity);
            return resultColor;

        }
    }
    internal class Average
    {
        protected float[,] kernel = new float[3, 3] { { 1.0f / 9.0f, 1.0f / 9.0f, 1.0f / 9.0f },
                                                    { 1.0f / 9.0f, 1.0f / 9.0f, 1.0f / 9.0f },
                                                    { 1.0f / 9.0f, 1.0f / 9.0f, 1.0f / 9.0f } };

        protected Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {

            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            return Color.FromArgb(Clamp((int)resultR, 0, 255),
                                  Clamp((int)resultG, 0, 255),
                                  Clamp((int)resultB, 0, 255));
        }
        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public Bitmap Execute(Bitmap sourceImage)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int l = 0; l < sourceImage.Width; l++)
                for (int k = 0; k < sourceImage.Height; k++)
                    resultImage.SetPixel(l, k, calculateNewPixelColor(sourceImage, l, k));

            return resultImage;
        }
    }

    internal class AutoContrast
    {
        public int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;

            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }
        public Bitmap Execute(Bitmap sourceImage)
        {
            double minCR = 255, maxCR = 0;
            double minCG = 255, maxCG = 0;
            double minCB = 255, maxCB = 0;
            double Intensity = 0;
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);
                    Intensity = 0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B;
                    if (Intensity > maxCR)
                        maxCR = Intensity;
                    if (Intensity < minCR)
                        minCR = Intensity;

                    if (Intensity > maxCG)
                        maxCG = Intensity;
                    if (Intensity < minCG)
                        minCG = Intensity;

                    if (Intensity > maxCB)
                        maxCB = Intensity;
                    if (Intensity < minCB)
                        minCB = Intensity;
                }
            for (int i = 0; i < sourceImage.Width; i++)
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);
                    double new_InR = ((double)sourceColor.R - minCR) / (maxCR - minCR) * 255.0;
                    double new_InG = ((double)sourceColor.G - minCG) / (maxCR - minCG) * 255.0;
                    double new_InB = ((double)sourceColor.B - minCB) / (maxCR - minCB) * 255.0;

                    Color resultColor = Color.FromArgb(Clamp((int)new_InR, 0, 255),
                                                       Clamp((int)new_InG, 0, 255),
                                                       Clamp((int)new_InB, 0, 255));
                    resultImage.SetPixel(i, j, resultColor);
                }


            return resultImage;
        }
    }

    internal class Globalpoint
    {
        public int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;

            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }
        public Bitmap Execute(Bitmap sourceImage)
        {

            double porog = 120;
            Color pblack = Color.Black;
            Color pwhite = Color.White;

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);
                    if (sourceColor.R > porog)
                    {
                        resultImage.SetPixel(i, j, pwhite);

                    }
                    else
                    {
                        resultImage.SetPixel(i, j, pblack);
                    }

                }



            return resultImage;
        }
    }

    internal class Niblack
    {
        public Bitmap Execute(Bitmap sourceImage)
        {
            int width = sourceImage.Width;
            int height = sourceImage.Height;
            Bitmap resImage = new Bitmap(width, height);


            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    Color Color = CalculateNewPixelColor(sourceImage, x, y);
                    resImage.SetPixel(x, y, Color);

                }



            return resImage;
        }

        public byte clamp(float value, float min, float max)
        {
            return (byte)(Math.Min(Math.Max(min, value), max));
        }

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;

            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }

        public Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            float constant = -0.2f;
            int radius = 1;
            int matrixSize = (1 + 2 * radius) * (1 + 2 * radius);

            int intesitySum = 0;
            int resultI;

            int deviation;

            for (int l = -radius; l <= radius; l++)
                for (int k = -radius; k <= radius; k++)
                {
                    int idX = clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    intesitySum += (int)neighborColor.R;

                }

            resultI = (int)(intesitySum / matrixSize);
            intesitySum = 0;

            for (int l = -radius; l <= radius; l++)
                for (int k = -radius; k <= radius; k++)
                {
                    int idX = clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    intesitySum += (int)Math.Pow(neighborColor.R - resultI, 2);

                }

            deviation = (int)Math.Sqrt(intesitySum / matrixSize);

            byte T = (byte)(resultI + (byte)(constant * deviation));


            Color color = sourceImage.GetPixel(x, y);
            if (color.R >= T) return Color.White;
            else return Color.Black;

        }
    }
    internal class Global_Gistogramma
    {

        public Bitmap Execute(Bitmap sourceImage)
        {
            int width = sourceImage.Width;
            int height = sourceImage.Height;
            Bitmap res = new Bitmap(width, height);


            int[] h = CalculateHistogram(sourceImage);//гистограмма



            int hSum = h.Sum();
            int x_min = hSum * (int)0.05;

            for (int i = 0; i < 255; i++)
            {
                if (h[i] < x_min)
                {
                    x_min -= h[i];
                    h[i] = 0;
                }
                else
                {
                    h[i] -= x_min;
                }
                if (x_min == 0) break;

            }

            int x_max = hSum * (int)0.05;

            for (int i = 255; i < 0; i--)
            {
                if (x_max > h[i])
                {
                    x_max -= h[i];
                    h[i] = 0;
                }
                else
                {
                    h[i] -= x_max;
                }
                if (x_max == 0) break;

            }


            int weight = 0;
            for (int i = 0; i < 255; i++)
            {
                weight += h[i] * i;
            }

            int T = (int)(weight / h.Sum());

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    Color color = sourceImage.GetPixel(x, y);
                    if (color.G <= T) res.SetPixel(x, y, Color.White);
                    else res.SetPixel(x, y, Color.Black);

                }



            return res;
        }

        public int[] CalculateHistogram(Bitmap sourceimage)
        {
            int[] h = new int[256];

            for (int y = 0; y < sourceimage.Height; y++)
                for (int x = 0; x < sourceimage.Width; x++)
                {
                    Color color = sourceimage.GetPixel(x, y);
                    h[color.R]++;
                }

            return h;
        }

    }




    internal class Salt
    {

        public Bitmap Execute(Bitmap sourceImage)
        {
            Random rand = new Random();
            int width = sourceImage.Width;
            int height = sourceImage.Height;
            Bitmap res = new Bitmap(width, height);
            // int x = rand.Next(0, 255);
            // int y = rand.Next(0, 255);
            int l = rand.Next(0, 1000);
            for (int i = 0; i < l; i++)
            {

                int x = rand.Next(0, width);
                int y = rand.Next(0, height);
                sourceImage.SetPixel(x, y, Color.White);



            }
            for (int i = 0; i < l; i++)
            {

                int f = rand.Next(0, width);
                int r = rand.Next(0, height);
                sourceImage.SetPixel(f, r, Color.Black);



            }
            return sourceImage;

        }


    }

    internal class Gaussovshum
    {
       

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;

            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }
        public Bitmap Execute(Bitmap sourceImage)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            Random rand = new Random();
            double Mu = 0;
            double Sigma = 20;
            int size = 100;
            double[] intensity = new double[size];
            byte[] noise = new byte[sourceImage.Width * sourceImage.Height];
            int count = 0;
            double sum = 0;
            for (int i = 0; i < intensity.Length; i++)
            {
                intensity[i] = (1 / (Math.Sqrt(2 * Math.PI * Sigma))) * Math.Exp(-Math.Pow(i - Mu, 2) / (2 * Math.Pow(Sigma, 2)));
                sum += intensity[i];

            }

            for (int i = 0; i < intensity.Length; i++)
            {
                intensity[i] /= sum;
                intensity[i] *= noise.Length;
                intensity[i] = (int)Math.Floor(intensity[i]);
            }
            for (int i = 0; i < intensity.Length; i++)
            {
                for (int j = 0; j < (int)intensity[i]; j++)
                {
                    noise[count + j] = (byte)i;
                }
                count += (int)intensity[i];
            }

            for (int i = 0; i < noise.Length - count; i++)
            {
                noise[count + i] = 0;
            }

            noise = noise.OrderBy(x => rand.Next()).ToArray();
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    int res = Clamp(sourceImage.GetPixel(i, j).R + noise[sourceImage.Height * i + j], 0, 255);
                    resultImage.SetPixel(i, j, Color.FromArgb(res, res, res));
                }
            }
            return resultImage;
        } }
    internal class Bilater {
        public int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;

            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }
        public Color NewBilateralPixel(Bitmap sourceImage, int x, int y)
        {
            double sigma = 40;
            int radius = 1;
            int count = (int)Math.Pow(radius * 2 + 1, 2);
            double sum = 0;
            double sumGaus = 0;
            double gaussian1 = 0;
            double gaussian2 = 0;

            for (int l = -radius; l <= radius; l++)
            {
                for (int k = -radius; k <= radius; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    gaussian1 = 1 / (2 * Math.PI * Math.Pow(sigma, 2)) * Math.Exp(-(Math.Pow(l, 2) + Math.Pow(k, 2)) / (2 * Math.Pow(sigma, 2)));
                    gaussian2 = 1 / (Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-(Math.Pow((double)neighborColor.R / 255 - (double)sourceImage.GetPixel(x, y).R / 255, 2)) / (2 * Math.Pow(sigma, 2)));
                    sumGaus += gaussian1 * gaussian2;
                    sum += gaussian1 * gaussian2 * (double)neighborColor.R / 255;

                }
            }
            return Color.FromArgb(Clamp((int)(sum / sumGaus * 255), 0, 255),
            Clamp((int)(sum / sumGaus * 255), 0, 255),
            Clamp((int)(sum / sumGaus * 255), 0, 255));
        }

        public Bitmap Execute(Bitmap sourceImage)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, NewBilateralPixel(sourceImage, i, j));
                }
            }
            return resultImage;
        }


    }
    internal class PSNR_SSIM {
        public int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;

            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }
        public int PSNR(Bitmap sourceImage1, Bitmap sourceImage2)
        {
            double sum = 0;
            int PSNR = 0;
            double MSE = 0;
            for (int i = 0; i < sourceImage1.Width; i++)
            {
                for (int j = 0; j < sourceImage1.Height; j++)
                {
                    sum += Math.Pow(sourceImage1.GetPixel(i, j).R - sourceImage2.GetPixel(i, j).R, 2);
                }
            }
            MSE = sum / sourceImage1.Width / sourceImage1.Height;
            PSNR = (int)(20 * Math.Log10(255 / Math.Sqrt(MSE)));
            return PSNR;
        }

        public double ArithMean(Bitmap sourceImage, int x, int y)
        {
            int radius = 3;
            int count = (int)Math.Pow(radius * 2 + 1, 2);
            double sum = 0;
            for (int l = -radius; l <= radius; l++)
            {
                for (int k = -radius; k <= radius; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    sum += neighborColor.R;
                }
            }
            return sum / count;
        }
        public double MeanSquareDeviation(Bitmap sourceImage, int x, int y, double s)
        {
            int radius = 3;
            int count = (int)Math.Pow(radius * 2 + 1, 2);
            double sum = 0;

            for (int l = -radius; l <= radius; l++)
            {
                for (int k = -radius; k <= radius; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    sum += Math.Pow(neighborColor.R - s, 2);
                }
            }
            return Math.Sqrt(sum / (count - 1));
        }

        public double MeanSquareDeviationDouble(Bitmap sourceImage1, Bitmap sourceImage2, int x, int y, double s1, double s2)
        {
            int radius = 3;
            int count = (int)Math.Pow(radius * 2 + 1, 2);
            double sum = 0;
            for (int l = -radius; l <= radius; l++)
            {
                for (int k = -radius; k <= radius; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage1.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage1.Height - 1);
                    Color neighborColor1 = sourceImage1.GetPixel(idX, idY);
                    Color neighborColor2 = sourceImage2.GetPixel(idX, idY);


                    sum += (neighborColor1.R - s1) * (neighborColor2.R - s2);

                }
            }

            return sum / (count - 1);
        }

        public double SSIM(Bitmap sourceImage1, Bitmap sourceImage2)
        {
            double l = 0, c = 0, s = 0;
            double mX = 0, mY = 0, sigmaX = 0, sigmaY = 0, sigmaXY = 0;
            double SSIM = 0;
            double c1 = 6.5025, c2 = 58.5252, c3 = 2926125;
            for (int i = 0; i < sourceImage1.Width; i++)
            {
                for (int j = 0; j < sourceImage1.Height; j++)
                {
                    mX = ArithMean(sourceImage1, i, j);
                    mY = ArithMean(sourceImage2, i, j);
                    sigmaX = MeanSquareDeviation(sourceImage1, i, j, mX);
                    sigmaY = MeanSquareDeviation(sourceImage2, i, j, mY);
                    sigmaXY = MeanSquareDeviationDouble(sourceImage1, sourceImage2, i, j, mX, mY);
                    l = (2 * mX * mY + c1) / (mX * mX + mY * mY +
                    c1);
                    c = (2 * sigmaX * sigmaY + c2) / (sigmaX * sigmaX + sigmaY * sigmaY + c2);
                    s = (2 * sigmaXY + c3) / (sigmaX * sigmaY + c3);
                    SSIM += l * c * s;
                }
            }

            return SSIM / sourceImage1.Width / sourceImage1.Height;
        }

    }

    internal class Midpoint
    {
       
        public  Bitmap Execute(Bitmap source)
        {
            Bitmap resultImage = new Bitmap(source.Width, source.Height);
            for (int i = 0; i < source.Width; i++)
            {

                for (int j = 0; j < source.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(source, i, j));



                }
            }
            return resultImage;
        }
      
        private  Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {

            float resultR_max = 0;
            float resultG_max = 0;
            float resultB_max = 0;
            float resR=0,resG=0, resB = 0;
            float constanta = 0.5f;
            float resultR_min = 255;
            float resultG_min = 255;
            float resultB_min = 255;
            for (int j = -1; j <= 1; j++)
                for (int i = -1; i <= 1; i++)
                {
                    int idX = Clamp(x + i, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + j, 0, sourceImage.Height - 1);
                    
                    {
                        resR = constanta*(Math.Max(sourceImage.GetPixel(idX, idY).R, resultR_max)+Math.Min(sourceImage.GetPixel(idX, idY).G, resultR_min));
                        resG = constanta*(Math.Max(sourceImage.GetPixel(idX, idY).G, resultG_max)+Math.Min(sourceImage.GetPixel(idX, idY).G, resultG_min));
                        resB = constanta*( Math.Max(sourceImage.GetPixel(idX, idY).B, resultB_max)+Math.Min(sourceImage.GetPixel(idX, idY).G, resultB_min));
                    }
                }
            return Color.FromArgb(
                Clamp((int)resR, 0, 255),
                Clamp((int)resG, 0, 255),
                Clamp((int)resB, 0, 255));
        }
       
        public  int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;

            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }
    }
    public class CreateHistogrammm
    {
        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(sourse image);
        // Luminance
        ImageStatisticsHSL hslStatistics = new ImageStatisticsHSL(bmp);
        int[] luminanceValues = hslStatistics.Luminance.Values;
        // RGB
        ImageStatistics rgbStatistics = new ImageStatistics(bmp);
        int[] redValues = rgbStatistics.Red.Values;
        int[] greenValues = rgbStatistics.Green.Values;
        int[] blueValues = rgbStatistics.Blue.Values;
    }
}