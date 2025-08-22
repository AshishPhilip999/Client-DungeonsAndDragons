using UnityEngine;
using Google.Protobuf;
using DnD.Player;
using DnD.Service;
using DnD.Terrain;
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

        //Debug.Log("[Client Request Handler] Request sent to server");
    }

    public static void getTerrainData(float posX, float posY, int viewDistance)
    {
        //Debug.Log("[Client Request Handler] Getting terrain Data");
        ClientRequest request = new ClientRequest();
        request.ReqType = ClientRequestType.TileGeneration;

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

        request.Client = client;

        byte[] clientData = client.ToByteArray();
        request.RequestData = ByteString.CopyFrom(clientData);

        sendRequest(request);
    }

    public static void updatePlayerData(float posX, float posY)
    {
        //Debug.Log("[Client Request Handler] Updating player Data");
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

    public static void updateTileItemData(TileItem tileItem)
    {
        ClientRequest request = new ClientRequest();
        request.ReqType = ClientRequestType.TileItemUpdateRequest;

        TileItemData tileItemData = new TileItemData();

        tileItemData.Type = TileItemDataType.Delete;

        tileItemData.PosX = tileItem.tilePosX;
        tileItemData.PosY = tileItem.tilePosY;
        tileItemData.TerrainPosX = tileItem.terrainPosX;
        tileItemData.TerrainPosY = tileItem.terrainPosY;

        byte[] tileItemDataBytes = tileItemData.ToByteArray();

        request.Client = ServerConnectivityInstance.service.localGameCLient;

        request.RequestData = ByteString.CopyFrom(tileItemDataBytes);

        sendRequest(request);
    }
}
