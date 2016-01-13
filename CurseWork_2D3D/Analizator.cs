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
        public Versh FindGround()
        {
            int lastY = _height-1;
            // Найдём самый большой сегмент, который касается нижнего края
            Versh Ground = FindLargeSegment(lastY);
            
            // Найдём его самую дальнюю вершину, чтобы вычислить его длину
            bool findFirst = false;
            int firstY = 0;
            int firstX = 0;
            int y = 0;
            while (findFirst == false && y < _height)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_v2d[y, x].Root == Ground)
                    {
                        findFirst = true;
                        firstY = y;
                        firstX = x; // для нахождения границ пока что
                        break;
                    }
                }
                y++;
            }
            // Зная длину земли, можно задать максимальное количество слоёв изображения
            int kolZ = _height - firstY;
            zCoordinates = new int[kolZ];

            List<Versh> Borders;
            int[,] steps = {{0,1},{-1,0},{0,-1},{1,0}};
            int i = firstY;
            int j = firstX;
            Versh ourRoot = _v2d[firstY, firstX].Root;
            do
            {
                for (int k = 0; k < 4; k++)
                {
                    if (ourRoot == _v2d[i + steps[k, 0], j + steps[0, 1]].Root)
                    {
                        
                    }
                }

            } while (_v2d[i,j] != _v2d[firstY, firstX]);

//            // Создадим землю 3д модели
//            for (int k = lastY, range = 0; k >= firstY; k--, range++)
//            {
//                for (int x = 0; x < _width; x++)
//                {
//                    // Оптимизировать?? 
//                    if (_v2d[k, x].Root == Ground.Root)
//                        _v2d[k, x]._z = range;
//                }
//            }
            return Ground.Root;
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
            Versh maxSegm = _v2d[range, 0];
            for (int x = 0; x < _width; x++)
            {
                if (_v2d[range, x].VershCount > max)
                {
                    max = _v2d[range, x].VershCount;
                    maxSegm = _v2d[range, x];
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
