using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using GUIApp.Comm;

namespace GUIApp
{
    public class HeartbeatListener : IDisposable
    {
        private string ipAddress;
        private int port;
        private UdpClient? udpClient;
        private CancellationTokenSource? cancellationTokenSource;
        private bool isListening;
        public ConcurrentQueue<SimulatorStatus> statusQueue;

        public event EventHandler<string>? MessageReceived;
        public ConcurrentQueue<SimulatorStatus> StatusQueue => statusQueue;

        public HeartbeatListener(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            isListening = false;
            statusQueue = new ConcurrentQueue<SimulatorStatus>();
        }

        public void Start()
        {
            if (isListening)
            {
                throw new InvalidOperationException("Heartbeat listener is already running");
            }

            try
            {
                // Create UDP client
                udpClient = new UdpClient();
                udpClient.ExclusiveAddressUse = false;

                // Bind to the port
                IPEndPoint localEp = new IPEndPoint(IPAddress.Any, port);
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(localEp);

                // Join the multicast group
                IPAddress multicastAddress = IPAddress.Parse(ipAddress);
                udpClient.JoinMulticastGroup(multicastAddress);

                // Start listening asynchronously
                cancellationTokenSource = new CancellationTokenSource();
                isListening = true;
                Task.Run(() => ReceiveMessages(cancellationTokenSource.Token));
            }
            catch (Exception)
            {
                // Clean up on error
                if (udpClient != null)
                {
                    udpClient.Close();
                    udpClient.Dispose();
                    udpClient = null;
                }
                isListening = false;
                throw;
            }
        }

        public void Stop()
        {
            if (!isListening)
            {
                return;
            }

            // Cancel the listening task
            cancellationTokenSource?.Cancel();

            // Clean up UDP client
            if (udpClient != null)
            {
                try
                {
                    IPAddress multicastAddress = IPAddress.Parse(ipAddress);
                    udpClient.DropMulticastGroup(multicastAddress);
                    udpClient.Close();
                    udpClient.Dispose();
                }
                catch (Exception)
                {
                    // Ignore errors during cleanup
                }
                finally
                {
                    udpClient = null;
                }
            }

            isListening = false;
        }

        private async Task ReceiveMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && udpClient != null)
            {
                try
                {
                    // Receive multicast message
                    UdpReceiveResult result = await udpClient.ReceiveAsync();

                    // Deserialize the Protocol Buffer message
                    SimulatorStatus status = SimulatorStatus.Parser.ParseFrom(result.Buffer);

                    // Add to concurrent queue
                    statusQueue.Enqueue(status);

                    // Also raise event with string representation for backward compatibility
                    string message = status.ToString();
                    MessageReceived?.Invoke(this, message);
                }
                catch (ObjectDisposedException)
                {
                    // UDP client was disposed, exit gracefully
                    break;
                }
                catch (InvalidProtocolBufferException)
                {
                    // Failed to deserialize - skip this message and continue listening
                    continue;
                }
                catch (Exception)
                {
                    // Continue listening even if there's an error with one message
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
        }

        public void Dispose()
        {
            Stop();
            cancellationTokenSource?.Dispose();
        }
    }
}
