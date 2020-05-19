using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            if (clientSocket.Connected)
            {
                byte[] outstream = Encoding.ASCII.GetBytes("hej");

                serverStream.Write(outstream, 0, outstream.Length);
                serverStream.Flush();
            }
            else
            {
                await DisplayAlert("Error", "Not connected", "OK");
                await Navigation.PopModalAsync();
            }
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

        private void ControlButtons(bool auto)
        {
            if (!auto)
            {
                btnUp.IsVisible = true;
                btnDown.IsVisible = true;
                btnLeft.IsVisible = true;
                btnRight.IsVisible = true;
            }
            if (auto)
            {
                btnUp.IsVisible = false;
                btnDown.IsVisible = false;
                btnLeft.IsVisible = false;
                btnRight.IsVisible = false;
            }
        }
    }
}