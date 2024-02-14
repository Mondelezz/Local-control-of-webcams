using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using AForge.Video.DirectShow;

namespace Producer
{
    class Program
    {
        const int SW_HIDE = 0;
        const int SW_SHOW = 0;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);



        private static IPEndPoint comsumerEndPoint;
        private static UdpClient udpClient;

        static void Main(string[] args)
        {
            var comsumerIp = ConfigurationManager.AppSettings.Get("comsumerIp");
            var comsumerPort = int.Parse(ConfigurationManager.AppSettings.Get("comsumerPort"));
            comsumerEndPoint = new IPEndPoint(IPAddress.Parse(comsumerIp), comsumerPort);

            udpClient = new UdpClient();
            udpClient.Client.SendBufferSize = 8192; 

            Console.WriteLine($"comsumer: {comsumerEndPoint}");

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();

            Console.WriteLine("\n Press Enter to hide the console...");
            Console.ReadLine();

            ShowWindow(GetConsoleWindow(), SW_HIDE);
        }

        private static void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            var bmp = new Bitmap(eventArgs.Frame, 800, 600);

            try
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    var bytes = ms.ToArray();
                    udpClient.Send(bytes, bytes.Length, comsumerEndPoint);
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine($"SocketException: {se.ErrorCode} - {se.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }
    }
}
