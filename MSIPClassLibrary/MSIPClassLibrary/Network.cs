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
    
        public class NetworkInterface:IDisposable
        {
            private DatagramSocket _socket;

            public bool IsConnected { get; set; }

            public NetworkInterface()
            {
                IsConnected = false;

                _socket = new DatagramSocket();
                _socket.MessageReceived += OnSocketMessageReceived;
                
            }

            public async Task Connect(HostName remoteHostName, string remoteServiceNameOrPort)
            {
               
                if(IsConnected!=true)
                  await _socket.ConnectAsync(remoteHostName,remoteServiceNameOrPort);

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

            
            public async void SendMessage(string message)
            {
               

                DataWriter _writer = null;
                if (_writer == null)
                {
                    
                 //   var stream = _socket.OutputStream;
                    _writer = new DataWriter(_socket.OutputStream);
                }

                
                Encoding enc = Encoding.GetEncoding("us-ascii");
                Byte[] sendBytes = enc.GetBytes(message);

                
                _writer.WriteBytes(sendBytes);
                //var writer=
             //   if(!IsConnected)
             //       this.Connect(new HostName("81.88.80.235"), "5060");

                await _writer.StoreAsync();
                

            }

            public void Dispose()
            {
                _socket.Dispose();
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
