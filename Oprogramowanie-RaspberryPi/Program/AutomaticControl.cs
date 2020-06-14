using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Program
{
    public class AutomaticControl : UARTcom
    {
        bool sleepSwitch = false;

        public bool SleepSwitch
        {
            set { sleepSwitch = value; }
        }

        public void Control()
        {
           /* Kinectcom kinect = new Kinectcom();
            kinect.init();*/


            Chauffeur(/*th*/);
        }

        private void Chauffeur(/*Thread th*/)
        {
            //string straight = "1\r\n", left = "4\r\n", right = "3\r\n", stop = "0\r\n";
            //bool[] direction = { false, false, false, false };
            while (!sleepSwitch)
            {
                Sensors();
                
                Console.WriteLine("Left Ground:  " + lGround.ToString()  + "\n" +
                                  "Right Ground: " + rGround.ToString()  + "\n" +
                                  "Front:        " + front.ToString()    + "\n" +
                                  "Battery:      " + battery.ToString()  + "\n" +
                                  "Is docked:    " + isDocked.ToString() + "\n" );

                /*if (front < 1000 && direction[0] == false)
                {
                    while( Communication(straight) == false) { Thread.Sleep(20); }
                    direction[0] = true;
                }
                    
                if (front > 1000)
                {
                    if(direction[0] == true)
                    {
                        while (Communication(stop) == false) { Thread.Sleep(20); }
                        direction[0] = false;
                        Thread.Sleep(4000);
                    }
                    while (Communication(left) == false) { Thread.Sleep(20); }
                    Thread.Sleep(100);
                }*/
            }
            try
            {
                Console.WriteLine("newThread going to sleep.");

                Thread.Sleep(Timeout.Infinite);
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("newThread cannot go to sleep - " +
                    "interrupted by main thread.");
            }
        }
    }
}
