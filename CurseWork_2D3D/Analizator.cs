using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseWork_2D3D
{
    public class Analizator
    {
        public Bitmap _photo;
        public int _height;
        public int _width;
        public Versh[,] _v2d;
        public int[] zCoordinates;
        public Analizator(Bitmap photo)
        {
            _photo = photo;
            _height = photo.Height;
            _width = photo.Width;
            _v2d = Segmentation.v2d;
        }

        // Находим поверхность ("землю") 3д модели, а также максимальное количество слоёв
        public void FindGround()
        {
            int lastY = _width - 1;
            // Найдём самый большой сегмент, который касается нижнего края
            Versh Ground = FindLargeSegment(lastY);
            
            // Найдём его самую дальнюю вершину, чтобы вычислить его длину
            bool findFirst = false;
            int firstY = 0;
            int y = 0;
            while (findFirst == false || y >= _width)
            {
                for (int x = 0; x < _height; x++)
                {
                    if (_v2d[x, y].Root == Ground.Root)
                    {
                        findFirst = true;
                        firstY = y;
                        break;
                    }
                }
                y++;
            }
            // Зная длину земли, можно задать максимальное количество слоёв изображения
            int kolZ = _width - firstY;
            zCoordinates = new int[kolZ];

            // Создадим землю 3д модели
            for (int k = lastY, range = 0; k >= firstY; k--, range++)
            {
                for (int x = 0; x < _height; x++)
                {
                    // Оптимизировать?? 
                    if (_v2d[x, y].Root == Ground.Root)
                        _v2d[x, y]._z = range;
                }
            }
        }

        // Нахождение заднего фона изображения
        public void FindBackground()
        {
            // Найдём самый большой сегмент, который касается верхнего края
            Versh Background = FindLargeSegment(0);
            
            // Т.к. это фон, координата z максимально возможно отдалена от пользователя
            int z = zCoordinates.Length;

            // Зададим z координату всем вершинам сегмента
            setZ(Background, z);
        }


        // Нахождение самого большого сегмента в определённом ряду
        // Пока работает только для строк
        public Versh FindLargeSegment(int range)
        {
            int max = 1;
            Versh maxSegm = _v2d[0, range];
            for (int x = 0; x < _height; x++)
            {
                if (_v2d[x, range].VershCount > max)
                {
                    max = _v2d[x, range].VershCount;
                    maxSegm = _v2d[x, range];
                }
            }
            return maxSegm;
        }


        // метод, задающий координату Z всему сегменту
        public void setZ(Versh ourSegm, int z)
        {
            foreach (Versh v in _v2d)
            {
                if (v.Root == ourSegm.Root)
                    v._z = z;
            }
        }

    }


}
