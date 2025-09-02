using UnityEngine;
using DnD.Service;
using DnD.Player;
using Google.Protobuf;
using System.Linq;
using System.Collections.Generic;

public class ServerResponseHandler
{
    public static void handleResponse(ServerResponse response)
    {
        ServerResponseType responseType = response.Response;

        switch(responseType)
        {
            case ServerResponseType.ConnectionSuccess:
                MainThreadDispatch.RunOnMainThread(() =>
                {
                    Debug.Log("[ServerResponseHandler] Connection Success");

                    Transform player = ServerConnectivityInstance.player;
                    ViewDistanceController viewDistanceController = player.GetComponent<ViewDistanceController>();
                    ClientRequestHandler.getTerrainData(player.position.x, player.position.y, viewDistanceController.viewDistance);
                });
                break;

            case ServerResponseType.TileGenerationResponse:
                Debug.Log("[ServerResponseHandler:: handleResponse -> TielGenerationResponse] Received tile data.");
                DnD.Terrain.TerrainList terrainList = DnD.Terrain.TerrainList.Parser.ParseFrom(response.ResponseData);
                List<Dnd.Terrain.Terrain> terrains = terrainList.Terrains.ToList();

                handleTerrainGenerationResponse(terrains);
                break;

            case ServerResponseType.PlayerUpdate:
                MainThreadDispatch.RunOnMainThread(() =>
                {
                    Client client = DnD.Service.Client.Parser.ParseFrom(response.ResponseData);
                    ServerConnectivityInstance.clientsHandler.updatePlayerData(client);
                });
                return;
        }
    }

    private static void handleTerrainGenerationResponse(List<Dnd.Terrain.Terrain> terrains)
    {
        DnD.Player.TerrainData localPlayerTerrainData = ServerConnectivityInstance.service.localGameCLient.Player.TerrainData;
        if(localPlayerTerrainData == null)
        {
            Debug.LogError("Null");
        }
        foreach (Dnd.Terrain.Terrain terrain in terrains)
        {
            Debug.LogWarning("[Server Response Handler] Getting terrain at " + "x:" + terrain.PosX + ", y:" + terrain.PosY);
            WorldData.addToTerrainData(terrain);

            localPlayerTerrainData.ExistingTerrainPositions.Add(terrain.PosX);
            localPlayerTerrainData.ExistingTerrainPositions.Add(terrain.PosY);

            Debug.Log("[ServerResponseHandler:: handleTerrainGenerationResponse] Added Terrain: posX: " + terrain.PosX + ", posY: " + terrain.PosY);
        }
        WorldData.tilesPopulated = true;
    }
}
