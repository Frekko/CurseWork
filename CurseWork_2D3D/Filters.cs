using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurseWork_2D3D
{
    // методом Кэнни находим лайнарт
    public class Filters
    {
        private int height;
        private int width;
        public Bitmap _Foto;


        public Filters(Bitmap Foto2D)
        {
            _Foto = Foto2D;
            height = Foto2D.Height; // высота фото
            width = Foto2D.Width; // ширина фото

            /*
            // Всё домножается на 3, т.к. мы проверяем по 3-ём палитрам
            double[,] kernel = {{-3, 0, 3}, {-6, 0, 6}, {-3, 0, 3}};

            foto2 = Filters.Svertka(foto1, height, width, kernel);*/

        }
     
        ////////////////////////////////////////////////////////////////////////////
        // Два важных метода для конвертации
        // получение байтов из Битмапа
        public static byte[] GetBytes(Bitmap input)
        {
            int count = input.Height*input.Width*3; // размер нашего изображения 
            BitmapData inputD = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb); // выделяем память
            var output = new byte[count];
            Marshal.Copy(inputD.Scan0, output, 0, count); // копируем себе в массив
            input.UnlockBits(inputD); // разблокировка памяти
            return output;
        }

        // получение Битмапа из байтов
        public static Bitmap GetBitmap(byte[] input, int width, int height)
        {
            if (input.Length%3 != 0)
                return null;
                    // проверяем сможем ли мы сконвертировать обратно (должно делиться на 3, так хранятся цветные пиксели)
            var output = new Bitmap(width, height);
            BitmapData outputD = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb); // выделяем память

            Marshal.Copy(input, 0, outputD.Scan0, input.Length);
            output.UnlockBits(outputD); // разблокировка памяти
            return output;
        }

        ////////////////////////////////////////////////////////////////////////////

        // Перевод цветного изображения в градацию серого
        public static Bitmap GrayImage(Bitmap ImageBitmap)
        {
            int index = 0;
            byte[] ImageBytes = GetBytes(ImageBitmap);
            for (int i = 0; i < ImageBitmap.Width - 1; i += 3)
            {
                int GrayColor = (int) (i*0.299 + (i + 1)*0.587 + (i + 2)*0.114);
                ImageBytes[index] = (byte) GrayColor;
                ImageBytes[index++] = (byte) GrayColor;
                ImageBytes[index + 2] = (byte) GrayColor;
            }
            ImageBitmap = GetBitmap(ImageBytes, ImageBitmap.Width, ImageBitmap.Height);
            return ImageBitmap;
        }

        // алгоритм свёртки для фильтров
        public Bitmap Svertka(Bitmap foto, int height, int width, double[,] kernel) // принимает параметры ядра
        {
            // переводим наше изображение в байты
            byte[] inputBytes = GetBytes(foto);
            // создаём массив для итога с нужным размером
            byte[] outputBytes = new byte[inputBytes.Length];

            int kernelWidth = kernel.GetLength(0);
            int kernelHeight = kernel.GetLength(1);

            double sumR;
            double sumG;
            double sumB;
            double sumKernel;

            // проходим по изображению, не обрабатывая края
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    sumR = 0;
                    sumG = 0;
                    sumB = 0;
                    sumKernel = 0;

                    // проходим по ядру
                    for (int i = 0; i < kernelWidth; i++)
                    {
                        for (int j = 0; j < kernelHeight; j++)
                        {
                            int positionX = x + (i - (kernelWidth/2));
                            int positionY = y + (j - (kernelHeight/2));

                            // не обрабатываются края (при их категоричности доработаю)
                            if ((positionX < 0) || (positionX >= width) || (positionY < 0) || (positionY >= height))
                                continue;

                            // т.к. всё лежит подряд в массиве, то и умножаем позицию на 3, получаем 3 палитры подряд
                            byte r = inputBytes[3*(width*positionY + positionX) + 0];
                            byte g = inputBytes[3*(width*positionY + positionX) + 1];
                            byte b = inputBytes[3*(width*positionY + positionX) + 2];

                            double kernelValue = kernel[i, j];

                            sumR += r*kernelValue;
                            sumG += g*kernelValue;
                            sumB += b*kernelValue;

                            sumKernel += kernelValue;
                        }
                    }

                    // Нельзя делить на ноль
                    if (sumKernel <= 0)
                        sumKernel = 1;

                    // Нельзя выйти за цветовые пределы
                    sumR = sumR/sumKernel;
                    if (sumR < 0)
                        sumR = 0;
                    if (sumR > 255)
                        sumR = 255;

                    sumG = sumG/sumKernel;
                    if (sumG < 0)
                        sumG = 0;
                    if (sumG > 255)
                        sumG = 255;

                    sumB = sumB/sumKernel;
                    if (sumB < 0)
                        sumB = 0;
                    if (sumB > 255)
                        sumB = 255;

                    // Записываем результат в цвет пикселя
                    outputBytes[3*(width*y + x) + 0] = (byte) sumR;
                    outputBytes[3*(width*y + x) + 1] = (byte) sumG;
                    outputBytes[3*(width*y + x) + 2] = (byte) sumB;
                }
            }
            // Конвертируем полученные байты обратно в Битмап
            return GetBitmap(outputBytes, width, height);
        }

        // Фильтр Гаусса
        private void Gauss()
        {

            // Использует определенные значения свёртки 
            // Всё домножено на 3, т.к. смотрим по 3-ём палитрам сразу
            double[,] kernelX = { { -3, 0, 3 }, { -6, 0, 6 }, { -3, 0, 3 } };
            double[,] kernelY = { { -3, -6, -3 }, { 0, 0, 0}, { 3, 6, 3 } };
            //Bitmap fotoAfterSvertka = Svertka(_foto, height, width, kernelY);
        
            
        
        }

        // Собель
        public Bitmap Sobel(Bitmap foto)
        {

            int width = foto.Width;
            int height = foto.Height;

            // Оператор Собеля
            double[,] kernelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            double[,] kernelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            // лимит для вычисления границ
            int limit = (foto.Height * foto.Width);

            // переводим наше изображение в байты
            byte[] inputBytes = GetBytes(foto);
            // создаём массив для итога с нужным размером
            byte[] outputBytes = new byte[inputBytes.Length];

            // вдруг потом понадобится другой размер, пусть будет так 
            int kernelWidth = kernelX.GetLength(0);
            int kernelHeight = kernelX.GetLength(1);

            double sumRx;
            double sumGx;
            double sumBx;
            double sumRy;
            double sumGy;
            double sumBy;
            double sumKernelx;
            double sumKernely;

            // проходим по изображению, не обрабатывая края
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    sumRx = 0;
                    sumGx = 0;
                    sumBx = 0;
                    sumRy = 0;
                    sumGy = 0;
                    sumBy = 0;
                    sumKernelx = 0;
                    sumKernely = 0;

                    // проходим по ядру
                    for (int i = 0; i < kernelWidth; i++)
                    {
                        for (int j = 0; j < kernelHeight; j++)
                        {
                            int positionX = x + (i - (kernelWidth/2));
                            int positionY = y + (j - (kernelHeight/2));

                            // не обрабатываются края (при их категоричности доработаю)
                            if ((positionX < 0) || (positionX >= width) || (positionY < 0) || (positionY >= height))
                                continue;

                            // т.к. всё лежит подряд в массиве, то и умножаем позицию на 3, получаем 3 палитры подряд
                            byte rX = inputBytes[3*(width*positionY + positionX) + 0];
                            byte gX = inputBytes[3*(width*positionY + positionX) + 1];
                            byte bX = inputBytes[3*(width*positionY + positionX) + 2];

                            byte rY = inputBytes[3*(width*positionY + positionX) + 0];
                            byte gY = inputBytes[3*(width*positionY + positionX) + 1];
                            byte bY = inputBytes[3*(width*positionY + positionX) + 2];

                            double kernelValueX = kernelX[i, j];
                            double kernelValueY = kernelY[i, j];

                            sumRx += rX*kernelValueX;
                            sumGx += gX*kernelValueX;
                            sumBx += bX*kernelValueX;

                            sumRy += rY*kernelValueY;
                            sumGy += gY*kernelValueY;
                            sumBy += bY*kernelValueY;

                            sumKernelx += kernelValueX;
                            sumKernely += kernelValueY;

                        }
                    }
                    // В одном месте просто  ставился лимит, но это же не правильно. Мб овместить

                    if (sumRx*sumRx + sumRy*sumRy > limit || sumGx*sumGx + sumGy*sumGy > limit ||
                        sumBx*sumBx + sumBy*sumBy > limit)
                    {
                        outputBytes[3*(width*y + x) + 0] = 0;
                        outputBytes[3*(width*y + x) + 1] = 0;
                        outputBytes[3*(width*y + x) + 2] = 0;
                    }
                    else
                    {
                        outputBytes[3*(width*y + x) + 0] = 255;
                        outputBytes[3*(width*y + x) + 1] = 255;
                        outputBytes[3*(width*y + x) + 2] = 255;
                    }
                    /*
                    outputBytes[3 * (width * y + x) + 0] = (byte)Math.Sqrt(sumRx * sumRx + sumRy * sumRy);
                    outputBytes[3 * (width * y + x) + 1] = (byte)Math.Sqrt(sumGx * sumGx + sumGy * sumGy);
                    outputBytes[3 * (width * y + x) + 2] = (byte)Math.Sqrt(sumBx * sumBx + sumBy * sumBy);
                     */
                }
            }
            // Конвертируем полученные байты обратно в Битмап
            return GetBitmap(outputBytes, width, height);
        }
        
        // Алгоритм Кэнни (Canny)
        public void CannyFilter(Bitmap foto)
        {

            // избавимся от шумов фильтром Гаусса
            double[,] kernel = { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } };
            foto = Svertka(foto, height, width, kernel);
            // Обрабатывает 
        }
         
        /*
        // получаем цветовую карту нашей фотографии и обратно
        private Color ColorMap()
        {
            Color[,] colorMap = new Color[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    colorMap[i, j] = _Foto.GetPixel(j, i);
                }
            }
            return colorMap[height, width];
        }
        // Перевод обратно в BitMap
        public Bitmap inBitMap(Color _exitColorMap)
        {
            Bitmap[,] exitfoto = new Bitmap[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    exitfoto[i, j].SetPixel(j, i, _exitColorMap);
                }
            }

            return exitfoto[height,width];
        }
        */
    }

}
