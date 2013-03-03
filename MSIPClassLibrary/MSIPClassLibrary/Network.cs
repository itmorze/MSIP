using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace MSIPClassLibrary
{
    
        public class NetworkInterface
        {
            private DatagramSocket _socket;

            public bool IsConnected { get; set; }

            public NetworkInterface()
            {
                IsConnected = false;

                _socket = new DatagramSocket();
                _socket.MessageReceived += OnSocketMessageReceived;
                
            }

            public async void Connect(HostName remoteHostName, string remoteServiceNameOrPort)
            {
                await _socket.ConnectAsync(remoteHostName, remoteServiceNameOrPort);

                IsConnected = true;
            }


            public event EventHandler<MessageReceivedEventArgs> MessageReceived;

            private void OnSocketMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
            {
                var reader = args.GetDataReader();

                var count = reader.UnconsumedBufferLength;

                var data = reader.ReadString(count);

                if (MessageReceived != null)
                {
                    var ea = new MessageReceivedEventArgs();
                    ea.Message = new udpMessage() { SIP = data };
                    ea.RemoteHostName = args.RemoteAddress;
                    ea.RemotePort = args.RemotePort;

                    MessageReceived(this, ea);
                }
            }

            DataWriter _writer = null;
            public async void SendMessage(string message)
            {
                if (_writer == null)
                {
                    var stream = _socket.OutputStream;
                    _writer = new DataWriter(stream);
                }

                
                //Byte[] sendBytes = Encoding.ASCII.GetBytes(Info);
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] encodedBytes = utf8.GetBytes(message);
                
                
                Encoding enc = Encoding.GetEncoding("us-ascii");
                Byte[] sendBytes = enc.GetBytes(message);
          //      string newstr = enc.GetString(sendBytes,0,sendBytes.Length);
                
                _writer.WriteBytes(sendBytes);
                await _writer.StoreAsync();
            }
        }

        public class udpMessage
        {
            public string SIP { get; set; }
        }

        public class MessageReceivedEventArgs
        {
            public udpMessage Message { get; set; }
            public HostName RemoteHostName { get; set; }
            public string RemotePort { get; set; }
        }
}
