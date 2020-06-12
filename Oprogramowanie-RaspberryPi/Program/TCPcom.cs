using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Program
{
    class TCPcom
    {
        private static TcpListener listener;
        AutomaticControl autoCont = new AutomaticControl();
        UARTcom uart = new UARTcom();

        public void Connection()
        {
            listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();

            AutomaticControl autoCont = new AutomaticControl();
            

            Console.WriteLine("Listening...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();

                NetworkStream stream = client.GetStream();

                var th = new Thread(autoCont.Control);
                th.IsBackground = true;

                while (true)
                {
                    if (stream.DataAvailable)
                    {
                        byte[] received = ReadData(stream);

                        string data = Encoding.UTF8.GetString(received);

                        EventHandler(data, th);
                        
                        Console.WriteLine("Received: " + data);

                        
                    }
                    else
                        Thread.Sleep(1);
                }
            }
        }

        private static byte[] ReadData(NetworkStream stream)
        {
            List<byte> received = new List<byte>();

            while (stream.DataAvailable)
            {
                byte[] buffer = new byte[32];

                stream.Read(buffer, 0, buffer.Length);

                received.AddRange(buffer);
            }

            received.RemoveAll(b => b == 0);

            return received.ToArray();
        }

        private void EventHandler(string command, Thread th)
        {
            
            //uart.Sensors();
            /*AutomaticControl autoCont = new AutomaticControl();
            var th = new Thread(autoCont.Control);
            th.IsBackground = true;*/

            switch (command)
            {
                case "stop":
                    uart.Communication("0\r\n");
                    break;
                case "up":
                    uart.Communication("1\r\n");
                    break;
                case "down":
                    uart.Communication("2\r\n");
                    break;
                case "right":
                    uart.Communication("3\r\n");
                    break;
                case "left":
                    uart.Communication("4\r\n");
                    break;
                case "auto":
                    th.Start();
                    break;
                case "manual":
                    th.Interrupt();
                    autoCont.SleepSwitch = true;
                    //uart.SleepSwitchua = true;
                    //uart.Communication("0\r\n");
                    th.Join();
                    Console.WriteLine("Is auto con alive: " + th.IsAlive.ToString());
                    break;
                default:
                    break;
            }
        }

    }
}
