using NetMQ;
using NetMQ.Sockets;

namespace STAMP_GUI
{
    public partial class Form1 : Form
    {
        private DealerSocket? zmqSocket;
        private const string STAMP_IP = "192.168.1.10";
        private const int STAMP_PORT = 40000;

        public Form1()
        {
            InitializeComponent();
        }

        private async void buttonConnectToSTAMP_Click(object? sender, EventArgs e)
        {
            bool success = false;

            try
            {
                buttonConnectToSTAMP.Enabled = false;
                buttonConnectToSTAMP.Text = "Connecting...";

                await Task.Run(() =>
                {
                    try
                    {
                        // Create ZeroMQ socket
                        zmqSocket = new DealerSocket();

                        // Set socket options for connection timeout
                        zmqSocket.Options.Linger = TimeSpan.FromSeconds(1);

                        // Connect to the STAMP device
                        string endpoint = $"tcp://{STAMP_IP}:{STAMP_PORT}";
                        zmqSocket.Connect(endpoint);

                        // Connection successful
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        // Connection failed
                        success = false;

                        // Clean up socket if it was created
                        if (zmqSocket != null)
                        {
                            zmqSocket.Dispose();
                            zmqSocket = null;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                success = false;
            }
            finally
            {
                // Update UI back on UI thread
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        buttonConnectToSTAMP.Enabled = true;
                        buttonConnectToSTAMP.Text = success ? "Connected" : "Connect to STAMP";
                        ConnectedToStamp(success);
                    }));
                }
                else
                {
                    buttonConnectToSTAMP.Enabled = true;
                    buttonConnectToSTAMP.Text = success ? "Connected" : "Connect to STAMP";
                    ConnectedToStamp(success);
                }
            }
        }

        private void ConnectedToStamp(bool success)
        {
            if (success)
            {
                MessageBox.Show("Successfully connected to STAMP device!", "Connection Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Failed to connect to STAMP device at {STAMP_IP}:{STAMP_PORT}",
                    "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up ZeroMQ socket
                if (zmqSocket != null)
                {
                    zmqSocket.Dispose();
                    zmqSocket = null;
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
