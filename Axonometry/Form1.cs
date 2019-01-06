using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Axonometry
{
    /// <summary>
    /// This form creates the wireframe objects using two data structures and shows them on screen.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// 3D axes which will be plotted on screen.
        /// </summary>
        private AxonometricPoint[] axes;
        /// <summary>
        /// Template figure's vertices. This won't be modified.
        /// </summary>
        private AxonometricPoint[] figure;
        /// <summary>
        /// Copy of the template figure. This figure will be rotated at each timer's tick.
        /// </summary>
        private AxonometricPoint[] rotatedFigure;
        /// <summary>
        /// List of connected vertices. It stores the indexes of two connected vertices, so it isn't affected by any transformation of the figure.
        /// </summary>
        private List<Tuple<int, int>> edges;
        /// <summary>
        /// Temporary points used to plot each line (avoiding the allocation of new points at each redraw, in order to reduce the garbage collection).
        /// </summary>
        private Point a, b;
        public Form1()
        {
           InitializeComponent();
            
            this.axes = new AxonometricPoint[3];
            this.axes[0] = new AxonometricPoint(20, 0, 0); //, xAngle: Math.PI / 180 * 225, yAngle: 0, zAngle: Math.PI / 2, xScale: 0.5);
            this.axes[1] = new AxonometricPoint(0, 20, 0); //, xAngle: Math.PI / 180 * 225, yAngle: 0, zAngle: Math.PI / 2, xScale: 0.5);
            this.axes[2] = new AxonometricPoint(0, 0, 20); //, xAngle: Math.PI / 180 * 225, yAngle: 0, zAngle: Math.PI / 2, xScale: 0.5);


            this.edges = new List<Tuple<int, int>>();
            //Cube:
 /*                     this.figure = new AxonometricPoint[8];
                        this.figure[0] = new AxonometricPoint(-10, -10, -10);
                        this.figure[1] = new AxonometricPoint(-10, 10, -10);
                        this.figure[2] = new AxonometricPoint(10, 10, -10);
                        this.figure[3] = new AxonometricPoint(10, -10, -10);
                        this.figure[4] = new AxonometricPoint(-10, -10, 10);
                        this.figure[5] = new AxonometricPoint(-10, 10, 10);
                        this.figure[6] = new AxonometricPoint(10, 10, 10);
                        this.figure[7] = new AxonometricPoint(10, -10, 10);

                        this.edges.Add(new Tuple<int, int>(0, 1));
                        this.edges.Add(new Tuple<int, int>(1, 2));
                        this.edges.Add(new Tuple<int, int>(2, 3));
                        this.edges.Add(new Tuple<int, int>(3, 0));
                        this.edges.Add(new Tuple<int, int>(4, 5));
                        this.edges.Add(new Tuple<int, int>(5, 6));
                        this.edges.Add(new Tuple<int, int>(6, 7));
                        this.edges.Add(new Tuple<int, int>(7, 4));
                        this.edges.Add(new Tuple<int, int>(0, 4));
                        this.edges.Add(new Tuple<int, int>(1, 5));
                        this.edges.Add(new Tuple<int, int>(2, 6));
                        this.edges.Add(new Tuple<int, int>(3, 7));
  */          
            // Diamond: A pyramid with a regular polygon as a base and a second polygon rotated by PI / sides.
            int sides = 12; // Sides of the diamond.
            this.figure = new AxonometricPoint[sides * 2 + 1];
            this.figure[sides * 2] = new AxonometricPoint(0, 0, 0);
            for (int i = 0; i < sides; i++)
            {
                this.figure[i] = new AxonometricPoint((int)(10 * Math.Cos(Math.PI / sides * 2 * i + Math.PI / sides)), (int)(10 * Math.Sin(Math.PI / sides * 2 * i + Math.PI / sides)), 15);
                this.figure[i + sides] = new AxonometricPoint((int)(13 * Math.Cos(Math.PI / sides * 2 * i)), (int)(13 * Math.Sin(Math.PI / sides * 2 * i)), 13);

                this.edges.Add(new Tuple<int, int>(i, (i + 1) % sides));
                this.edges.Add(new Tuple<int, int>(i + sides, (i + 1) % sides + sides));
                this.edges.Add(new Tuple<int, int>(i + sides, 2 * sides));
                this.edges.Add(new Tuple<int, int>(i, i + sides));
                this.edges.Add(new Tuple<int, int>(i, (i + 1) % sides + sides));
            }

            rotatedFigure = new AxonometricPoint[this.figure.Length];
            for (int i = 0; i < rotatedFigure.Length; i++)
            {
                rotatedFigure[i] = new AxonometricPoint(figure[i].X, figure[i].Y, figure[i].Z); //, xAngle: Math.PI / 180 * 225, yAngle: 0, zAngle: Math.PI / 2, xScale: 0.5);
            }


            a = new Point(0, 0);
            b = new Point(0, 0);
        }



        /// <summary>
        /// Paint event handler. It redraws rotatedFigure and the three axes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < this.edges.Count; i++)
            {
                a.X = (int)(rotatedFigure[this.edges[i].Item1].Hor * 10.0 + this.Width / 2);
                a.Y = (int)(this.Height / 2 - rotatedFigure[this.edges[i].Item1].Ver * 10.0);
                b.X = (int)(rotatedFigure[this.edges[i].Item2].Hor * 10.0 + this.Width / 2);
                b.Y = (int)(this.Height / 2 - rotatedFigure[this.edges[i].Item2].Ver * 10.0);

                e.Graphics.DrawLine(Pens.Black, a, b);
            }

            for (int i = 0; i < axes.Length; i++)
                e.Graphics.DrawLine(Pens.Red, new Point(this.Width / 2, this.Height / 2), new Point((int)(10.0 * this.axes[i].Hor + this.Width / 2), (int)(this.Height / 2 - 10.0 * this.axes[i].Ver)));
               
        }

        /// <summary>
        /// Timer's tick event. It applies a transformation for each vertex of rotatedFigure and then invalidates the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Rototraslate the figure at each tick.
            for (int i = 0; i < rotatedFigure.Length; i++)
            {
                rotatedFigure[i].X -= 0.5;
                rotatedFigure[i].Y -= 0.5;
                rotatedFigure[i].RotateZ(0.1);
            }

            this.Invalidate();
        }
    }
}
