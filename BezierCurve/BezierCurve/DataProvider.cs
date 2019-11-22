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
        private int _index = 0;

        public int Width;
        public int Height;

        public List<EditablePoint> Points; // limit 20 pkt
        public int pointsCount = 101;
        public float[,] BezierPoints;

        public Bitmap image;
        public DirectBitmap directImage;
        public int index
        {
            get => _index;
            set => _index = (value >= pointsCount ? 0 : value);
        }

        public DataProvider(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            this.Points = new List<EditablePoint>();
            this.BezierPoints = new float[pointsCount,2]; // 0 - X, 1 - Y
        }
    }
}
