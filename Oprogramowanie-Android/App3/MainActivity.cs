using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace App3
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        TcpClient clientSocket;
        NetworkStream serverStream;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            /*Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);*/


            Button buttonC = FindViewById<Button>(Resource.Id.btnConnect);
            buttonC.Click += ConnectOnClick;

            Button buttonD = FindViewById<Button>(Resource.Id.btnDisconnect);
            buttonD.Click += DisconnectOnClick;

            Button buttonAF = FindViewById<Button>(Resource.Id.btnAutofill);
            buttonAF.Click += AutofillOnClick;

            Button buttonS = FindViewById<Button>(Resource.Id.btnSend);
            buttonS.Click += SendOnClick;

        }

        private void SendOnClick(object sender, EventArgs e)
        {
            if (clientSocket.Connected)
            {
                byte[] outstream = Encoding.ASCII.GetBytes("hej");

                serverStream.Write(outstream, 0, outstream.Length);
                serverStream.Flush();
            }
        }

        private void AutofillOnClick(object sender, EventArgs e)
        {
            FindViewById<EditText>(Resource.Id.txtIP).Text = "192.168.8.122";
            FindViewById<EditText>(Resource.Id.txtPort).Text = "8080";
        }

        private void ConnectOnClick(object sender, EventArgs eventArgs)
        {
            EditText ip = FindViewById<EditText>(Resource.Id.txtIP);
            EditText port = FindViewById<EditText>(Resource.Id.txtPort);

            clientSocket = new TcpClient();
            serverStream = default(NetworkStream);
            try
            {
                clientSocket.Connect(ip.Text, Int32.Parse(port.Text));
                Thread ctThread = new Thread(Connect);
                ctThread.Start();
                if(clientSocket.Connected) ShowMessage("Connected :D", sender);
            }
            catch (SocketException e)
            {
                ShowMessage(e.Message, sender);
            }
            catch (FormatException e)
            {
                ShowMessage(e.Message, sender);
            }
        }

        private void Connect()
        {
            serverStream = clientSocket.GetStream();
        }


        private void DisconnectOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                clientSocket.Close();
                serverStream.Close();
                ShowMessage("Disonnected :c", sender);
            }
            catch { }
        }

        private void ShowMessage(string e, object sender)
        {
            View view = (View)sender;
            Snackbar.Make(view, e, Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        /*public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }*/
    }
}

