using UnityEngine;
using Google.Protobuf;
using DnD.Player;
using DnD.Service;
using System;

public class ClientRequestHandler
{
    public static void sendRequest(ClientRequest request)
    {
        byte[] data = request.ToByteArray();
        byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

        ServerConnectivityInstance.service.netWorkStream.Write(lengthPrefix, 0, 4);
        ServerConnectivityInstance.service.netWorkStream.Write(data, 0, data.Length);
        ServerConnectivityInstance.service.netWorkStream.Flush();

        Debug.Log("[Client Request Handler] Request sent to server");
    }

    public static void getTerrainData(float posX, float posY, int viewDistance)
    {
        Debug.Log("[Client Request Handler] Getting terrain Data");
        ClientRequest request = new ClientRequest();
        request.ReqType = ClientRequestType.TileGeneration;

        Player player = ServerConnectivityInstance.service.localGameCLient.Player;
        Player playerData = new Player();
        playerData.PosX = posX;
        playerData.PosY = posY;

        playerData.TerrainData = player.TerrainData;

        Client client = ServerConnectivityInstance.service.localGameCLient;
        client.Player = playerData;

        byte[] clientData = client.ToByteArray();
        request.RequestData = ByteString.CopyFrom(clientData);

        sendRequest(request);
    }

    public static void updatePlayerData(float posX, float posY)
    {
        Debug.Log("[Client Request Handler] Updating player Data");
        ClientRequest request = new ClientRequest();
        request.ReqType = ClientRequestType.ClientUpdate;

        Player player = ServerConnectivityInstance.service.localGameCLient.Player;
        player.PosX = posX;
        player.PosY = posY;

        Client client = ServerConnectivityInstance.service.localGameCLient;
        client.Player = player;

        byte[] clientData = client.ToByteArray();
        request.RequestData = ByteString.CopyFrom(clientData);

        sendRequest(request);
    }

    public static void getNPCInstance()
    {
        Debug.Log("[Client Request Handler] Getting NPC Data");
        ClientRequest request = new ClientRequest();
        request.ReqType = ClientRequestType.NpcInstance;

        Client client = ServerConnectivityInstance.service.localGameCLient;

        byte[] clientData = client.ToByteArray();
        request.RequestData = ByteString.CopyFrom(clientData);

        sendRequest(request);
    }
}
