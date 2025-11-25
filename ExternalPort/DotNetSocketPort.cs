using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ExternalPort
{
    /// <summary>Socketクラスを使いイーサネット通信を行うクラス</summary>
    /// <remarks></remarks>
    internal class DotNetSocketPort : IExternalPort, IDisposable
    {
        protected Socket client;

        public bool Connect()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (client.Connected == false)
            {
                //clinet.Connect();
                return true;
            }

            return false;
        }

        public Task<bool> ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public byte[] Receive()
        {
            while(client.Available == 0)
            {
            }
            byte[] buffer = new byte[client.Available];
            int bytesRead = client.Receive(buffer);

            return buffer;

            /*var buf = new byte[1024]; //この値もどこかで設定できるようにする
            var len = client.Receive(buf);

            return Encoding.UTF8.GetString(buf, 0, len);*/
        }

        public Task<byte[]> ReciveAsync()
        {
            throw new NotImplementedException();
        }

        public void Send(string cmd) //stringではなくintにすること
        {
            var bytes = Encoding.UTF8.GetBytes(cmd);
            var len = client.Send(bytes);
        }

        public Task SendAsync(string cmd)
        {
            throw new NotImplementedException();
        }
    }
}
