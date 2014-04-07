using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApplication
{
    class Program
    {
        private const string MasterServerIp = @"halo.macgamingmods.com";
        private const int MasterServerPort = 29920;
        static void Main(string[] args)
        {
            var outStream = new MemoryStream();
            using (var tcpClient = new TcpClient { ReceiveTimeout = 5000 })
            {
                tcpClient.Connect(MasterServerIp, MasterServerPort);
                using (var tcpStream = tcpClient.GetStream())
                {
                    var tempBytes = new Byte[4096];
                    int readCount;
                    while ((readCount = tcpStream.Read(tempBytes, 0, tempBytes.Length)) != 0)
                    {
                        outStream.Write(tempBytes, 0, readCount);
                    }
                }
            }

            var myStr = Encoding.ASCII.GetString(outStream.ToArray()).Split('\n');
            foreach (var str in myStr)
                Console.WriteLine(str);
        }
    }
}
