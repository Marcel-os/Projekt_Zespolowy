using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace Program
{
    public class UARTcom
    {
        public int lGround = 0;
        public int rGround = 0;
        public int front = 0;
        public float battery = 0;
        public bool isDocked = false;

        /*bool sleepSwitchua = false;

        public bool SleepSwitchua
        {
            set { sleepSwitchua = value; }
        }*/

        public void Communication(string command)
        {
            string portName = "/dev/ttyS0";
            int baudRate = 9600;

            using (SerialPort uart = new SerialPort(portName))
            {
                uart.Encoding = Encoding.UTF8;
                uart.BaudRate = baudRate;
                uart.ReadTimeout = 1000;
                uart.WriteTimeout = 1000;
                uart.Open();

                try
                {
                    uart.WriteLine(command);
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("ERROR: Sending command timed out");
                }

                string existingData = uart.ReadExisting();
                Console.Write(existingData);
                if (!existingData.Contains('\n') && !existingData.Contains('\r'))
                {
                    try
                    {
                        Console.WriteLine(uart.ReadLine());
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine($"ERROR: No response in {uart.ReadTimeout}ms.");
                    }
                }
            }
        }

        public void Sensors()
        {
            string portName = "/dev/ttyS0";
            int baudRate = 9600;

            using (SerialPort uart = new SerialPort(portName))
            {
                uart.Encoding = Encoding.UTF8;
                uart.BaudRate = baudRate;
                uart.ReadTimeout = 4000;
                uart.WriteTimeout = 1000;
                uart.Open();

                while (true)
                {
                    try
                    {
                        string sensors = uart.ReadLine();
                        //Console.WriteLine(sensors);
                        string[] sensorsArray = sensors.Split(' ');
                        lGround = int.Parse(sensorsArray[1]);
                        rGround = int.Parse(sensorsArray[2]);
                        front = int.Parse(sensorsArray[3]);
                        battery = float.Parse(sensorsArray[4]);
                        isDocked = Convert.ToBoolean(int.Parse(sensorsArray[5]));
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine($"ERROR: No response in {uart.ReadTimeout}ms.");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Index out of bounds");
                    }
                }
            }
        }
    }
}
