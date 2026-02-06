using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DnD.Service;
using Google.Protobuf;
using System;
using System.Text;

public class ServerListener
{
    public static TerrainGenerator tg;
    public static void Listen(NetworkStream stream)
    {
        Thread listenerThread = new Thread(() => { startListening(stream); });
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    private static void startListening(NetworkStream stream)
    {
        Debug.Log("[Server Listener] Started Listening");

        while (true)
        {
            try
            {
                // 1️⃣ Read length
                byte[] lengthBuffer = new byte[4];
                if (!ReadExact(stream, lengthBuffer, 4))
                {
                    Debug.Log("[Server Listener] Disconnected");
                    break;
                }

                Array.Reverse(lengthBuffer); // Java → C#
                int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                if (messageLength < 0)
                {
                    Debug.LogError("[Server Listener] Invalid message length: " + messageLength);
                    break;
                }

                // 2️⃣ Read message
                byte[] messageBuffer = new byte[messageLength];
                if (!ReadExact(stream, messageBuffer, messageLength))
                {
                    Debug.Log("[Server Listener] Disconnected while reading message");
                    break;
                }

                // 3️⃣ Parse protobuf
                ServerResponse response = ServerResponse.Parser.ParseFrom(messageBuffer);
                Debug.Log("[Server Listener] Received: " + response.Response);

                ServerResponseHandler.handleResponse(response);
            }
            catch (Exception e)
            {
                Debug.LogError("[Server Listener] " + e);
                break;
            }
        }
    }

    private static bool ReadExact(NetworkStream stream, byte[] buffer, int length)
    {
        int offset = 0;
        while (offset < length)
        {
            int read = stream.Read(buffer, offset, length - offset);
            if (read == 0)
                return false; // disconnected

            offset += read;
        }
        return true;
    }


}
