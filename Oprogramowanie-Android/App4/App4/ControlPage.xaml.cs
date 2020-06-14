using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App4
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ControlPage : ContentPage
    {
        NetworkStream serverStream;
        TcpClient clientSocket;

        public ControlPage(NetworkStream sStream, TcpClient cSocket)
        {
            InitializeComponent();

            serverStream = sStream;
            clientSocket = cSocket;
        }

        private async void BtnDisconnect_Clicked(object sender, EventArgs e)
        {
            try
            {
                clientSocket.Close();
                serverStream.Close();
                await DisplayAlert("Disconnected :c", "", "OK");
                await Navigation.PopModalAsync();
            }
            catch { }
        }

        private async void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (clientSocket.Connected)
            {
                byte[] outstream = null;
                if (switchTg.IsToggled)
                {
                    outstream = Encoding.ASCII.GetBytes("auto");
                    ControlButtons(true);
                    //ReceiveData();
                }
                if (!switchTg.IsToggled)
                {
                    outstream = Encoding.ASCII.GetBytes("manual");
                    ControlButtons(false);
                }
                serverStream.Write(outstream, 0, outstream.Length);
                serverStream.Flush();
            }
            else
            {
                await DisplayAlert("Error", "Not connected", "OK");
                await Navigation.PopModalAsync();
            }
        }

/*        private void ReceiveData()
        {
            while (switchTg.IsToggled)
            {
                if (serverStream.DataAvailable)
                {
                    byte[] received = ReadData(serverStream);

                    string data = Encoding.UTF8.GetString(received);
                    labelData.Text = data;
                    //EventHandler(data, th);

                    //Console.WriteLine("Received: " + data);
                    Thread.Sleep(200);

                }
                else
                    Thread.Sleep(20);
            }
        }

        private static byte[] ReadData(NetworkStream stream)
        {
            List<byte> received = new List<byte>();

            while (stream.DataAvailable)
            {
                byte[] buffer = new byte[248];

                stream.Read(buffer, 0, buffer.Length);

                received.AddRange(buffer);
            }

            received.RemoveAll(b => b == 0);

            return received.ToArray();
        }*/

        private void ControlButtons(bool auto)
        {
            if (!auto)
            {
                labelData.IsVisible = false;
                btnStop.IsVisible = true;
                btnUp.IsVisible = true;
                btnDown.IsVisible = true;
                btnLeft.IsVisible = true;
                btnRight.IsVisible = true;
            }
            if (auto)
            {
                labelData.IsVisible = true;
                btnStop.IsVisible = false;
                btnUp.IsVisible = false;
                btnDown.IsVisible = false;
                btnLeft.IsVisible = false;
                btnRight.IsVisible = false;
            }
        }

        private void BtnUp_Clicked(object sender, EventArgs e)
        {
            SendCommand("up");
        }

        private void BtnDown_Clicked(object sender, EventArgs e)
        {
            SendCommand("down");
        }

        private void BtnLeft_Clicked(object sender, EventArgs e)
        {
            SendCommand("left");
        }

        private void BtnRight_Clicked(object sender, EventArgs e)
        {
            SendCommand("right");
        }

        private void BtnStop_Clicked(object sender, EventArgs e)
        {
            SendCommand("stop");
        }

        private async void SendCommand(string command)
        {
            if (clientSocket.Connected)
            {
                byte[] outstream = Encoding.ASCII.GetBytes(command);

                serverStream.Write(outstream, 0, outstream.Length);
                serverStream.Flush();
            }
            else
            {
                await DisplayAlert("Error", "Not connected", "OK");
                await Navigation.PopModalAsync();
            }
        }
    }
}