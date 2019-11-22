using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BezierCurve
{
    public partial class Form1 : Form
    {
        DataProvider data;
        DataManipulator manipulator;
        BitmapDrawer drawer;
        public Form1()
        {
            InitializeComponent();
            data = new DataProvider(pictureBox1.Width, pictureBox1.Height);
            drawer = new BitmapDrawer(data);
            manipulator = new DataManipulator(data, drawer);
            checkBox1.Checked = true;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            drawer.DrawPoints(e.Graphics);
            drawer.DrawLines(e.Graphics);
            drawer.DrawBezier(e.Graphics);
            e.Graphics.DrawRectangle(new Pen(Color.Red), new Rectangle(199, 199, 3, 3));
            e.Graphics.DrawRectangle(new Pen(Color.Red), new Rectangle(data.Width / 2 - 1, data.Height / 2 - 1, 3, 3));
            drawer.DrawImage(e.Graphics, manipulator);
        }

        #region Button events
        private void button1_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = true;
            data.Points.Clear();
            pictureBox1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = true;
            data.Points.Clear();
            pictureBox1.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (data.image == null)
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Open Image";
                    dlg.Filter = "Image files (*.bmp;*.png;*.jpg)|*.bmp;*.png;*.jpg";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        manipulator.LoadImage(dlg.FileName);
                    }
                    pictureBox1.Invalidate();
                }
            else
            {
                data.index += 1;
                Debug.WriteLine(data.index);
                pictureBox1.Invalidate();
            }
        }
        #endregion

        #region CheckBox events
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
                this.pictureBox1.MouseClick += new MouseEventHandler(this.pictureBox1_MouseClickAddPoint);
            }
            else
            {
                this.pictureBox1.MouseClick -= new MouseEventHandler(this.pictureBox1_MouseClickAddPoint);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
                this.pictureBox1.MouseDown += new MouseEventHandler(this.pictureBox1_MouseDownCatchPoint);
            }
            else
            {
                this.pictureBox1.MouseDown -= new MouseEventHandler(this.pictureBox1_MouseDownCatchPoint);
            }
        }
        #endregion

        #region Mouse Events
        private void pictureBox1_MouseClickAddPoint(object sender, MouseEventArgs e)
        {
            if (manipulator.TryNewPoint(new EditablePoint(e.Location)) == true)
            {
                manipulator.CalculateBezierPoints();
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseDownCatchPoint(object sender, MouseEventArgs e)
        {
            if (manipulator.TryCatchPoint(e.Location) == true)
            {
                this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUpDropPoint);
                this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMoveMovePoint);
                this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeaveDropPoint);
            }
        }

        private void pictureBox1_MouseMoveMovePoint(object sender, MouseEventArgs e)
        {
            manipulator.TryMovePoint(e.Location);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUpDropPoint(object sender, MouseEventArgs e)
        {
            this.pictureBox1.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUpDropPoint);
            this.pictureBox1.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMoveMovePoint);
            this.pictureBox1.MouseLeave -= new System.EventHandler(this.pictureBox1_MouseLeaveDropPoint);
            manipulator.CalculateBezierPoints();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseLeaveDropPoint(object sender, EventArgs e)
        {
            this.pictureBox1.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUpDropPoint);
            this.pictureBox1.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMoveMovePoint);
            this.pictureBox1.MouseLeave -= new System.EventHandler(this.pictureBox1_MouseLeaveDropPoint);
            manipulator.CalculateBezierPoints();
            pictureBox1.Invalidate();
        }
        #endregion

        #region RadioButton events
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                drawer.SetImageFast();
                pictureBox1.Invalidate();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                drawer.SetImageNaive();
                pictureBox1.Invalidate();
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                drawer.SetImageFiltering();
                pictureBox1.Invalidate();
            }
        }
        #endregion

    }

}
