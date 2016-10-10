using EasySockets.Compression;
using EasySockets.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasySockets.Models
{
    public sealed class Client
    {
        public delegate void ServerConnectedEventHandler();
        /// <summary>
        /// Occurs when a client has successfully connected.
        /// </summary>
        public event ServerConnectedEventHandler ServerConnected;

        public delegate void DataReceivedEventHandler(Socket sock, object[] data);
        /// <summary>
        /// Occurs when data has been received from a connected client.
        /// </summary>
        public event DataReceivedEventHandler DataReceived;

        public delegate void ServerDisconnectedEventHandler();
        /// <summary>
        /// Occurs when a client has lost connection to the server.
        /// </summary>
        public event ServerDisconnectedEventHandler ServerDisconnected;

        /// <summary>
        /// Retrieves the encryption object.
        /// </summary>
        public Encryption.Encryption Encryption { get; set; }

        /// <summary>
        /// Retrieves the compression object.
        /// </summary>
        public ICompression Compression { get; set; }

        /// <summary>
        /// Retrieves the connection state.
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// Retrieves the buffer.
        /// </summary>
        internal byte[] Buffer { get; set; }

        /// <summary>
        /// Retrieves the socket.
        /// </summary>
        internal Socket Socket { get; set; }

        public Client()
        {
            Initialize();
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public Client(Socket socket)
        {
            Initialize();
            Socket = socket;
        }

        private void Initialize()
        {
            Buffer = new byte[1000000];
        }

        /// <summary>
        /// Handles the receiving of data.
        /// </summary>
        /// <param name="ar"></param>
        internal void OnReceiveCallback(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            int receivedLength = 0;

            try
            {
                receivedLength = sock.EndReceive(ar);
            }
            catch (SocketException)
            {
                OnServerDisconnected();
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
            Socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallback), Socket);
        }

        /// <summary>
        /// Handles the sending of data.
        /// </summary>
        /// <param name="ar"></param>
        internal void OnSendCallback(IAsyncResult ar)
        {
            Socket sock = ar.AsyncState as Socket;
            sock.EndSend(ar);
        }

        /// <summary>
        /// Calls the event for when the server connects.
        /// </summary>
        internal void OnServerConnected()
        {
            ServerConnected?.Invoke();
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
        /// Calls the event for when the server has disconnected.
        /// </summary>
        internal void OnServerDisconnected()
        {
            ServerDisconnected?.Invoke();
        }
    }
}
