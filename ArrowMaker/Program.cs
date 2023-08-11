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
            int n_devices = 1;
            int delay = 50;
            int port = 5005;
            // first arg is # of devices
            // second arg is time between device transmissions
            // third arg if it exists is port
            if (args.Length > 0)
            {
                n_devices = Int32.Parse(args[0]);
            }

            if (args.Length > 1)
            {
                delay = Int32.Parse(args[1]);
            }

            if (args.Length > 2)
            {
                port = Int32.Parse(args[2]);
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
                Random rand = new Random();
                foreach(KeyValuePair<int, Device> entry in devices)
                {
                    entry.Value.Update();
                    entry.Value.Transmit(endpoint, socket);
                }
                System.Threading.Thread.Sleep(delay);
            }
        }
    }
}
