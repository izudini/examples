using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace UdpMulticastListener
{
    public partial class Form1 : Form
    {
        private UdpClient? udpClient;
        private Thread? listenerThread;
        private bool isListening = false;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            // Start listening to UDP multicast messages
            StartMulticastListener();
        }

        private void StartMulticastListener()
        {
            try
            {
                // Create UDP client and bind to port 10000
                udpClient = new UdpClient(10000);

                // Join the multicast group
                IPAddress multicastAddress = IPAddress.Parse("224.0.0.0");
                udpClient.JoinMulticastGroup(multicastAddress);

                isListening = true;

                // Start listener thread
                listenerThread = new Thread(ListenForMessages);
                listenerThread.IsBackground = true;
                listenerThread.Start();

                Console.WriteLine("Started listening for UDP multicast messages on 224.0.0.0:10000");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting multicast listener: {ex.Message}");
                MessageBox.Show($"Error starting multicast listener: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListenForMessages()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (isListening && udpClient != null)
            {
                try
                {
                    // Receive multicast message
                    byte[] receivedData = udpClient.Receive(ref remoteEndPoint);

                    // Convert to string
                    string message = Encoding.UTF8.GetString(receivedData);

                    // Print to console
                    Console.WriteLine($"Received message from {remoteEndPoint}: {message}");
                }
                catch (SocketException)
                {
                    // Socket closed, exit loop
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving message: {ex.Message}");
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Clean up resources
            isListening = false;

            if (udpClient != null)
            {
                try
                {
                    udpClient.Close();
                }
                catch { }
            }

            if (listenerThread != null && listenerThread.IsAlive)
            {
                listenerThread.Join(1000); // Wait up to 1 second for thread to finish
            }

            base.OnFormClosing(e);
        }
    }
}
