using NetMQ.Sockets;
using Google.Protobuf;
using NetMQ;

namespace STAMP_GUI
{
    public class AIECS_Controller : IDisposable
    {
        private DealerSocket? zmqSocket;
        

        public bool IsConnected { get; private set; } = false;

        public async Task<bool> Connect()
        {
            if (IsConnected)
            {
                return true;
            }

            bool success = false;

            await Task.Run(() =>
            {
                try
                {
                    zmqSocket = new DealerSocket();
                    zmqSocket.Options.Linger = TimeSpan.FromSeconds(1);

                    string endpoint = $"tcp://{Globals.AIECS_IP}:{Globals.AIECS_PORT}";
                    zmqSocket.Connect(endpoint);
                    success = true;
                    IsConnected = true;
                }
                catch (Exception ex)
                {
                    success = false;
                    IsConnected = false;
                    if (zmqSocket != null)
                    {
                        zmqSocket.Dispose();
                        zmqSocket = null;
                    }
                }
            });

            return success;
        }

        public bool Disconnect()
        {
            try
            {
                if (zmqSocket != null)
                {
                    zmqSocket.Disconnect($"tcp://{Globals.AIECS_IP}:{Globals.AIECS_PORT}");
                    zmqSocket.Dispose();
                    zmqSocket = null;
                }
                IsConnected = false;
                return true;
            }
            catch (Exception)
            {
                IsConnected = false;
                return false;
            }
        }

        public bool SendMessage<T>(T message) where T : IMessage<T>
        {
            if (!IsConnected || zmqSocket == null)
            {
                return false;
            }

            try
            {
                byte[] messageBytes = message.ToByteArray();
                zmqSocket.SendFrame(messageBytes);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public void Dispose()
        {
            Disconnect();
        }
    }
}
