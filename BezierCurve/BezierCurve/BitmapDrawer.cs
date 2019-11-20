using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BezierCurve
{
    public class BitmapDrawer
    {
        private DataProvider data;
        private Brush pointBrush = new SolidBrush(Color.Black);
        private Pen linePen = new Pen(Color.LightPink);
        private Pen bezierPen = new Pen(Color.Black);

        public BitmapDrawer(DataProvider data)
        {
            this.data = data;
        }

        public void DrawPoints(Graphics graphics)
        {
            for (int i = 0; i < data.Points.Count; i++)
            {
                graphics.FillRectangle(pointBrush, data.Points[i].X - 1, data.Points[i].Y - 1, 3, 3);
            }
            //Parallel.For(0, data.points.Count, i =>
            //{
            //    Brush brush = new SolidBrush(Color.Blue);
            //    graphics.FillRectangle(brush, data.points[i].X - 1, data.points[i].Y - 1, 3, 3);
            //});
        }

        public void DrawLines(Graphics graphics)
        {
            if (data.Points.Count < 2) return;
            for (int i = 0; i < data.Points.Count - 1; i++)
            {
                graphics.DrawLine(linePen, data.Points[i].X, data.Points[i].Y, data.Points[i + 1].X, data.Points[i + 1].Y);
            }
            //Parallel.For(0, data.points.Count - 1, i =>
            //{
            //    graphics.DrawLine(blackPen, data.points[i].X, data.points[i].Y, data.points[i + 1].X, data.points[i + 1].Y);
            //});
        }

        public void DrawBezier(Graphics graphics)
        {
            if (data.Points.Count < 3) return;
            for (int i = 0; i < 100; i++)
            {
                graphics.DrawLine(bezierPen, data.BezierPoints[i, 0], data.BezierPoints[i, 1], data.BezierPoints[i+1, 0], data.BezierPoints[i+1, 1]);
            }
        }
    }
}
