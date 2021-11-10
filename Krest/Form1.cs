using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Krest
{
    public partial class Form1 : Form
    {

        float a = 1;
        float xmin = 0;
        float xmax = (float)(1);
        float h = 0.005F;
        float t = 0.001F;

        float dt = 0.00001F;

        Dictionary<int, PointF> ujprev, uj;

        private float Ut0(float x)
        {
            // return (float)Math.Sin(3 * Math.PI * x/2);
            return 1.5F + x * x;
        }

        private float psi(float x) // не используем
        {
            return 0;
        }

        private float Ux0 (float t)
        {
            return 0.9F+t;
        }

        private float Udxl(double t)
        {
            return 1.5F;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            uj.Clear();
            t += dt;
            PointF U0 = new PointF(xmin, Ux0(t));
            float x = xmin + h;
            int idx = 1;
            float U;
            uj.Add(0, U0);
            while (idx != ujprev.Count-1){
                U = ujprev[idx].Y + dt * a / (float)Math.Pow(h, 2) * (ujprev[idx + 1].Y - 2 * ujprev[idx].Y + ujprev[idx - 1].Y) + dt*x;
                uj.Add(idx, new PointF(x,U));
                x += h;
                idx++;
            }

            //uj.Add(uj.Count, new PointF(xmax, uj[idx-1].Y + h*t));
            uj.Add(uj.Count, new PointF(xmax, Udxl(t)));

            chart1.Series[0].Points.Clear();
            ujprev.Clear();
            for (int i = 0; i < uj.Count; i++)
            {
                chart1.Series[0].Points.AddXY(uj[i].X, uj[i].Y);
                ujprev.Add(i, new PointF(uj[i].X, uj[i].Y));
            }
            

        }

        public Form1()
        {
            InitializeComponent();
            ujprev = new Dictionary<int, PointF>();
            uj = new Dictionary<int, PointF>();
            init_solve();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = "timestamp-" + Convert.ToString(t);
                filename = filename.Replace(',', '.');
                using (StreamWriter sw = new StreamWriter("./" + filename, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine("t = " + Convert.ToString(t));
                    for (int i = 0; i < ujprev.Count; i++)
                    {
                        sw.WriteLine(Convert.ToString(ujprev[i].X) + "\t" + Convert.ToString(ujprev[i].Y));
                    }
                    sw.Write('\n');
                }
            } catch(Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        private void init_solve()
        {
            chart1.Series[0].Points.Clear();

            int idx = 1;
            float x = xmin;
            ujprev.Add(0, new PointF(xmin, Ux0(t)));
            x += h;
            while (  x < xmax)
            {
                PointF p = new PointF(x, Ut0(x));
                ujprev.Add(idx++, p);
                x += h;
            }
            ujprev.Add(idx, new PointF(xmax, Udxl(t)));

            for (int i = 0; i < ujprev.Count; i++)
            {
                chart1.Series[0].Points.AddXY(ujprev[i].X, (ujprev[i].Y));
            }
        }
    }
}
