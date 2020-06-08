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

            var th = new Thread(Sensors);
            th.IsBackground = true;
            th.Start();
            Thread.Sleep(2000);

            ReadSensors();
        }

        private void ReadSensors()
        {
            while (!sleepSwitch)
            {
                Console.WriteLine("Left Ground:  " + lGround.ToString()  + "\n" +
                                  "Right Ground: " + rGround.ToString()  + "\n" +
                                  "Front:        " + front.ToString()    + "\n" +
                                  "Battery:      " + battery.ToString()  + "\n" +
                                  "Is docked:    " + isDocked.ToString() + "\n" );
                Thread.Sleep(1000);
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
