using System;
using System.Windows.Forms;

/// <summary>
/// This program implements an axonometric mapping of 3D objects on a 2D screen. For demonstration purposes the objects to be displayed are simple wireframes with no surfaces or hidden-line removal algorithms.
/// </summary>

namespace Axonometry
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
