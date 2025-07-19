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
        if(stream != null)
        {
            //IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            //while (true)
            //{
            //    byte[] serverResponseData = client.Receive(ref remoteEndPoint);
            //    //Debug.Log("[Server] Recieved response from server");

            //    ServerResponse serverResponse = ServerResponse.Parser.ParseFrom(serverResponseData);

            //    MainThreadDispatch.RunOnMainThread(() => {
            //        ServerResponseHandler.handleResponse(serverResponse);
            //    });
            //}

            while (true)
            {
                try
                {
                    Debug.Log("[Server Listener] Reading 4 bytes");
                    byte[] lengthBuffer = new byte[4];
                    int read = stream.Read(lengthBuffer, 0, 4);

                    if (read == 0)
                    {
                        Debug.Log("[Server Listener] Disconnected from server");
                        break;
                    }

                    if (read < 4)
                    {
                        Debug.LogError("[Server Listener] Failed to read message length");
                    }

                    Array.Reverse(lengthBuffer);
                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                    if (messageLength <= 0)
                    {
                        Debug.LogError("[Server Listener] Invalid Message length");
                    }

                    byte[] messageBuffer = new byte[messageLength];
                    int totalRead = 0;
                    while (totalRead < messageLength)
                    {
                        int bytesRead = stream.Read(messageBuffer, totalRead, messageLength - totalRead);
                        if (bytesRead == 0)
                        {
                            Debug.LogError("[Server Listener] Disconnected from server while reading message");
                            break;
                        }
                        totalRead += bytesRead;
                    }

                    if (totalRead == messageLength)
                    {
                        ServerResponse serverResponse = ServerResponse.Parser.ParseFrom(messageBuffer);
                        Debug.Log("[Server Listener] Received response from server");

                        MainThreadDispatch.RunOnMainThread(() =>
                        {
                            ServerResponseHandler.handleResponse(serverResponse);
                        });
                    } else
                    {
                        Debug.Log("[Server Listener] Total Read is not equal to messageLength");
                    }

                } catch(Exception e)
                {
                    Debug.LogError("[Server Listener] " + e);
                }
            }
        } else {
            Debug.LogError("[Server Listener] Network Stream is null");
        }
    }
}
