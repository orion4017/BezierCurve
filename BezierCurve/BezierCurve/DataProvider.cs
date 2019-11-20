using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BezierCurve
{
    public class DataProvider
    {
        public int Width;
        public int Heigh;

        public List<EditablePoint> Points; // limit 20 pkt
        public float[,] BezierPoints;

        public DataProvider(int width, int height)
        {
            this.Width = width;
            this.Heigh = height;

            this.Points = new List<EditablePoint>();
            this.BezierPoints = new float[101,2]; // 0 - X, 1 - Y
        }
    }
}
