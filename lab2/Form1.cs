using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.Linq;


namespace lab2
{
    public partial class Form1 : Form
    {
        EX excel = new EX();
        double Mx = 0;
        double skoisp = 0;
        public Form1()
        {
            InitializeComponent();
            gauss();
        }

        private double[] randArray;
        private void gauss()
        {

                label1.Text = "m = ";
                textBox1.Text = "5";
                label2.Text = "σ = ";
                textBox2.Text = "10";
                label3.Text = "n = ";
                textBox3.Text = "6";
                label4.Text = "N = ";
                textBox4.Text = "10000";
                label4.Visible = true;
                textBox4.Visible = true;             
        }     

        private void calculate_button_Click(object sender, EventArgs e)
        {           
                GaussDistribution();          
        }

        // Гауссовское распределение
        private void GaussDistribution()
        {
            Random rand = new Random();
            int N = int.Parse(textBox4.Text);          // Количество генерируемых чисел
            double m = double.Parse(textBox1.Text);    // Мат. ожидание
            double sko = double.Parse(textBox2.Text);  // СКО
            int n = int.Parse(textBox3.Text);          // Число суммируемых равномерно распределённых чисел

            randArray = new double[N];
            for (int i = 0; i < N; i++)
            {
                double tmp = 0;
                for (int j = 0; j < n; j++)
                    tmp += rand.NextDouble();

                randArray[i] = m + sko * Math.Sqrt(12.0 / n) * (tmp - (double)n / 2);
            }

            CalculateStatValues();
            DrawHistogram();
            distNameLabel.Text = "Гауссовское распределение\r\nm = " + m.ToString() + ", σ = " + sko.ToString() + ", N = " + N.ToString();
        }


        // Вывод сгенерированных чисел в текстовый файл
        private void show_button_Click(object sender, EventArgs e)
        {
            if (randArray == null) return;
            StreamWriter sw = new StreamWriter("random.txt");
            for (int i = 0; i < randArray.Length; i++)
                sw.WriteLine(randArray[i].ToString(CultureInfo.InvariantCulture));
            sw.Close();

            if (File.Exists("random.txt")) Process.Start("random.txt");
        }

        // Вычисление математического ожидания, дисперсии и СКО
        private void CalculateStatValues()
        {
            Mx = randArray.Sum() / randArray.Length;
            textBox_Mx.Text = Mx.ToString();

            double Dx = randArray.Sum(t => (t - Mx) * (t - Mx)) / (randArray.Length - 1);
            textBox_Dx.Text = Dx.ToString();

            double Dx_isp = ((double)randArray.Length / ((double)randArray.Length - 1)) * Dx;
            textBox_Dx_isp.Text = Dx_isp.ToString();

            textBox_sko_isp.Text = (Math.Sqrt(Dx_isp)).ToString();
            skoisp = Math.Sqrt(Dx_isp);

            textBox_sko.Text = (Math.Sqrt(Dx)).ToString();


            double ver = (double)1-0.95;
            double k =(double)randArray.Length;
            double result =excel.st((double)1-ver,k) ;
            double sigma = (result * Math.Sqrt(Dx_isp)) / (double)Math.Sqrt(randArray.Length);
            double Mx_min_interval, Mx_max_interval;

            Mx_min_interval = Mx - sigma;
            Mx_max_interval = Mx + sigma;

            double a1 = (1 - 0.95) / 2;
            double a2 = (1 + 0.95) / 2;

            double xi1 = excel.xi(a1,(double)randArray.Length-1);
            double xi2 = excel.xi(a2, (double)randArray.Length - 1);

            double sko_min, sko_max;

            sko_min = (Math.Sqrt(randArray.Length-1) * Math.Sqrt(Dx_isp)) / Math.Sqrt(xi1);
            sko_max = (Math.Sqrt(randArray.Length-1) * Math.Sqrt(Dx_isp)) / Math.Sqrt(xi2);

            double Dx_min, Dx_max;
            Dx_min = Math.Pow(sko_min, 2);
            Dx_max = Math.Pow(sko_max,2);

            textBox5.Text = Mx_min_interval.ToString() + " - " + Mx_max_interval.ToString();
            textBox6.Text = Dx_min.ToString() + " - " + Dx_max.ToString();
            textBox7.Text = sko_min.ToString() + " - " + sko_max.ToString();
        }

        // Построить гистограмму
        private void DrawHistogram()
        {
            List<double> numbers = new List<double>(randArray);
            numbers.Sort();

            const int intervalsCount = 20;
            double width = numbers.Last() - numbers.First();

            double widthOfInterval = width / intervalsCount;

            double[] heights = new double[intervalsCount];    // Высота столбцов гистограммы
            double[] X_values = new double[intervalsCount];  // Значение по оси x

            X_values[0] = 0.0245 * width + numbers.First();
            for (int i = 1; i < intervalsCount; i++)
                X_values[i] = X_values[i - 1] + widthOfInterval;

            double[] midle_interval = new double[intervalsCount];
            double xLeft = numbers.First();           // Начало диаграммы по оси x
            double xRight = xLeft + widthOfInterval;  // Конец текущего интервала по оси x
            int j = 0;
            for (int i = 0; i < intervalsCount; i++)
            {
                midle_interval[i] = (xLeft + xRight) / 2;


                while (j < numbers.Count && xLeft <= numbers[j] && xRight > numbers[j])
                {
                    heights[i] ++;
                    j++;
                }
                heights[i] /= numbers.Count;
                xLeft = xRight;
                xRight += widthOfInterval;
            }

            
            double[] count_in_intervals = new double[intervalsCount];

            for(int i = 0; i < intervalsCount; i++)
            {
                count_in_intervals[i] = heights[i] * numbers.Count;
            }
                     

            for (int i = 1; i < intervalsCount; i++)
            {
                midle_interval[i] = midle_interval[i-1]+ widthOfInterval;
            }

            double[] zi = new double[intervalsCount];
            for (int i = 0; i < intervalsCount; i++)
            {
                zi[i] = (midle_interval[i] - Mx) / skoisp;
            }

            double[] fi = new double[intervalsCount];
            for (int i = 0; i < intervalsCount; i++)
            {
                fi[i]=excel.normdist(zi[i]);
            }

            double[] ni = new double[intervalsCount];
            for (int i = 0; i < intervalsCount; i++)
            {
                ni[i] = ((widthOfInterval* numbers.Count)/skoisp)*fi[i];
            }

            double xinabl = 0;
            for (int i = 0; i < intervalsCount; i++)
            {
                xinabl = xinabl + (Math.Pow((count_in_intervals[i] - ni[i]), 2) / ni[i]);
            }

            double xikrit = excel.xi(0.05, (double)(20 - 2 - 1));

            label12.Text = "Хи крит. = " + xikrit.ToString();
            label13.Text ="Хи набл. = "+xinabl.ToString();

            // Получим панель для рисования
            GraphPane pane = zedGraphControl1.GraphPane;
            pane.XAxis.Title.Text = "Значение величины";
            pane.YAxis.Title.Text = "Частота попадания в интервал";
            pane.Title.Text = "Гистограмма непрерывного распределения";

            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane.CurveList.Clear();

            BarItem bar = pane.AddBar("", X_values, heights, Color.AntiqueWhite);
            
            // !!! Расстояния между кластерами (группами столбиков) гистограммы = 0.0
            // У нас в кластере только один столбик.
            pane.BarSettings.MinClusterGap = 0.0f;

            pane.XAxis.Scale.Min = numbers.First();
            pane.XAxis.Scale.Max = numbers.Last();
            pane.XAxis.Scale.AlignH = AlignH.Center;

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            zedGraphControl1.AxisChange();

            // Обновляем график
            zedGraphControl1.Invalidate();
        }

    }
}
