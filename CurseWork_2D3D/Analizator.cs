using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SharpGL.OpenGLAttributes;

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
        public List<Versh> FindGround()
        {
            int lastY = _height-1;
            // Найдём самый большой сегмент, который касается нижнего края
            Versh Ground = FindLargeSegment(lastY);
            
            // Найдём его самую дальнюю вершину, чтобы вычислить его длину
            bool findFirst = false;
            int firstRow = 0;
            int firstColumn = 0;
            int y = 0;
            while (findFirst == false && y < _height)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_v2d[y, x].Root == Ground.Root)
                    {
                        findFirst = true;
                        firstRow = y;
                        firstColumn = x; // для нахождения границ пока что
                        break;
                    }
                }
                y++;
            }
            // Зная длину земли, можно задать максимальное количество слоёв изображения
            int kolZ = _height - firstRow;
            zCoordinates = new int[kolZ];

            int[,] steps = {{0,1},{1,1},{1,0},{1,-1},{0,-1},{-1,-1},{-1,0},{-1,1}};

            for (int i = 0; i < _width; i++)
            {
                if (_v2d[_height - 1, i].Root == Ground.Root)
                {
                    firstRow = _height - 1;
                    firstColumn = i;
                    break;
                }
            }
            List<Versh> borders = new List<Versh>();
            List<Versh> trash = new List<Versh>();
            borders.Add(_v2d[firstRow, firstColumn]);
            Versh first = _v2d[firstRow, firstColumn];
            Versh current = null;
            for (int k = 0; k < 8; k++)
            {
                int secondColumn = firstColumn + steps[k, 1];
                int secondRow = firstRow + steps[k, 0];
                if (secondColumn  < 0 || secondColumn  >= _width || secondRow  < 0 ||
                    secondRow >= _height)
                    continue;

                if (_v2d[secondRow, secondColumn].Root == first.Root && _v2d[secondRow, secondColumn].isBorderVersh(_height, _width) && _v2d[secondRow, secondColumn] != first)
                {
                    current = _v2d[secondRow, secondColumn];
                    borders.Add(current);
                    break;
                }
            }

            bool foundNext;
            while (current != first)
            {
                foundNext = false;
                int currentRow = current._x;
                int currentColumn = current._y;
                for (int k = 0; k < 8; k++)
                {
                    int newRow = currentRow + steps[k, 0];
                    int newColumn = currentColumn + steps[k, 1];
                    if (newRow == first._x && newColumn == first._y)
                    {
                        foreach (Versh border in borders)
                        {
                            border._z = border._y; // - _height;
                        }
                        return borders;
                    }
                    if (newColumn < 0 || newColumn >= _width || newRow < 0 ||
                        newRow >= _height)
                        continue;

                    if (_v2d[newRow, newColumn].Root == first.Root)
                        if (_v2d[newRow, newColumn].isBorderVersh(_height, _width))
                            if (borders.Contains(_v2d[newRow, newColumn]) == false)
                                if (trash.Contains(_v2d[newRow, newColumn]) == false) //для выхода из тупиков
                            {
                                current = _v2d[newRow, newColumn];
                                borders.Add(current);
                                foundNext = true;
                                break;
                            }
                }
                if (foundNext == false)
                {
                    trash.Add(borders[borders.Count - 1]);
                    borders.RemoveAt(borders.Count-1);
                    current = borders[borders.Count - 1];
                }
            }

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
            foreach (Versh border in borders)
            {
                border._z = border._y;// - _height;
            }
            return borders;
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
            return maxSegm.Root;
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
