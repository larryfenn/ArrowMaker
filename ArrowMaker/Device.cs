using System;
using System.Numerics;
using System.Net;
using System.Net.Sockets;

namespace ArrowMaker
{
    class Device
    {
        private byte deviceId;
        private int startMs;
        private float speed;
        private Vector3 rotAxis;
        private Quaternion orientation;

        public Device(int seed, byte id)
        {
            deviceId = id;
            Random rand = new Random(seed);
            startMs = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
            rotAxis = new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
            rotAxis = Vector3.Normalize(rotAxis);
            speed = (2 * (float)rand.NextDouble() - 1);
            Update();
        }

        public void Update()
        {
            float phase = 2 * (float)Math.PI * ((float)(speed * GetTimeElapsed() % 1000) / 1000);
            Quaternion dummyOrientation = Quaternion.CreateFromAxisAngle(rotAxis, phase);
            orientation = Quaternion.Normalize(dummyOrientation);
        }
        public Byte[] MakeBuffer()
        {
            Byte[] datagram = new Byte[14];
            // Our (new) data format;
            // ID (1 byte), time (Int32, 4 bytes), w, x, y, z (Int16, 2 bytes each), actionFlag (1 byte)
            // Old data format didn't have ID or actionFlag
            byte[] id = BitConverter.GetBytes(deviceId);
            byte[] time = BitConverter.GetBytes(GetTimeElapsed());
            byte[] w = BitConverter.GetBytes((int)(orientation.W * 16384));
            byte[] x = BitConverter.GetBytes((int)(orientation.X * 16384));
            byte[] y = BitConverter.GetBytes((int)(orientation.Y * 16384));
            byte[] z = BitConverter.GetBytes((int)(orientation.Z * 16384));
            byte[] actionFlag = BitConverter.GetBytes((int)0);
            Buffer.BlockCopy(id, 0, datagram, 0, 1);
            Buffer.BlockCopy(time, 0, datagram, 1, 4);
            Buffer.BlockCopy(w, 0, datagram, 5, 2);
            Buffer.BlockCopy(x, 0, datagram, 7, 2);
            Buffer.BlockCopy(y, 0, datagram, 9, 2);
            Buffer.BlockCopy(z, 0, datagram, 11, 2);
            Buffer.BlockCopy(actionFlag, 0, datagram, 13, 1);
            return datagram;
        }

        public void Transmit(IPEndPoint endpoint, Socket socket)
        {
            byte[] datagram = MakeBuffer();
            socket.SendTo(datagram, 0, datagram.Length, SocketFlags.None, endpoint);
        }
        private int GetTimeElapsed()
        {
            return (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - startMs;
        }
    }
}
