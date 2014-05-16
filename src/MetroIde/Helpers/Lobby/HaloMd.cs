using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;

namespace MetroIde.Helpers.Lobby
{
    public static class HaloMd
    {
        private const string ModListUrl = @"http://halomd.macgamingmods.com/mods/mods.json.gz";
        private const string MasterServerIp = @"halo.macgamingmods.com";
        private const int MasterServerPort = 29920;

        /// <summary>
        /// Formatted as a list of "ipv4:port" strings.
        /// </summary>
        public static string[] ServerList
        {
            get
            {
                using (var outStream = new MemoryStream())
                using (var tcpClient = new TcpClient {ReceiveTimeout = 5000})
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
                    return Encoding.ASCII.GetString(outStream.ToArray()).Split('\n');
                }
            }
        }

        /// <summary>
        /// Downloads and extracts the HaloMD mod list file into memory (as json).
        /// </summary>
        private static Stream ModListJson
        {
            get
            {
                var outStream = new MemoryStream();
                var request = WebRequest.Create(ModListUrl);
                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var gzipStream = new GZipInputStream(responseStream))
                {
                    var tempBytes = new Byte[4096];
                    int readCount;
                    while ((readCount = gzipStream.Read(tempBytes, 0, tempBytes.Length)) != 0)
                    {
                        outStream.Write(tempBytes, 0, readCount);
                    }
                }
                return outStream;
            }
        }
    }
}
