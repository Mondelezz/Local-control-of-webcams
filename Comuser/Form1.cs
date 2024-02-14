using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Comuser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            int port = int.Parse(ConfigurationManager.AppSettings.Get("port"));
            UdpClient client = new UdpClient(port);

            while (true)
            {
                try
                {
                    UdpReceiveResult data = await client.ReceiveAsync();
                    using (var ms = new MemoryStream(data.Buffer))
                    {
                        pictureBox1.Invoke(new Action(() =>
                        {
                            pictureBox1.Image = new Bitmap(ms);
                        }));
                    }
                    Text = $"Bytes received: {data.Buffer.Length * sizeof(byte)}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in Form1_Load: {ex.Message}");
                }
            }
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            MessageBox.Show(string.Join("\n", host.AddressList.Where(i => i.AddressFamily == AddressFamily.InterNetwork)));   
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
