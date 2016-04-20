using KinectV2MouseControl;
using MoMoker.src;
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
        static void Main(string[] args)
        {
            //KinectControl kController = new KinectControl();
            KinectGestures kGestures = new KinectGestures();
            Console.ReadKey();

        }
    }
}
