using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace BTChatServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool isConnecting = false;

        // Need params for BT.
        Guid serviceGuid = Guid.Parse("00001101-0000-1000-8000-00805f9b34f");
        RfcommDeviceService rfcommService;
        StreamSocket socket;
        DataReader reader;

        /// <summary>
        /// Bluetooth start / stop connecting.
        /// </summary>
        private async void startButton_Click(object sender, EventArgs e)
        {
            if(!isConnecting)
            {
                string selector = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.FromUuid(serviceGuid));
                DeviceInformationCollection collection = await DeviceInformation.FindAllAsync(selector);
            }

        }
    }
}
