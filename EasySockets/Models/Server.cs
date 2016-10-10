using EasySockets.Compression;
using EasySockets.Encryption;
using EasySockets.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasySockets.Models
{
    public sealed class Server
    {
        public delegate void ClientAcceptedEventHandler(Socket sock);
        /// <summary>
        /// Occurs when a client has successfully connected.
        /// </summary>
        public event ClientAcceptedEventHandler ClientAccepted;

        public delegate void DataReceivedEventHandler(Socket sock, object[] data);
        /// <summary>
        /// Occurs when data has been received from a connected client.
        /// </summary>
        public event DataReceivedEventHandler DataReceived;

        public delegate void ClientDisconnectedEventHandler(Socket sock);
        /// <summary>
        /// Occurs when a client has lost connection to the server.
        /// </summary>
        public event ClientDisconnectedEventHandler ClientDisconnected;

        /// <summary>
        /// Retrieves the encryption object.
        /// </summary>
        public Encryption.Encryption Encryption { get; set; }

        /// <summary>
        /// Retrieves the compression object.
        /// </summary>
        public ICompression Compression { get; set; }

        /// <summary>
        /// Retrieves the connected clients.
        /// </summary>
        public List<Socket> Clients { get; set; }

        /// <summary>
        /// Retrieves the buffer.
        /// </summary>
        internal byte[] Buffer { get; set; }

        /// <summary>
        /// Retrieves the socket.
        /// </summary>
        internal Socket Socket { get; set; }

        public Server()
        {
            Initialize();
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public Server(Socket socket)
        {
            Initialize();
            Socket = socket;
        }

        private void Initialize()
        {
            Clients = new List<Socket>();
            Buffer = new byte[1000000];
        }

        /// <summary>
        /// Handles the accepting and connecting of clients.
        /// </summary>
        /// <param name="ar"></param>
        internal void OnAcceptCallback(IAsyncResult ar)
        {
            Socket sock = Socket.EndAccept(ar);
            OnClientAccepted(sock);

            sock.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallback), sock);
            Socket.BeginAccept(new AsyncCallback(OnAcceptCallback), null);
        }

        /// <summary>
        /// Handles the receiving of data.
        /// </summary>
        /// <param name="ar"></param>
        internal void OnReceiveCallback(IAsyncResult ar)
        {
            var sock = (Socket)ar.AsyncState;
            int receivedLength = 0;

            try
            {
                receivedLength = sock.EndReceive(ar);
            }
            catch (Exception)
            {
                OnClientDisconnected(sock);
                return;
            }

            byte[] packet = new byte[receivedLength];
            System.Buffer.BlockCopy(Buffer, 0, packet, 0, receivedLength);

            if (Encryption != null)
            {
                packet = Encryption.Decrypt(packet);
            }

            if (Compression != null)
            {
                packet = Compression.Decompress(packet);
            }

            OnDataReceived(sock, BinarySerializer.Deserialize<object[]>(packet));
            sock.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallback), sock);
        }

        /// <summary>
        /// Handles the sending of data.
        /// </summary>
        /// <param name="ar"></param>
        internal void OnSendCallback(IAsyncResult ar)
        {
            var sock = (Socket)ar.AsyncState;
            sock.EndSend(ar);
        }

        /// <summary>
        /// Calls the event for when a client has been accepted and has connected.
        /// </summary>
        /// <param name="sock"></param>
        internal void OnClientAccepted(Socket sock)
        {
            Clients.Add(sock);
            ClientAccepted?.Invoke(sock);
        }

        /// <summary>
        /// Calls the event for when data has been received.
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="data"></param>
        internal void OnDataReceived(Socket sock, object[] data)
        {
            DataReceived?.Invoke(sock, data);
        }

        /// <summary>
        /// Calls the event for when the client has disconnected.
        /// </summary>
        /// <param name="sock"></param>
        internal void OnClientDisconnected(Socket sock)
        {
            Clients.Remove(sock);
            ClientDisconnected?.Invoke(sock);
        }
    }
}
