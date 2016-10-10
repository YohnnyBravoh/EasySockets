using EasySockets.Compression;
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
    public sealed class ServerController
    {
        /// <summary>
        /// Associates the server socket with a local endpoint, places it in a listening state and accepts incoming connection attempts.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        public static void Listen(Server server, int port)
        {
            if (!server.Socket.Connected)
            {
                server.Socket.Bind(new IPEndPoint(IPAddress.Any, port));
                server.Socket.Listen(10);
                server.Socket.BeginAccept(new AsyncCallback(server.OnAcceptCallback), null);
            }
        }

        /// <summary>
        /// Sends data in the form of an object array to the specified socket.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="sock"></param>
        /// <param name="data"></param>
        public static void Send(Server server, Socket sock, object[] data)
        {
            byte[] serializedPacket = BinarySerializer.Serialize(data);

            if (server.Compression != null)
            {
                serializedPacket = server.Compression.Compress(serializedPacket);
            }

            if (server.Encryption != null)
            {
                serializedPacket = server.Encryption.Encrypt(serializedPacket);
            }

            sock.BeginSend(serializedPacket, 0, serializedPacket.Length, SocketFlags.None, new AsyncCallback(server.OnSendCallback), sock);
        }
    }
}
