using System;
using NetMQ;
using NetMQ.Sockets;

namespace GUIApp
{
    public class ZeroMQController : IDisposable
    {
        private PublisherSocket? publisherSocket;
        private string endpoint;
        private bool isConnected;

        public bool IsConnected => isConnected;

        public ZeroMQController(string endpoint = "tcp://localhost:5555")
        {
            this.endpoint = endpoint;
            isConnected = false;
        }

        /// <summary>
        /// Connects to the ZeroMQ endpoint
        /// </summary>
        public void Connect()
        {
            if (isConnected)
            {
                throw new InvalidOperationException("Already connected to ZeroMQ endpoint");
            }

            try
            {
                publisherSocket = new PublisherSocket();
                publisherSocket.Connect(endpoint);
                isConnected = true;
            }
            catch (Exception)
            {
                publisherSocket?.Dispose();
                publisherSocket = null;
                isConnected = false;
                throw;
            }
        }

        /// <summary>
        /// Binds to the ZeroMQ endpoint (for server mode)
        /// </summary>
        public void Bind()
        {
            if (isConnected)
            {
                throw new InvalidOperationException("Already connected to ZeroMQ endpoint");
            }

            try
            {
                publisherSocket = new PublisherSocket();
                publisherSocket.Bind(endpoint);
                isConnected = true;
            }
            catch (Exception)
            {
                publisherSocket?.Dispose();
                publisherSocket = null;
                isConnected = false;
                throw;
            }
        }

        /// <summary>
        /// Sends a string message to the ZeroMQ recipient
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SendMessage(string message)
        {
            if (!isConnected || publisherSocket == null)
            {
                throw new InvalidOperationException("Not connected to ZeroMQ endpoint. Call Connect() or Bind() first.");
            }

            publisherSocket.SendFrame(message);
        }

        /// <summary>
        /// Sends the START command to the ZeroMQ recipient
        /// </summary>
        public void SendStart()
        {
            SendMessage("START");
        }

        /// <summary>
        /// Sends the STOP command to the ZeroMQ recipient
        /// </summary>
        public void SendStop()
        {
            SendMessage("STOP");
        }

        /// <summary>
        /// Disconnects from the ZeroMQ endpoint
        /// </summary>
        public void Disconnect()
        {
            if (publisherSocket != null)
            {
                publisherSocket.Dispose();
                publisherSocket = null;
            }
            isConnected = false;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
