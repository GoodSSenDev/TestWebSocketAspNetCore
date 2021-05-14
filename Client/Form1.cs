using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client;
using NetworkModule;
using NetworkModule.Json;
using NetworkModule.Messages;

namespace Client
{
    public partial class Form1 : Form
    {
        private ClientBll _clientTool = new ClientBll();

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event method that start the connection when UI click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void connectBtn_Click(object sender, EventArgs e)
        {
            await _clientTool.StartConnection();
        }

        private async void HeartMessageSendBtn_Click(object sender, EventArgs e)
        {
            await _clientTool.SendHeartBeatRequest();
        }

        private async void HeartBeatSendGetResBtn_Click(object sender, EventArgs e)
        {
            await _clientTool.SendHeartBeartRequestAndReceiveAsync();
        }
    }
}
