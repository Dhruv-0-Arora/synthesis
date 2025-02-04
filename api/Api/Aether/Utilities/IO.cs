﻿using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using SynthesisServer.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SynthesisAPI.Aether
{
    public static class IO
    {
        public static byte[] GetNextMessage(ref byte[] buffer)
        {
            byte[] msgLength = new byte[sizeof(int)];
            Array.Copy(buffer, 0, msgLength, 0, msgLength.Length);

            byte[] msg = new byte[BitConverter.ToInt32(msgLength, 0)];
            Array.Copy(buffer, sizeof(int), msg, 0, msg.Length);

            buffer = buffer.Skip(msgLength.Length + msg.Length).ToArray();
            return msg;
        }
        public static void SendMessage(IMessage msg, string clientID, Socket socket, AsyncCallback sendCallback)
        {
            Any packedMsg = Any.Pack(msg);
            byte[] msgBytes = new byte[packedMsg.CalculateSize()];
            packedMsg.WriteTo(msgBytes);

            MessageHeader header = new MessageHeader() { IsEncrypted = false, ClientId = clientID };
            byte[] headerBytes = new byte[header.CalculateSize()];
            header.WriteTo(headerBytes);

            byte[] data = new byte[sizeof(int) + headerBytes.Length + sizeof(int) + msgBytes.Length];

            BitConverter.GetBytes(headerBytes.Length).CopyTo(data, 0);
            headerBytes.CopyTo(data, sizeof(int));

            BitConverter.GetBytes(msgBytes.Length).CopyTo(data, sizeof(int) + headerBytes.Length);
            msgBytes.CopyTo(data, sizeof(int) + headerBytes.Length + sizeof(int));

            if (BitConverter.IsLittleEndian) { Array.Reverse(data); }
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, sendCallback, null);
        }
        public static void SendEncryptedMessage(IMessage msg, string clientID, byte[] symmetricKey, Socket socket, SymmetricEncryptor encryptor, AsyncCallback sendCallback)
        {
            Any packedMsg = Any.Pack(msg);
            byte[] msgBytes = new byte[packedMsg.CalculateSize()];
            packedMsg.WriteTo(msgBytes);

            byte[] delimitedMessage = new byte[sizeof(int) + msgBytes.Length];
            BitConverter.GetBytes(msgBytes.Length).CopyTo(delimitedMessage, 0);
            msgBytes.CopyTo(delimitedMessage, sizeof(int));

            byte[] encryptedMessage = encryptor.Encrypt(delimitedMessage, symmetricKey);

            MessageHeader header = new MessageHeader() { IsEncrypted = true, ClientId = clientID };
            byte[] headerBytes = new byte[header.CalculateSize()];
            header.WriteTo(headerBytes);

            byte[] data = new byte[sizeof(int) + headerBytes.Length + encryptedMessage.Length];

            BitConverter.GetBytes(headerBytes.Length).CopyTo(data, 0);
            headerBytes.CopyTo(data, sizeof(int));

            encryptedMessage.CopyTo(data, sizeof(int) + headerBytes.Length);

            if (BitConverter.IsLittleEndian) { Array.Reverse(data); }
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, sendCallback, null);
        }
        public static void SendMessageTo(IMessage msg, string clientID, Socket socket, EndPoint remoteEP, AsyncCallback sendCallback)
        {
            Any packedMsg = Any.Pack(msg);
            byte[] msgBytes = new byte[packedMsg.CalculateSize()];
            packedMsg.WriteTo(msgBytes);

            MessageHeader header = new MessageHeader() { IsEncrypted = false, ClientId = clientID };
            byte[] headerBytes = new byte[header.CalculateSize()];
            header.WriteTo(headerBytes);

            byte[] data = new byte[sizeof(int) + headerBytes.Length + sizeof(int) + msgBytes.Length];

            BitConverter.GetBytes(headerBytes.Length).CopyTo(data, 0);
            headerBytes.CopyTo(data, sizeof(int));

            BitConverter.GetBytes(msgBytes.Length).CopyTo(data, sizeof(int) + headerBytes.Length);
            msgBytes.CopyTo(data, sizeof(int) + headerBytes.Length + sizeof(int));

            if (BitConverter.IsLittleEndian) { Array.Reverse(data); }
            socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, remoteEP, sendCallback, null);
        }
        public static void SendEncryptedMessageTo(IMessage msg, string clientID, byte[] symmetricKey, Socket socket, EndPoint remoteEP, SymmetricEncryptor encryptor, AsyncCallback sendCallback)
        {
            Any packedMsg = Any.Pack(msg);
            byte[] msgBytes = new byte[packedMsg.CalculateSize()];
            packedMsg.WriteTo(msgBytes);

            byte[] delimitedMessage = new byte[sizeof(int) + msgBytes.Length];
            BitConverter.GetBytes(msgBytes.Length).CopyTo(delimitedMessage, 0);
            msgBytes.CopyTo(delimitedMessage, sizeof(int));

            byte[] encryptedMessage = encryptor.Encrypt(delimitedMessage, symmetricKey);

            MessageHeader header = new MessageHeader() { IsEncrypted = true, ClientId = clientID };
            byte[] headerBytes = new byte[header.CalculateSize()];
            header.WriteTo(headerBytes);

            byte[] data = new byte[sizeof(int) + headerBytes.Length + encryptedMessage.Length];

            BitConverter.GetBytes(headerBytes.Length).CopyTo(data, 0);
            headerBytes.CopyTo(data, sizeof(int));

            encryptedMessage.CopyTo(data, sizeof(int) + headerBytes.Length);

            if (BitConverter.IsLittleEndian) { Array.Reverse(data); }
            socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, remoteEP, sendCallback, null);
        }

        public static byte[] GetBuffer(this IMessage msg) {
            byte[] buf = new byte[msg.CalculateSize()];
            msg.WriteTo(buf);
            return buf;
        }
    }
}
