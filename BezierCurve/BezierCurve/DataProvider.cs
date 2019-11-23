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

        public bool repeat = false;
        public bool followLine = true;
        public bool rotate = false;
        public float rotateAngle = 0;
        public float rotateAngleIncrement = 3;

        public int Width;
        public int Height;
        public int miniatureWidth;
        public int miniatureHeight;

        public List<EditablePoint> Points; // limit 20 pkt
        public int pointsCount = 301;
        public float[,] BezierPoints;

        public Bitmap image;
        public Bitmap miniature;
        public DirectBitmap directImage;

        public int index
        {
            get => _index;
            set => _index = (value >= pointsCount ? 0 : value);
        }

        public DataProvider(int width, int height, int mw, int mh)
        {
            this.Width = width;
            this.Height = height;
            this.miniatureWidth = mw;
            this.miniatureHeight = mh;

            this.Points = new List<EditablePoint>();
            this.BezierPoints = new float[pointsCount,2]; // 0 - X, 1 - Y
        }
    }
}
