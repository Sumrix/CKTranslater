using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimilarityEditor
{
    internal static class Program
    {
        public static Color GetColorFotSimilarity(float k)
        {
            k = 1 - k;
            return Color.FromArgb((int) (255 * k), 200 + (int) (55 * k), (int) (255 * k));
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}