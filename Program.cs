using ImageProcessing;
using System;
using System.Windows.Forms;

namespace ImageProcessing
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // التأكد أن اسم الفورم هنا يطابق اسم الكلاس الخاص بك
            Application.Run(new Form1());
        }
    }
}