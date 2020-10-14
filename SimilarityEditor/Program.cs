using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimilarityEditor
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

        public static Color GetColorFotSimilarity(float k)
        {
            k = 1 - k;
            return Color.FromArgb((int)(255 * k), 200 + (int)(55 * k), (int)(255 * k));
        }
    }
}
