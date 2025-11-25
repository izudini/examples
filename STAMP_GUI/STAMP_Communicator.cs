using NetMQ;
using NetMQ.Sockets;
using Google.Protobuf;
using GUIApp.Comm;

namespace STAMP_GUI
{
    public class STAMP_Communicator : IDisposable
    {
        private DealerSocket? zmqSocket;
        
        private bool isConnected = false;

        public bool IsConnected => isConnected;

        public async Task<bool> Connect()
        {
            bool success = false;

            await Task.Run(() =>
            {
                try
                {
                    zmqSocket = new DealerSocket();
                    zmqSocket.Options.Linger = TimeSpan.FromSeconds(1);

                    string endpoint = $"tcp://{Globals.STAMP_IP}:{Globals.STAMP_PORT}";
                    zmqSocket.Connect(endpoint);
                    isConnected = true;
                    success = true;
                }
                catch (Exception ex)
                {
                    success = false;
                    isConnected = false;
                    if (zmqSocket != null)
                    {
                        zmqSocket.Dispose();
                        zmqSocket = null;
                    } 
                }
            });

            return success;
        }

     
        public bool SendMessage(IMessage message)
        {
            if (!isConnected || zmqSocket == null)
            {
                return false;
            }

            try
            {
                // Serialize the protobuf message to a byte array
                byte[] messageBytes = message.ToByteArray();

                // Send the byte array over ZeroMQ
                zmqSocket.SendFrame(messageBytes);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SendControlCommand(Command command)
        {
            var controlMessage = new STAMP_Control
            {
                STAMPCommand = command
            };

            return SendMessage(controlMessage);
        }

        public bool SendSubsystemStatusMessage(SystemStatus status)
        {
            var statusMessage = new Configure_STAMP_Status
            {
                Status = status
            };

            return SendMessage(statusMessage);
        }



        public void Dispose()
        {
            isConnected = false;
            if (zmqSocket != null)
            {
                zmqSocket.Dispose();
                zmqSocket = null;
            }
        }
    }
}
