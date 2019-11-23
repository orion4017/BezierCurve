using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

namespace BezierCurve
{
    public class BitmapDrawer
    {
        public delegate void TransformFunction(Graphics graphics, DataManipulator manipulator);
        public TransformFunction DrawImage;
        private DataProvider data;
        private Brush pointBrush = new SolidBrush(Color.Black);
        private Pen linePen = new Pen(Color.LightPink);
        private Pen bezierPen = new Pen(Color.Black);

        public BitmapDrawer(DataProvider data)
        {
            this.data = data;
            DrawImage = new TransformFunction(DrawImageWinForms);
        }

        #region transformFunction setters
        public void SetImageFast()
        {
            DrawImage = new TransformFunction(DrawImageWinForms);
        }
        public void SetImageNaive()
        {
            DrawImage = new TransformFunction(DrawImageNaive);
        }
        public void SetImageFiltering()
        {
            DrawImage = new TransformFunction(DrawImageFiltering);
        }
        #endregion

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
            for (int i = 0; i < data.pointsCount - 1; i++)
            {
                graphics.DrawLine(bezierPen, data.BezierPoints[i, 0], data.BezierPoints[i, 1], data.BezierPoints[i + 1, 0], data.BezierPoints[i + 1, 1]);
            }
        }

        #region DrawImage functions
        private void DrawImageWinForms(Graphics graphics, DataManipulator manipulator)
        {
            if (data.image == null || data.Points.Count < 3) return;

            PointF point = new PointF(data.BezierPoints[data.index, 0] - data.image.Width / 2, data.BezierPoints[data.index, 1] - data.image.Height / 2);
            graphics.TranslateTransform((float)(point.X + data.image.Width / 2), (float)(point.Y + data.image.Height / 2));

            graphics.RotateTransform(manipulator.CalculateAngle());

            graphics.TranslateTransform(-(float)(point.X + data.image.Width / 2), -(float)(point.Y + data.image.Height / 2));

            graphics.DrawImage(data.image, point);
        }

        private void DrawImageNaive(Graphics graphics, DataManipulator manipulator)
        {
            if (data.image == null || data.Points.Count < 3) return;

            DirectBitmap image = data.directImage;
            PointF point = new PointF(data.BezierPoints[data.index, 0], data.BezierPoints[data.index, 1]);
            PointF center = new PointF(image.Width / 2, image.Height / 2);
            float angle = manipulator.CalculateAngle() * (float)Math.PI / 180f;
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    (float, float) vec = (i - center.X, j - center.Y);
                    (float, float) vec2 = (vec.Item1 * (float)Math.Cos(angle) - vec.Item2 * (float)Math.Sin(angle), vec.Item1 * (float)Math.Sin(angle) + vec.Item2 * (float)Math.Cos(angle));
                    graphics.DrawRectangle(new Pen(image.GetPixel(i, j)), new Rectangle((int)(point.X + vec2.Item1), (int)(point.Y + vec2.Item2), 1, 1));
                }
            }
        }

        private void DrawImageFiltering(Graphics graphics, DataManipulator manipulator)
        {
            throw new NotImplementedException();
        }
        #endregion

        //public Bitmap RotateImage(Bitmap img, float rotationAngle)
        //{
        //    //create an empty Bitmap image
        //    Bitmap bmp = new Bitmap(img.Width, img.Height);

        //    //turn the Bitmap into a Graphics object
        //    Graphics gfx = Graphics.FromImage(bmp);

        //    //now we set the rotation point to the center of our image
        //    gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

        //    //now rotate the image
        //    gfx.RotateTransform(rotationAngle);

        //    gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

        //    //set the InterpolationMode to HighQualityBicubic so to ensure a high
        //    //quality image once it is transformed to the specified size
        //    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

        //    //now draw our new image onto the graphics object
        //    gfx.DrawImage(img, new Point(0, 0));

        //    //dispose of our Graphics object
        //    gfx.Dispose();

        //    //return the image
        //    return bmp;
        //}
    }
}
