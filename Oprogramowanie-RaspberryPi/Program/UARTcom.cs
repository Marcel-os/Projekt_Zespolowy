using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace Program
{
    public class UARTcom
    {
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
                    // we didn't get the response yet, let's wait for it then
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
    }
}
