using EasySockets.Encryption;
using EasySockets.Models;
using EasySockets.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasySockets.Controllers
{
    public sealed class ClientController
    {
        /// <summary>
        /// Continuously establishes a connection to a remote host until it has connected.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public static void Connect(Client client, IPAddress ip, int port)
        {
            while (!client.Socket.Connected)
            {
                try
                {
                    client.Socket.Connect(ip, port);
                }
                catch (SocketException) { }
            }

            client.OnServerConnected();
            client.Socket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, new AsyncCallback(client.OnReceiveCallback), client.Socket);
        }

        /// <summary>
        /// Sends data in the form of an object array to the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        public static void Send(Client client, object[] data)
        {
            byte[] serializedPacket = BinarySerializer.Serialize(data);

            if (client.Compression != null)
            {
                serializedPacket = client.Compression.Compress(serializedPacket);
            }

            if (client.Encryption != null)
            {
                serializedPacket = client.Encryption.Encrypt(serializedPacket);
            }

            client.Socket.BeginSend(serializedPacket, 0, serializedPacket.Length, SocketFlags.None, new AsyncCallback(client.OnSendCallback), client.Socket);
        }
    }
}
