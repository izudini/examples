using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GUIApp
{
    public class Communicator : IDisposable
    {
        private string ipAddress;
        private int port;
        private UdpClient? udpClient;

        public Communicator(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            udpClient = new UdpClient();
        }

        public void SendMessage(string message)
        {
            if (udpClient == null)
            {
                throw new InvalidOperationException("UDP client is not initialized");
            }

            // Convert string to bytes
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Create endpoint
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            // Send the message
            udpClient.Send(data, data.Length, endPoint);
        }

        public void Dispose()
        {
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient.Dispose();
                udpClient = null;
            }
        }
    }
}
