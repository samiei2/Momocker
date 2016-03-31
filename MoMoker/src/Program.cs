using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoMoker
{
    class Program
    {
        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);
        static void Main(string[] args)
        {
            SetCursorPos(199, 199);
            for (int i = 0; i < 500; i++)
            {
                SetCursorPos(i, i);
            }
        }
    }
}
