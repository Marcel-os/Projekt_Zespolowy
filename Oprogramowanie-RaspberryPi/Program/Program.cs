using System;
using System.IO.Ports;



namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            /*foreach(var item in SerialPort.GetPortNames())
            {
                Console.WriteLine(item);
            }*/
            
            TCPcom TCP = new TCPcom();
            TCP.Connection();
        }
    }
}
