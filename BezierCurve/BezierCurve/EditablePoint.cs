using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BezierCurve
{
    public class EditablePoint
    {
        public int X;
        public int Y;

        public EditablePoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public EditablePoint(Point point)
        {
            this.X = point.X;
            this.Y = point.Y;
        }
    }
}
