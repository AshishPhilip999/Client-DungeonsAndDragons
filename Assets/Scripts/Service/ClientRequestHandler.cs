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
        request.RequestType = ClientRequestType.TileGenerationRequest;

        Player player = ServerConnectivityInstance.service.localGameCLient.Player;
        Player playerData = new Player();
        playerData.PosX = posX;
        playerData.PosY = posY;
        playerData.CurrentTerrainPosX = player.CurrentTerrainPosX;
        playerData.CurrentTerrainPosY = player.CurrentTerrainPosY;

        foreach(Dnd.Terrain.Terrain terrain in WorldData.terrainData)
        {
            Dnd.Terrain.Terrain currTerrain = new Dnd.Terrain.Terrain();
            currTerrain.PosX = terrain.PosX;
            currTerrain.PosY = terrain.PosY;

            playerData.TerrainData.Add(currTerrain);
        }

        Client client = ServerConnectivityInstance.service.localGameCLient;
        client.Player = playerData;

        byte[] clientData = client.ToByteArray();
        request.RequestData = ByteString.CopyFrom(clientData);

        sendRequest(request);
    }
}
