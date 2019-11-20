using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BezierCurve
{
    public class DataManipulator
    {
        private DataProvider data;
        private BitmapDrawer drawer;
        private int index = -1;

        public DataManipulator(DataProvider data, BitmapDrawer drawer)
        {
            this.data = data;
            this.drawer = drawer;
        }

        public bool TryNewPoint(EditablePoint location)
        {
            if (data.Points.Count == 20)
            {
                MessageBox.Show("Number of points cannot be greater than 20");
                return false;
            }

            int precision = 5;
            if (data.Points.Find(x => x.X - precision < location.X && x.X + precision > location.X && x.Y - precision < location.Y && x.Y + precision > location.Y) != null)
                return false;

            data.Points.Add(location);
            return true;
        }

        public void CalculateBezierPoints()
        {
            if (data.Points.Count < 3)
                return;

            int n = data.Points.Count - 1;
            EditablePoint[] editables = data.Points.ToArray();

            //for(int i=0; i<101; i++)
            //{
            //    double x = (double)i / 100;
            //    data.BezierPoints[i, 0] = 0;
            //    data.BezierPoints[i, 1] = 0;
            //    for(int j = 0; j <= n; j++)
            //    {
            //        double newton = Newton(n, j);
            //        double minustpow = Power(1 - x, n - j);
            //        double tpow = Power(x, j);
            //        data.BezierPoints[i, 0] += (float)(newton * minustpow * tpow * editables[j].X);
            //        data.BezierPoints[i, 1] += (float)(newton * minustpow * tpow * editables[j].Y);
            //    }
            //}

            Parallel.For(0, 101, (i =>
            {
                double x = (double)i / 100;
                data.BezierPoints[i, 0] = 0;
                data.BezierPoints[i, 1] = 0;
                for (int j = 0; j <= n; j++)
                {
                    double newton = Newton(n, j);
                    double minustpow = Power(1 - x, n - j);
                    double tpow = Power(x, j);
                    data.BezierPoints[i, 0] += (float)(newton * minustpow * tpow * editables[j].X);
                    data.BezierPoints[i, 1] += (float)(newton * minustpow * tpow * editables[j].Y);
                }
            }));    
        }

        #region CalculateBezierPoints auxiliary funcions
        private double Newton(int n, int i)
        {
            double N = Factorial(n);
            double I = Factorial(i);
            double NI = Factorial(n - i);

            return N/(I*NI);
        }
        private double Power(double num, int pow)
        {
            double ret = 1f;
            for (int i = 0; i < pow; i++)
                ret *= num;

            return ret;
        }
        private long Factorial(int n)
        {
            if (n == 0)
                return 1;

            long num = 1;
            for (int i = 2; i <= n; i++)
            {
                num *= i;
            }

            return num;
        }
        #endregion
        public bool TryCatchPoint(Point location)
        {
            int precision = 5;
            EditablePoint point = data.Points.Find(x => x.X - precision < location.X && x.X + precision > location.X && x.Y - precision < location.Y && x.Y + precision > location.Y);
            if (point == null)
                return false;
            index = data.Points.FindIndex(x => x.X == point.X && x.Y == point.Y);
            return true;
        }

        public void TryMovePoint(Point location)
        {
            if (location.X < 0 || location.X >= data.Width || location.Y < 0 || location.Y >= data.Heigh)
                return;
            data.Points[index].X = location.X;
            data.Points[index].Y = location.Y;
        }
    }
}
