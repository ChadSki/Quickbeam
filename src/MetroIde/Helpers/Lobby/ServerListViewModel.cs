using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetroIde.Helpers.Lobby
{
    class ServerListViewModel
    {
        public ObservableCollection<HaloServer> Servers;

        public ServerListViewModel()
        {
            var foo = new ObservableCollection<HaloServer>();
            foreach (var address in HaloMd.ServerList)
            {
                foo.Add(new HaloServer(address));
            }
            Servers = foo;

            Task.Factory.StartNew(() =>
            {
                foreach (var hs in Servers)
                {
                    var udpClient = new UdpClient(hs.IpAddress, hs.Port) {Client = {ReceiveTimeout = 5000}};
                    
                    var message = new byte[] {
                        254, 253, 0, 144,
                        141, 143, 1, 255,
                        255, 255
                    };
                    udpClient.Send(message, message.Length);

                    var endPoint = new IPEndPoint(IPAddress.Any, 0);
                    var response = udpClient.Receive(ref endPoint);

                    string hostname = "";
                    int index = 0;
                    string text = "";
                    while (text != "hostname")
                    {
                        text = Encoding.ASCII.GetString(response, index, 8);
                        index++;
                    }
                    index += text.Length;
                    while (response[index] != 0)
                    {
                        hostname += Encoding.ASCII.GetString(response, index, 1);
                        index++;
                    }

                    hs.Name = hostname;
                }
            });
        }
    }
}
