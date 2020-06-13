using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Kinect;

namespace Program
{
    class Kinectcom
    {
        private KinectSensor kSensor;

        public void init()
        {
            if (kSensor.IsAvailable)
                Console.WriteLine("no wykrywa");
            else
                Console.WriteLine("no nie wykrywa");
        }
        
    }
}
