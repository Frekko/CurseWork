using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph;
using SharpGL.Enumerations;


namespace CurseWork_2D3D
{
    public partial class Form1 : Form
    {
        //private double limit = 14;
        //private double segmSize = 10;
        private Bitmap _photo;
        private string _fileName;
        private int _height;
        private int _width;
        // середина модели для отрисовки
        private float srHeight;
        private float srWidth;
        private bool haveData;

        private AreaContainer ground;
        private AreaContainer sky;
        private List<AreaContainer> areaContainers;
        public Form1(Bitmap photo, string name)
        {
            _photo = photo;
            _height = _photo.Height;
            _width = _photo.Width;
            _fileName = name;
            haveData = false;
            InitializeComponent();
            
            // середина модели для отрисовки
            srHeight = _height / 2;
            srWidth = _width / 2;
        }

        private float rtri = 0;
        Texture texture = new Texture();
        

        private void openGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {

            OpenGL glWind = this.openGLControl1.OpenGL; // для удобства работы с окном вывода

            //надо накладывать 3д текстуру!
            //glWind.Enable(OpenGL.GL_TEXTURE_2D);
            //texture.Create(glWind, "testFoto2.jpg");

            glWind.Enable(OpenGL.GL_TEXTURE_2D);
            texture.Create(glWind, _fileName/*"testFoto10.jpg"*/); // задаём текстуру
            
            glWind.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT); // чистим цвета и глубины
            glWind.LoadIdentity(); // сброс системы координат к начальной позиции

            glWind.Translate(0.0f, 0.0f, -10.0f); // по сути двигаем перо, которым рисуем (f - float)
            glWind.Rotate(rtri, 0, 1, 0); // вращение системы координат (угол поворота, координаты вектора вращения)

//            texture.Bind(glWind);

            //int[] textures = new int[1000];

            // glTexCoord2f(x1 / 383.0, y1 / 383.0);
            // glTexCoord2f(x2 / 383.0, y1 / 383.0);
            // glTexCoord2f(x2 / 383.0, y2 / 383.0);
            // glTexCoord2f(x1 / 383.0, y2 / 383.0);

            // либо GUAD_STRIP попробовать
//            glWind.Begin(OpenGL.GL_POLYGON); // начинаем отрисовывать
//
//           // glWind.TexSubImage2D(OpenGL.GL_TEXTURE_2D, 0, 300, 197, 100, 100, OpenGL.GL_RGB, OpenGL.GL_UNSIGNED_BYTE, textures);
//
//            
//            glWind.Color(1.0f, 1.0f, 1.0f); // задаём цвет в RGB
//           // glWind.TexCoord(0.0f, 1.0f);
//            glWind.TexCoord(1.0 / _photo.Height * 10.0, 1.0 / _photo.Width * 50.0); 
//            glWind.Vertex(-2.0f, 2.0f, -0.5f); // задаём вершину
//            //glWind.Color(0.0f, 0.0f, 1.0f); // задаём цвет в RGB
//
//            //glWind.TexCoord(1.0f, 1.0f);
//            glWind.TexCoord(1.0 / 1920 * 500.0, 1.0 / 1200 * 50.0); 
//            glWind.Vertex(2.0f, 2.0f, -0.5f); // задаём вершину
//
//            //glWind.TexCoord(1.0f, 0.0f);
//            glWind.TexCoord(1.0 / 1920 * 745.0, 1.0 / 1200 * 125.0); 
//            glWind.Vertex(4.0f, 1.0f, -0.5f); // задаём вершину
//
//            //glWind.TexCoord(0.0f, 0.0f);
//            glWind.TexCoord(1.0 / 1920 * 500.0, 1.0 / 1200 * 350.0); 
//            glWind.Vertex(2.0f, -2.0f, -0.5f); // задаём вершину
//
//            glWind.TexCoord(1.0 / 1920 * 10.0, 1.0 / 1200 * 350.0);
//            glWind.Vertex(-2.0f, -2.0f, -0.5f); // задаём вершину

            //glWind.Vertex(0.0f, 0.0f, -1.0f); // задаём вершину

            //glWind.Color(1.0f, 0.0f, 1.0f); // задаём цвет в RGB
            //glWind.Vertex(-0.3f, 0.2f, -1.0f); // задаём вершину
            //glWind.Vertex(-0.4f, 0.3f, -1.0f); // задаём вершину
            //glWind.Vertex(-0.2f, 0.2f, -1.0f); // задаём вершину
            //glWind.Vertex(0.2f, 0.1f, -1.0f); // задаём вершину
            
            Set3DModel(glWind);

            glWind.End();
            glWind.Flush();

            rtri += 5;
        }

        public void Set3DModel(OpenGL glWind)
        {
            List<Versh> EndedRoots = new List<Versh>();
            bool alreadyDone = false;

            Analizator analizator = new Analizator(_photo);
//            List<Versh> Ground = analizator.FindGround();

            if (haveData == false)
            {
                areaContainers = analizator.FormAreas();
                haveData = true;
            }

            Set3DPoligon(Ground, glWind);

//            for (int i = 0; i < _width; i++)
//            {
//                for (int j = 0; j < _height; j++)
//                {
//                    // лишние операции при нахождении
//                    foreach (Versh v in EndedRoots)
//                    {
//                        if (Segmentation.v2d[i, j].Root == v.Root)
//                        {
//                            alreadyDone = true;
//                            break;
//                        }
//                    }
//                    if (alreadyDone == false)
//                        Set3DPoligon(Segmentation.v2d[i, j].Root.VershCount, glWind, Segmentation.v2d[i, j].Root); // количество вершин, окно куда рисуем, Root?
//                    alreadyDone = false;
//                }
//            }
        }

        public void Set3DPoligon(List<Versh> borderList, OpenGL glWind)
        {
            glWind.Begin(OpenGL.GL_POLYGON); // начинаем отрисовывать
            glWind.Color(1.0f, 1.0f, 1.0f); // задаём цвет в RGB
            float z = -1000;
            texture.Bind(glWind);
            int shtuchka = 0;
            bool shtuchkaBool = false;
            foreach (Versh versh in borderList)
            {
                glWind.TexCoord(1.0 / _width * (versh._y - (1f / _width * srHeight)), 1.0 /_height*(versh._x - (1f/_height*srWidth)));
                glWind.Vertex((float) 1/_width*(versh._y - (1f/_width*srHeight)),
                    (float)-1 / _height * (versh._x - (1f / _height * srWidth)), (float)1 / _width * (versh._z - (1f / _width * srHeight)));
                    //1f/20*(float)versh._z); // задаём вершину
            }
//            for (int i = 0; i < kol; i++) // количество углов, для автоматизации отрисовки 
//            {
//                glWind.TexCoord(1.0 / _photo.Height * 10.0, 1.0 / _photo.Width * 50.0);
//                glWind.Vertex(-2.0f, 2.0f, -0.5f); // задаём вершину
//            }
        }

        public void getTexturePart()
        {
            
        }


        private void openGLControl1_Load(object sender, EventArgs e)
        {


        }

        // Моя учебная пирамида
        private void Pyramid(object sender, EventArgs e)
        {
            OpenGL glWind = this.openGLControl1.OpenGL; // для удобства работы с окном вывода

            glWind.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT); // чистим цвета и глубины
            glWind.LoadIdentity(); // сброс системы координат к начальной позиции

            glWind.Translate(-1.5f, 0.0f, -10.0f); // по сути двигаем перо, которым рисуем (f - float)
            glWind.Rotate(rtri, 0, 1, 0); // вращение системы координат (угол поворота, координаты вектора вращения)
            glWind.Begin(OpenGL.GL_TRIANGLES); // начинаем отрисовывать

            glWind.Color(1.0f, 0.0f, 0.0f); // задаём цвет в RGB
            glWind.Vertex(0.0f, 1.0f, 0.0f); // задаём вершину
            glWind.Color(0.0f, 1.0f, 0.0f); // задаём цвет
            glWind.Vertex(-1.0f, -1.0f, 1.0f); // задаём вершину
            glWind.Color(0.0f, 0.0f, 1.0f); // задаём цвет
            glWind.Vertex(1.0f, -1.0f, 1.0f); // задаём вершину

            glWind.Color(1.0f, 0.0f, 0.0f); // задаём цвет в RGB
            glWind.Vertex(0.0f, 1.0f, 0.0f); // задаём вершину
            glWind.Color(0.0f, 0.0f, 1.0f); // задаём цвет
            glWind.Vertex(1.0f, -1.0f, 1.0f); // задаём вершину
            glWind.Color(0.0f, 1.0f, 0.0f); // задаём цвет
            glWind.Vertex(1.0f, -1.0f, -1.0f); // задаём вершину

            glWind.Color(1.0f, 0.0f, 0.0f); // задаём цвет в RGB
            glWind.Vertex(0.0f, 1.0f, 0.0f); // задаём вершину
            glWind.Color(0.0f, 1.0f, 0.0f); // задаём цвет
            glWind.Vertex(1.0f, -1.0f, -1.0f); // задаём вершину
            glWind.Color(0.0f, 0.0f, 1.0f); // задаём цвет
            glWind.Vertex(-1.0f, -1.0f, -1.0f); // задаём вершину

            glWind.Color(1.0f, 0.0f, 0.0f); // задаём цвет в RGB
            glWind.Vertex(0.0f, 1.0f, 0.0f); // задаём вершину
            glWind.Color(0.0f, 0.0f, 1.0f); // задаём цвет
            glWind.Vertex(-1.0f, -1.0f, -1.0f); // задаём вершину
            glWind.Color(0.0f, 1.0f, 0.0f); // задаём цвет
            glWind.Vertex(-1.0f, -1.0f, 1.0f); // задаём вершину

            glWind.End();
            glWind.Flush();

            rtri += 10;
        }
    }
}
