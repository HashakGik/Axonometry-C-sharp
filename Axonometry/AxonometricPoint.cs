using System;

namespace Axonometry
{
    /// <summary>
    /// Axonometric Point class. It stores a 3D point and parameters for the axonometric projection, and allows to transform the point with a transformation matrix.
    /// </summary>
    class AxonometricPoint
    {
        public double X
        {
            get
            {
                return this.coords[0];
            }
            set
            {
                this.coords[0] = value;
            }
        }
        public double Y
        {
            get
            {
                return this.coords[1];
            }
            set
            {
                this.coords[1] = value;
            }
        }
        public double Z
        {
            get
            {
                return this.coords[2];
            }
            set
            {
                this.coords[2] = value;
            }
        }

        private double[] coords;

        /// <summary>
        /// Property which returns the horizontal value of the mapped 2D coordinates.
        /// </summary>
        public double Hor
        {
            get
            {
                return this.xScaleHor * this.coords[0] + this.yScaleHor * this.coords[1] + this.zScaleHor * this.coords[2];
            }
        }

        /// <summary>
        /// Property which returns the vertical value of the mapped 2D coordinates.
        /// </summary>
        public double Ver
        {
            get
            {
                return this.xScaleVer * this.coords[0] + this.yScaleVer * this.coords[1] + this.zScaleVer * this.coords[2];
            }
        }

        public readonly double xScaleHor;
        public readonly double xScaleVer;
        public readonly double yScaleHor;
        public readonly double yScaleVer;
        public readonly double zScaleHor;
        public readonly double zScaleVer;

        /// <summary>
        /// Constructor. Stores the coordinates in an array, to simplify geometric transformations.
        /// Common axonometric projections are:
        /// - Isometric: xAngle: 210°, yAngle: 330°, zAngle: 90°, xScale: 1, yScale: 1, zScale: 1.
        /// - Engineer: xAngle: 222°, yAngle: 353°, zAngle: 90°, xScale: 0.5, yScale: 1, zScale: 1.
        /// - Cavalier: xAngle: any (usually 225°), yAngle: 0°, zAngle: 90°, xScale: any (usually 1 or 0.5), yScale: 1, zScale: 1.
        /// - Bird's eye: xAngle: any, yAngle: xAngle + 90°, zAngle: 90°, xScale: any, yScale: any, zScale: any (usually 2.0/3.0).
        /// - Military: xAngle: any, yAngle: xAngle + 90°, zAngle: 90°, xScale: any, yScale: any, zScale: 1.
        /// </summary>
        /// <param name="x">X coordinate of the point.</param>
        /// <param name="y">Y coordinate of the point.</param>
        /// <param name="z">Z coordinate of the point.</param>
        /// <param name="xAngle">Angle (in radians) at which the X axis will be projected. Default value is for isometric axonometry.</param>
        /// <param name="yAngle">Angle (in radians) at which the Y axis will be projected. Default value is for isometric axonometry.</param>
        /// <param name="zAngle">Angle (in radians) at which the Z axis will be projected. Default value is for isometric axonometry.</param>
        /// <param name="xScale">Scaling factor of the x axis. Default value is for isometric axonometry.</param>
        /// <param name="yScale">Scaling factor of the y axis. Default value is for isometric axonometry.</param>
        /// <param name="zScale">Scaling factor of the z axis. Default value is for isometric axonometry.</param>
        public AxonometricPoint(double x, double y, double z, double xAngle = 7.0 / 6.0 * Math.PI, double yAngle = 11.0 / 6.0 * Math.PI, double zAngle = Math.PI / 2, double xScale = 1, double yScale = 1, double zScale = 1)
        {
            this.coords = new double[4];

            this.coords[0] = x;
            this.coords[1] = y;
            this.coords[2] = z;
            this.coords[3] = 1;

            this.xScaleHor = Math.Cos(xAngle) * xScale;
            this.xScaleVer = Math.Sin(xAngle) * xScale;
            this.yScaleHor = Math.Cos(yAngle) * yScale;
            this.yScaleVer = Math.Sin(yAngle) * yScale;
            this.zScaleHor = Math.Cos(zAngle) * zScale;
            this.zScaleVer = Math.Sin(zAngle) * zScale;
        }

        /// <summary>
        /// Applies a geometric transformation to the point.
        /// </summary>
        /// <param name="matrix">Transformation matrix to be applied.</param>
        public void Transform(double[,] matrix)
        {
            double[] tmp = new double[4];
            for (int i = 0; i < 4; i++)
            {
                tmp[i] = 0;
                for (int j = 0; j < 4; j++)
                    tmp[i] += this.coords[j] * matrix[i, j];
            }

            this.coords = tmp;
        }

        /// <summary>
        /// Rotates the point along the Z axis. It simply creates a rotation matrix and calls Transform().
        /// </summary>
        /// <param name="phi">Angle (in radians) by which the point will be rotated counter-clockwise</param>
        public void RotateZ(double phi)
        {
            this.Transform(new double[,] { { Math.Cos(phi), -Math.Sin(phi), 0, 0 }, { Math.Sin(phi), Math.Cos(phi), 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } });
        }
        /// <summary>
        /// Rotates the point along the Y axis. It simply creates a rotation matrix and calls Transform().
        /// </summary>
        /// <param name="phi">Angle (in radians) by which the point will be rotated counter-clockwise</param>
        public void RotateY(double phi)
        {
            this.Transform(new double[,] { { Math.Cos(phi), 0, Math.Sin(phi), 0 }, { 0, 1, 0, 0 }, { -Math.Sin(phi), 0, Math.Cos(phi), 0 }, { 0, 0, 0, 1 } });
        }
        /// <summary>
        /// Rotates the point along the X axis. It simply creates a rotation matrix and calls Transform().
        /// </summary>
        /// <param name="phi">Angle (in radians) by which the point will be rotated counter-clockwise</param>
        public void RotateX(double phi)
        {
            this.Transform(new double[,] { { 1, 0, 0, 0 }, { 0, Math.Cos(phi), -Math.Sin(phi), 0 }, { 0, Math.Sin(phi), Math.Cos(phi), 0 }, { 0, 0, 0, 1 } });
        }

        /// <summary>
        /// Scales the point. It simply creates a scaling matrix and calls Transform().
        /// </summary>
        /// <param name="x">Scaling along x axis.</param>
        /// <param name="y">Scaling along y axis.</param>
        /// <param name="z">Scaling along z axis.</param>
        public void Scale(double x, double y, double z)
        {
            this.Transform(new double[,] { { x, 0, 0, 0 }, { 0, y, 0, 0 }, { 0, 0, z, 0 }, { 0, 0, 0, 1 } });
        }
        /// <summary>
        /// Shears the point. It simply creates a shearing matrix and calls Transform().
        /// </summary>
        /// <param name="xy">Shearing xy factor.</param>
        /// <param name="xz">Shearing xz factor.</param>
        /// <param name="yx">Shearing yx factor.</param>
        /// <param name="yz">Shearing yz factor.</param>
        /// <param name="zx">Shearing zx factor.</param>
        /// <param name="zy">zy</param>
        public void Shear(double xy, double xz, double yx, double yz, double zx, double zy)
        {
            this.Transform(new double[,] { { 1, xy, xz, 0 }, { yx, 1, yz, 0 }, { zx, zy, 1, 0 }, { 0, 0, 0, 1 } });
        }
    }
}
