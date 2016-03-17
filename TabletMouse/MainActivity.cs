/*  SmartPhone and Tablet will change mouse and keyboard on your PC!
 *  Copyright (C) 2016  Nectcom

*   This program is free software; you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation; either version 2 of the License, or
*   (at your option) any later version.

*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.

*   You should have received a copy of the GNU General Public License along
*   with this program; if not, write to the Free Software Foundation, Inc.,
*   51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Collections;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Bluetooth;

namespace TabletMouse
{
    [Activity(Label = "TabletMouse", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private string TAG = "SerialBTMain";
        protected BluetoothAdapter adapter;
        protected BluetoothDevice device;
        protected BluetoothSocket socket;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Set BT Adapter.
            adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
                throw new Exception("No Bluetooth adapter");
            if (!adapter.IsEnabled)
                throw new Exception("Bluetooth is not enabled.");

            // set collection to spinner.
            Spinner btList = FindViewById<Spinner>(Resource.Id.bt_list);
            ArrayList children = new ArrayList();
            foreach(var dev in adapter.BondedDevices)
                children.Add(dev.Name);

            ArrayAdapter btAdapterList = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, children);
            btList.Adapter = btAdapterList;

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.send_button);

            button.Click += sendMsg;
            button.Click += (o, e) => Toast.MakeText(this, "send.", ToastLength.Short).Show();

            ToggleButton toggle = FindViewById<ToggleButton>(Resource.Id.state_toggle);
            toggle.Click += (o, e) =>
            {
                if (toggle.Checked)
                {
                    Log.Debug(TAG, "start connect.");
                    StartBT();
                }
                else
                {
                    Log.Debug(TAG, "start disconnect.");
                    StopBT();
                }
            };
        }

        // Start BT connecting.
        protected async void StartBT()
        {
            Log.Debug(TAG, "start to connect BT.");

            Spinner spinner = FindViewById<Spinner>(Resource.Id.bt_list);
            string devName = (string)spinner.SelectedItem;

            // search BT device which has devName.
            foreach(var dev in adapter.BondedDevices)
                if (dev.Name == devName)
                    device = dev;

            if (device == null)
                throw new Exception("Named device is not found.");

            //get socket to SPP.
            socket = device.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            await socket.ConnectAsync();

            Log.Debug(TAG, "connect to BT device.");
            Toast.MakeText(this, "connected.", ToastLength.Short).Show();
        }

        // Stop BT connection.
        protected void StopBT()
        {
            if(socket.IsConnected)
                socket.Close();
            Log.Debug(TAG, "close SSP socket.");
            Toast.MakeText(this, "disconnected.", ToastLength.Short).Show();
        }

        // send message and clear text.
        protected async void sendMsg(object sender, EventArgs e)
        {
            var editText = FindViewById<EditText>(Resource.Id.message_line);
            var text = editText.Text.ToCharArray();
            byte[] message = System.Text.Encoding.UTF8.GetBytes(text);

            if (socket.IsConnected)
                await socket.OutputStream.WriteAsync(message, 0, message.Length);

            editText.Text = "";
        }
    }
}

