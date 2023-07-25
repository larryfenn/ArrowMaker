using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ArrowMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, Device> devices = new Dictionary<int, Device>();
            int port = 5005;
            int n_devices = 1;
            // first arg is # of devices
            // second arg if it exists is port
            if (args.Length > 0)
            {
                n_devices = Int32.Parse(args[0]);
            }

            if (args.Length > 1)
            {
                port = Int32.Parse(args[1]);
            }

            for (int i = 0; i < n_devices; i++)
            {
                Random rand = new Random();
                devices.Add(i, new Device(rand.Next(), (byte)i));
            }

            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            while(true)
            {
                foreach(KeyValuePair<int, Device> entry in devices)
                {
                    entry.Value.Update();
                    entry.Value.Transmit(endpoint, socket);
                }
            }
        }
    }
}
