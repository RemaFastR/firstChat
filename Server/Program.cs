using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        public static List<Socket> clients = new List<Socket>();

        
        public static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
        public static short Port = 1234; 
        static void Main(string[] args)
        {
            StartListening();
            Console.ReadLine();
        }

        public static void StartListening()
        {
            try
            {
                Console.WriteLine($"Listening started port:{Port} protocol type: {ProtocolType.Tcp}");
                socket.Bind(new IPEndPoint(IPAddress.Any, Port));
                socket.Listen(10);
                socket.BeginAccept(AcceptCallback, socket);
            }
            catch (Exception ex)
            {
                throw new Exception("listening error" + ex);
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine($"Accept CallBack port:{Port} protocol type: {ProtocolType.Tcp}");
                Socket acceptedSocket = socket.EndAccept(ar);
                clients.Add(acceptedSocket);

                socket.BeginAccept(AcceptCallback, socket);
            }
            catch (Exception ex)
            {
                throw new Exception("Base Accept error" + ex);
            }
        }
        Socket _receiveSocket = socket;

        byte[] _buffer = new byte[1024];
        public void StartReceiving()
        {
           

            try
            {
                _buffer = new byte[1024];
                _receiveSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch { }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                if (_receiveSocket.EndReceive(AR) > 1)
                {
                    // Convert the first 4 bytes (int 32) that we received and convert it to an Int32 (this is the size for the coming data).
                    _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                    // Next receive this data into the buffer with size that we did receive before
                    _receiveSocket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                    string data = Encoding.Default.GetString(_buffer);
                    Console.WriteLine(data);
                    // Now we have to start all over again with waiting for a data to come from the socket.
                    StartReceiving();
                }
                else
                {
                }
            }
            catch
            {
               
            }
        }



    }
}
