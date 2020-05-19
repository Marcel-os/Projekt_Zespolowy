using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App4
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        TcpClient clientSocket;
        NetworkStream serverStream;

        public MainPage()
        {
            InitializeComponent();
        }

        private void BtnAutofill_Clicked(object sender, EventArgs e)
        {
            txtIp.Text = "192.168.8.122";
            txtPort.Text = "8080";
        }

        private async void BtnConnect_Clicked(object sender, EventArgs e)
        {
            string ip = txtIp.Text;
            string port = txtPort.Text;

            clientSocket = new TcpClient();
            serverStream = default(NetworkStream);

            try
            {
                clientSocket.Connect(ip, Int32.Parse(port));
                Thread ctThread = new Thread(Connect);
                ctThread.Start();
                if (clientSocket.Connected)
                {
                    await DisplayAlert("Connected :D", "", "OK");
                    await Navigation.PushModalAsync(new NavigationPage(new ControlPage(serverStream, clientSocket)));
                }
            }
            catch (SocketException en)
            {
                await DisplayAlert("Error", en.Message, "OK");
            }
            catch (FormatException en)
            {
                await DisplayAlert("Error", en.Message, "OK");
            }
            catch (ArgumentNullException en)
            {
                await DisplayAlert("Error", en.Message, "OK");
            }
        }

        private void Connect()
        {
            serverStream = clientSocket.GetStream();
        }
    }
}
