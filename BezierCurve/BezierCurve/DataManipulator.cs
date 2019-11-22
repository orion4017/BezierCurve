using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

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

        #region Points adding/moving
        public bool TryNewPoint(EditablePoint location)
        {
            //if (data.Points.Count == 20)
            //{
            //    MessageBox.Show("Number of points cannot be greater than 20");
            //    return false;
            //} //not needed since Newton function was changed

            int precision = 5;
            if (data.Points.Find(x => x.X - precision < location.X && x.X + precision > location.X && x.Y - precision < location.Y && x.Y + precision > location.Y) != null)
                return false;

            data.Points.Add(location);
            return true;
        }

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
            if (location.X < 0 || location.X >= data.Width || location.Y < 0 || location.Y >= data.Height)
                return;
            data.Points[index].X = location.X;
            data.Points[index].Y = location.Y;
        }
        #endregion

        public void CalculateBezierPoints()
        {
            if (data.Points.Count < 3)
                return;

            int n = data.Points.Count - 1;
            int num = data.pointsCount - 1;
            EditablePoint[] editables = data.Points.ToArray();

            Parallel.For(0, data.pointsCount, (i =>
            {
                double x = (double)i / (double)num;
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
            double result = 1;
            double N = n;
            double I = i;
            double NI = n - i;

            double m = Math.Min(I, NI);

            for (double j = Math.Max(I, NI) + 1, y = 1; j <= N; j++, y++)
            {
                result *= j;
                if (y <= m) result /= y;
            }

            return result;
        }
        private double Power(double num, int pow)
        {
            double ret = 1f;
            for (int i = 0; i < pow; i++)
                ret *= num;

            return ret;
        }
        //private long Factorial(int n)
        //{
        //    if (n == 0)
        //        return 1;

        //    long num = 1;
        //    for (int i = 2; i <= n; i++)
        //    {
        //        num *= i;
        //    }

        //    return num;
        //}
        #endregion

        public void LoadImage(string fileName)
        {
            Bitmap img = new Bitmap(fileName);
            int width = img.Width, height = img.Height;
            if (img.Width > 200)
                width = 200;
            if (img.Height > 300)
                height = 300;

            data.image = new Bitmap(img, width, height);
        }

        public float CalculateAngle()
        {
            if (data.index == 0)
            {
                return CalculateRightAngle();
            }
            else if (data.index == data.pointsCount - 1)
            {
                return CalculateLeftAngle();
            }
            else
            {
                return (CalculateRightAngle() + CalculateLeftAngle()) / 2;
            }
        }

        private float CalculateRightAngle()
        {
            (float, float) vec1 = (data.BezierPoints[data.index + 1, 0] - data.BezierPoints[data.index, 0], data.BezierPoints[data.index + 1, 1] - data.BezierPoints[data.index, 1]);
            if(vec1.Item1 == 0)
            {
                if (vec1.Item2 > 0)
                    return 90f;
                else
                    return -90f;
            }
            double scal = -(data.BezierPoints[data.index + 1, 1] - data.BezierPoints[data.index, 1]);
            double angle = Math.Acos(scal / VectorLength(vec1));

            if (vec1.Item1 >= 0)
                return (float)((angle * 180 / Math.PI) - (double)90);
            else
                return (float)((angle * -180 / Math.PI) - (double)90);
        }

        private float CalculateLeftAngle()
        {
            (float, float) vec1 = (data.BezierPoints[data.index, 0] - data.BezierPoints[data.index-1, 0], data.BezierPoints[data.index, 1] - data.BezierPoints[data.index-1, 1]);
            if (vec1.Item1 == 0)
            {
                if (vec1.Item2 > 0)
                    return 90f;
                else
                    return -90f;
            }
            double scal = -(data.BezierPoints[data.index, 1] - data.BezierPoints[data.index - 1, 1]);
            double angle = Math.Acos(scal / VectorLength(vec1));

            if (vec1.Item1 > 0)
                return (float)((angle * 180 / Math.PI) - (double)90);
            else
                return (float)((angle * -180 / Math.PI) - (double)90);
        }

        private double VectorLength((float, float) vec)
        {
            return Math.Sqrt(vec.Item1 * vec.Item1 + vec.Item2 * vec.Item2);
        }
    }
}
