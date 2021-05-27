using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IG.Engine.EmailService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] arg)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());

//#else
//    if (arg == null || arg.Length == 0)
//    {
//        MessageBox.Show("Please run application from VRIS program", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//        Application.Exit();

//    }
//    else
//    {
//        Application.EnableVisualStyles();
//        Application.SetCompatibleTextRenderingDefault(false);
//        Application.Run(new FrmMain());
//    }
//#endif

        }
    }
}