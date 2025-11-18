using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUIApp
{
    public partial class Form1 : Form
    {
        private UdpClient? udpClient;
        private IPAddress multicastAddress = IPAddress.Parse("224.0.0.0");
        private int port = 10000;
        private CancellationTokenSource? cancellationTokenSource;
        private TextBox? messageTextBox;
        private CheckBox? heartbeatCheckBox;
        private TextBox? textboxSimIP;
        private TextBox? textboxSimPort;
        private Button? connectButton;
        private Label? labelIP;
        private Label? labelPort;
        private GroupBox Simulator;
        private Label labelSimStatus;
        private TcpClient? simulatorClient;
        private ZeroMQController? zmqController;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize status label
            labelSimStatus.Text = "Disconnected";
            labelSimStatus.BackColor = System.Drawing.Color.LightGray;

            // Status messages always display regardless of checkbox
            if (messageTextBox != null)
            {
                messageTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] Starting UDP Multicast listener on 224.0.0.0:10000...\r\n");
            }
            StartUdpMulticastListener();
        }

        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            if (textboxSimIP == null || textboxSimPort == null || messageTextBox == null || connectButton == null)
                return;

            string ipAddress = textboxSimIP.Text.Trim();
            string portText = textboxSimPort.Text.Trim();

            // Validate inputs
            if (string.IsNullOrEmpty(ipAddress))
            {
                MessageBox.Show("Please enter an IP address", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                labelSimStatus.Text = "Error";
                labelSimStatus.BackColor = System.Drawing.Color.Red;
                return;
            }

            if (!int.TryParse(portText, out int zmqPort) || zmqPort < 1 || zmqPort > 65535)
            {
                MessageBox.Show("Please enter a valid port number (1-65535)", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                labelSimStatus.Text = "Error";
                labelSimStatus.BackColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                // Disable button during connection attempt
                connectButton.Enabled = false;
                labelSimStatus.Text = "Connecting...";
                labelSimStatus.BackColor = System.Drawing.Color.Yellow;
                messageTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] Connecting to ZeroMQ endpoint tcp://{ipAddress}:{zmqPort}...\r\n");

                // Close existing connection if any
                if (zmqController != null)
                {
                    zmqController.Dispose();
                    zmqController = null;
                }

                // Create ZeroMQ controller with endpoint
                string endpoint = $"tcp://{ipAddress}:{zmqPort}";
                zmqController = new ZeroMQController(endpoint);

                // Connect to the endpoint
                zmqController.Connect();

                if (zmqController.IsConnected)
                {
                    messageTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] Successfully connected to ZeroMQ endpoint {endpoint}\r\n");
                    labelSimStatus.Text = "Connected";
                    labelSimStatus.BackColor = System.Drawing.Color.GreenYellow;
                    connectButton.Text = "Disconnect";
                }
            }
            catch (Exception ex)
            {
                messageTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] ZeroMQ connection failed: {ex.Message}\r\n");
                labelSimStatus.Text = "Error";
                labelSimStatus.BackColor = System.Drawing.Color.Red;
                MessageBox.Show($"Failed to connect to ZeroMQ endpoint\n\n{ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (zmqController != null)
                {
                    zmqController.Dispose();
                    zmqController = null;
                }
            }
            finally
            {
                connectButton.Enabled = true;
            }
        }

        private void StartUdpMulticastListener()
        {
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
                udpClient.JoinMulticastGroup(multicastAddress);

                // Status messages always display regardless of checkbox
                if (messageTextBox != null)
                {
                    messageTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] Joined multicast group {multicastAddress}:{port}\r\n");
                }

                // Start listening asynchronously
                cancellationTokenSource = new CancellationTokenSource();
                Task.Run(() => ReceiveMessages(cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                // Error messages always display regardless of checkbox
                if (messageTextBox != null)
                {
                    messageTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] Error starting UDP listener: {ex.Message}\r\n");
                }
                MessageBox.Show($"Error starting UDP listener: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ReceiveMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && udpClient != null)
            {
                try
                {
                    // Receive multicast message
                    UdpReceiveResult result = await udpClient.ReceiveAsync();

                    // Convert bytes to string
                    string message = Encoding.UTF8.GetString(result.Buffer);

                    // Display in textbox (marshal to UI thread) only if checkbox is checked
                    if (messageTextBox != null && heartbeatCheckBox != null)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (heartbeatCheckBox.Checked)
                            {
                                messageTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
                            }
                        });
                    }
                }
                catch (ObjectDisposedException)
                {
                    // UDP client was disposed, exit gracefully
                    break;
                }
                catch (Exception ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        // Error messages always display regardless of checkbox
                        if (messageTextBox != null)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                messageTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] Error: {ex.Message}\r\n");
                            });
                        }
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Clean up resources
            cancellationTokenSource?.Cancel();

            if (udpClient != null)
            {
                try
                {
                    udpClient.DropMulticastGroup(multicastAddress);
                    udpClient.Close();
                    udpClient.Dispose();
                }
                catch (Exception ex)
                {
                    // Ignore errors during cleanup
                }
            }

            if (simulatorClient != null)
            {
                try
                {
                    simulatorClient.Close();
                    simulatorClient.Dispose();
                }
                catch (Exception ex)
                {
                    // Ignore errors during cleanup
                }
            }

            if (zmqController != null)
            {
                try
                {
                    zmqController.Dispose();
                }
                catch (Exception ex)
                {
                    // Ignore errors during cleanup
                }
            }
        }

        private void InitializeComponent()
        {
            messageTextBox = new TextBox();
            heartbeatCheckBox = new CheckBox();
            textboxSimIP = new TextBox();
            textboxSimPort = new TextBox();
            connectButton = new Button();
            labelIP = new Label();
            labelPort = new Label();
            Simulator = new GroupBox();
            labelSimStatus = new Label();
            Simulator.SuspendLayout();
            SuspendLayout();
            // 
            // messageTextBox
            // 
            messageTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            messageTextBox.BackColor = System.Drawing.Color.Black;
            messageTextBox.ForeColor = System.Drawing.Color.GreenYellow;
            messageTextBox.Location = new System.Drawing.Point(172, 354);
            messageTextBox.Multiline = true;
            messageTextBox.Name = "messageTextBox";
            messageTextBox.ReadOnly = true;
            messageTextBox.ScrollBars = ScrollBars.Vertical;
            messageTextBox.Size = new System.Drawing.Size(629, 150);
            messageTextBox.TabIndex = 0;
            // 
            // heartbeatCheckBox
            // 
            heartbeatCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            heartbeatCheckBox.AutoSize = true;
            heartbeatCheckBox.Checked = true;
            heartbeatCheckBox.CheckState = CheckState.Checked;
            heartbeatCheckBox.Location = new System.Drawing.Point(12, 363);
            heartbeatCheckBox.Name = "heartbeatCheckBox";
            heartbeatCheckBox.Padding = new Padding(5);
            heartbeatCheckBox.Size = new System.Drawing.Size(93, 29);
            heartbeatCheckBox.TabIndex = 1;
            heartbeatCheckBox.Text = "Heartbeats";
            heartbeatCheckBox.UseVisualStyleBackColor = true;
            // 
            // textboxSimIP
            // 
            textboxSimIP.Location = new System.Drawing.Point(44, 29);
            textboxSimIP.Name = "textboxSimIP";
            textboxSimIP.Size = new System.Drawing.Size(150, 23);
            textboxSimIP.TabIndex = 3;
            textboxSimIP.Text = "127.0.0.1";
            // 
            // textboxSimPort
            // 
            textboxSimPort.Location = new System.Drawing.Point(44, 58);
            textboxSimPort.Name = "textboxSimPort";
            textboxSimPort.Size = new System.Drawing.Size(48, 23);
            textboxSimPort.TabIndex = 5;
            textboxSimPort.Text = "5000";
            // 
            // connectButton
            // 
            connectButton.Location = new System.Drawing.Point(98, 58);
            connectButton.Name = "connectButton";
            connectButton.Size = new System.Drawing.Size(96, 25);
            connectButton.TabIndex = 6;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += ConnectButton_Click;
            // 
            // labelIP
            // 
            labelIP.AutoSize = true;
            labelIP.Location = new System.Drawing.Point(18, 32);
            labelIP.Name = "labelIP";
            labelIP.Size = new System.Drawing.Size(20, 15);
            labelIP.TabIndex = 2;
            labelIP.Text = "IP:";
            // 
            // labelPort
            // 
            labelPort.AutoSize = true;
            labelPort.Location = new System.Drawing.Point(4, 61);
            labelPort.Name = "labelPort";
            labelPort.Size = new System.Drawing.Size(32, 15);
            labelPort.TabIndex = 4;
            labelPort.Text = "Port:";
            // 
            // Simulator
            // 
            Simulator.Controls.Add(labelSimStatus);
            Simulator.Controls.Add(textboxSimIP);
            Simulator.Controls.Add(connectButton);
            Simulator.Controls.Add(labelPort);
            Simulator.Controls.Add(labelIP);
            Simulator.Controls.Add(textboxSimPort);
            Simulator.Location = new System.Drawing.Point(12, 12);
            Simulator.Name = "Simulator";
            Simulator.Size = new System.Drawing.Size(210, 170);
            Simulator.TabIndex = 7;
            Simulator.TabStop = false;
            Simulator.Text = "groupBox1";
            // 
            // labelSimStatus
            // 
            labelSimStatus.Location = new System.Drawing.Point(18, 144);
            labelSimStatus.Name = "labelSimStatus";
            labelSimStatus.Size = new System.Drawing.Size(158, 23);
            labelSimStatus.TabIndex = 7;
            labelSimStatus.Text = "label1";
            labelSimStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 504);
            Controls.Add(Simulator);
            Controls.Add(messageTextBox);
            Controls.Add(heartbeatCheckBox);
            Name = "Form1";
            Text = "GUIApp - UDP Multicast Listener";
            Load += Form1_Load;
            Simulator.ResumeLayout(false);
            Simulator.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
