using UnityEngine;
using DnD.Service;
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
                Debug.Log("[Server Response Handler] Connection Success");

                Transform player = ServerConnectivityInstance.player;
                ViewDistanceController viewDistanceController = player.GetComponent<ViewDistanceController>();
                ClientRequestHandler.getTerrainData(player.position.x, player.position.y, viewDistanceController.viewDistance);
                break;

            case ServerResponseType.TileGenerationResponse:
                DnD.Terrain.TerrainList terrainList = DnD.Terrain.TerrainList.Parser.ParseFrom(response.ResponseData);
                List<Dnd.Terrain.Terrain> terrains = terrainList.Terrains.ToList();

                handleTerrainGenerationResponse(terrains);
                break;
        }
    }

    private static void handleTerrainGenerationResponse(List<Dnd.Terrain.Terrain> terrains)
    {
        foreach(Dnd.Terrain.Terrain terrain in terrains)
        {
            Debug.LogWarning("[Server Response Handler] Getting terrain at " + "x:" + terrain.PosX + ", y:" + terrain.PosY);
            WorldData.addToTerrainData(terrain);
        }
    }
}
