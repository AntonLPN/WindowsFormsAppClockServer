using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppClockServer
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
        }

        private async void Timer1_Tick(object sender, EventArgs e)
        {
            IPAddress address = IPAddress.Loopback;
            IPEndPoint endPoint = new IPEndPoint(address, 11000);
            Socket sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
            byte[] buff = Encoding.Unicode.GetBytes(DateTime.Now.ToString());
            await sendSocket.SendToAsync(new ArraySegment<byte>(buff), SocketFlags.None, endPoint);
            sendSocket.Shutdown(SocketShutdown.Send);
            sendSocket.Close();

        }
     

        private void buttonStartServer_Click(object sender, EventArgs e)
        {
             timer1.Start();

            Task.Run(async () => {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
                IPAddress address = IPAddress.Loopback;
                IPEndPoint endPoint = new IPEndPoint(address, 11000);
                byte[] buff = new byte[1024];
                EndPoint ep = new IPEndPoint(IPAddress.Any, 11000);
                socket.Bind(endPoint);
                do
                {

                    await socket.ReceiveFromAsync(new ArraySegment<byte>(buff), SocketFlags.None, ep).ContinueWith(t => {
                        SocketReceiveFromResult res = t.Result;
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine($"{res.ReceivedBytes} байт получено из {res.RemoteEndPoint}");
                        builder.AppendLine(Encoding.Unicode.GetString(buff, 0, res.ReceivedBytes));
                        textBox1.BeginInvoke(new Action<string>(AddText), builder.ToString());
                    });
                }
                while (true);
            });




        }

        private void AddText(string str)
        {
            StringBuilder builder = new StringBuilder(textBox1.Text);
            builder.AppendLine(str);
            textBox1.Text = builder.ToString();
        }

       

      
    }
}
