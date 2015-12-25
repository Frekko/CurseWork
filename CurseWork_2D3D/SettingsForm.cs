using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurseWork_2D3D
{
    public partial class SettingsForm : Form
    {
        private Bitmap _photo;
        private Bitmap _photoEnd;
        private string settings = "";
        private double limit;
        int segmSize;
        
        public SettingsForm(Bitmap newWork)
        {
            _photo = newWork;
            InitializeComponent();
            label1.Text = "Значение лимита свёртки";
            label3.Text = "Порог минимума сегментов";
            label2.Text = "";
            label4.Text = "";
            limit = MainMenuForm._trueLimit;
            segmSize = MainMenuForm._trueSegmSize;
            textBox1.Text = limit.ToString();
            textBox2.Text = segmSize.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                limit = double.Parse(textBox1.Text);
                label2.Text = "";
            }
            catch (FormatException ex)
            {
                label2.Text = "Неверный формат, введите double";
                return;
            }
            try 
            {
                segmSize = int.Parse(textBox2.Text);
                label4.Text = "";
            }
            catch (FormatException ex)
            {
                label4.Text = "Неверный формат, введите int";
                return;
            }
            if (segmSize == 0)
            {
                label4.Text = "Не может быть равным 0!";
                return;
                
            }
            int height = _photo.Height;
            int width = _photo.Width;

            Segmentation seg = new Segmentation(_photo, limit, segmSize);
            seg.SortRebr();
            _photoEnd = seg.Segment();
            settings = "limit = " + limit + "segmSize = " + segmSize;
            new Picture(_photoEnd, settings).Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            settings = "Original";
            new Picture(_photo, settings).Show();
        }
        
        // Сохранение настроек для модели
        private void button3_Click(object sender, EventArgs e)
        {
            MainMenuForm._trueLimit = limit;
            MainMenuForm._trueSegmSize = segmSize;
        }

        // Сбросить настройки к дефолтным 
        private void button4_Click(object sender, EventArgs e)
        {
            MainMenuForm._trueLimit = 14;
            MainMenuForm._trueSegmSize = 10;
            textBox1.Text = MainMenuForm._trueLimit.ToString();
            textBox2.Text = MainMenuForm._trueSegmSize.ToString();
        }


    }
}
