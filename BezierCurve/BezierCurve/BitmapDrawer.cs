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
                    graphics.DrawRectangle(new Pen(image.GetPixel(i, j)), (int)(point.X + vec2.Item1), (int)(point.Y + vec2.Item2), 1, 1);
                }
            }
        }

        private void DrawImageFiltering(Graphics graphics, DataManipulator manipulator)
        {
            if (data.image == null || data.Points.Count < 3) return;

            float angle = manipulator.CalculateAngle() * (float)Math.PI / 180f;
            Color[,] colors = PrepareColorTable(data.colorMap, ref angle);
            colors = ShearX(colors, angle);
            colors = ShearY(colors, angle);
            colors = ShearX(colors, angle);

            PointF point = new PointF(data.BezierPoints[data.index, 0], data.BezierPoints[data.index, 1]);
            PointF center = new PointF(colors.GetLength(0) / 2, colors.GetLength(1) / 2);
            for (int i = 0; i < colors.GetLength(0); i++)
            {
                for (int j = 0; j < colors.GetLength(1); j++)
                {
                    if (colors[i, j] == Color.Transparent) continue;
                    (float, float) vec = (i - center.X, j - center.Y);
                    graphics.DrawRectangle(new Pen(colors[i, j]), (int)(point.X + vec.Item1), (int)(point.Y + vec.Item2), 1, 1);
                }
            }

        }

        private Color[,] ShearX(Color[,] colors, float angle)
        {
            //return colors;
            float alpha = -(float)Math.Tan(angle / 2f);
            int yl = colors.GetLength(1);
            int xd = (int)(((float)colors.GetLength(1) - 1f) * alpha);
            int xl = Math.Abs(xd) + colors.GetLength(0);
            Color[,] newColors = new Color[xl, yl];

            for (int i = 0; i < newColors.GetLength(0); i++)
            {
                for (int j = 0; j < newColors.GetLength(1); j++)
                {
                    newColors[i, j] = Color.Transparent;
                }
            }

            for (int i = 0; i < colors.GetLength(0); i++)
            {
                for (int j = 0; j < colors.GetLength(1); j++)
                {
                    if (colors[i, j] == Color.Transparent) continue;
                    if (i != 0)
                    {
                        float f = frac(alpha * (float)j);
                        newColors[i + (int)(j * alpha), j] = Color.FromArgb(
                            (int)((1 - f) * colors[i, j].R + f * colors[i - 1, j].R),
                            (int)((1 - f) * colors[i, j].G + f * colors[i - 1, j].G),
                            (int)((1 - f) * colors[i, j].B + f * colors[i - 1, j].B)
                            );
                    }
                    else
                    {
                        newColors[i + (int)(j * alpha), j] = colors[i, j];
                    }
                }

            }
            return newColors;
        }

        private float frac(float number)
        {
            return number - (float)Math.Truncate(number);
        }

        private Color[,] ShearY(Color[,] colors, float angle)
        {
            float beta = (float)Math.Sin(angle);
            int xl = colors.GetLength(0);
            int yd = Math.Abs((int)(((float)colors.GetLength(0) - 1f) * beta));
            int yl = yd + colors.GetLength(1);
            Color[,] newColors = new Color[xl, yl];

            for (int i = 0; i < newColors.GetLength(0); i++)
            {
                for (int j = 0; j < newColors.GetLength(1); j++)
                {
                    newColors[i, j] = Color.Transparent;
                }
            }

            for (int i = 0; i < colors.GetLength(0); i++)
            {
                float f = Math.Abs(frac(beta * (float)i));
                for (int j = 0; j < colors.GetLength(1); j++)
                {
                    if (colors[i, j] == Color.Transparent) continue;
                    if (j != 0)
                    {
                        newColors[i, j + (int)(i * beta) + yd] = Color.FromArgb(
                            (int)((1 - f) * colors[i, j].R + f * colors[i, j - 1].R),
                            (int)((1 - f) * colors[i, j].G + f * colors[i, j - 1].G),
                            (int)((1 - f) * colors[i, j].B + f * colors[i, j - 1].B)
                            );
                    }
                    else
                    {
                        newColors[i, j + (int)(i * beta) + yd] = colors[i, j];
                    }
                }

            }
            return newColors;
        }

        private Color[,] PrepareColorTable(Color[,] colors, ref float angle)
        {
            if (angle > 0)
            {
                angle = -(2f * (float)Math.PI - angle);
            }
            //while (angle >= ((float)Math.PI) / 2)
            //{
            //    int x = colors.GetLength(0);
            //    int y = colors.GetLength(1);
            //    Color[,] tmpColor = new Color[y, x];

            //    for (int i = 0; i < x; i++)
            //    {
            //        for (int j = 0; j < y; j++)
            //        {
            //            tmpColor[y - (j + 1), i] = colors[i, j];
            //        }
            //    }

            //    colors = tmpColor;
            //    angle -= ((float)Math.PI) / 2;
            //}
            while (angle <= -((float)Math.PI) / 2)
            {
                int x = colors.GetLength(0);
                int y = colors.GetLength(1);
                Color[,] tmpColor = new Color[y, x];

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        tmpColor[j, x - (i + 1)] = colors[i, j];
                    }
                }

                colors = tmpColor;
                angle += ((float)Math.PI) / 2;
            }

            return colors;
        }
        #endregion
    }
}
