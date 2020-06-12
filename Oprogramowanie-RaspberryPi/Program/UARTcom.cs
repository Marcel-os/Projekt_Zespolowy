using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace Program
{
    public class UARTcom
    {
        public int lGround = 0;
        public int rGround = 0;
        public int front = 0;
        public float battery = 0;
        public bool isDocked = false;

        static string portName = "/dev/ttyS0";
        int baudRate = 115200;
        SerialPort uart = new SerialPort(portName);

        public bool Communication(string command)
        {

            uart.Encoding = Encoding.UTF8;
            uart.BaudRate = baudRate;
            uart.ReadTimeout = 1;
            uart.WriteTimeout = 1000;
            if (uart.IsOpen == false)
                uart.Open();

            try
            {
                uart.WriteLine(command);
                return true;
            }
            catch (TimeoutException)
            {
                Console.WriteLine("ERROR: Sending command timed out");
                return false;
            }

            /*string existingData = uart.ReadExisting();
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
                    //Communication(command);
                }
            }*/

        }

        public void Sensors()
        {
            uart.Encoding = Encoding.UTF8;
            uart.BaudRate = baudRate;
            uart.ReadTimeout = 2000;
            uart.WriteTimeout = 1;
            if (uart.IsOpen == false)
                uart.Open();
           
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
            catch (FormatException)
            {
                Console.WriteLine("err");
            }
        }
    }
}
