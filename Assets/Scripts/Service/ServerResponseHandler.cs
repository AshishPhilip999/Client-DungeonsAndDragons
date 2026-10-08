using UnityEngine;
using DnD.Service;
using DnD.NPCs;
using DnD.Player;
using Google.Protobuf;
using Dnd.Terrain;
using System.Linq;
using System.Collections.Generic;

public class ServerResponseHandler
{
    public PlayerViewHandler viewHandler;
    private ServiceHandler serviceHandler;

    public ServerResponseHandler()
    {
        this.serviceHandler = new ServiceHandler();
    }

    public void handleResponse(ServerResponse response)
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
                    //ClientRequestHandler.getTerrainData(player.position.x, player.position.y, viewDistanceController.viewDistance);
                });
                break;

            case ServerResponseType.TileGenerationResponse:
                Debug.Log("[ServerResponseHandler:: handleResponse -> TielGenerationResponse] Received tile data.");
                DnD.Terrain.TerrainList terrainList = DnD.Terrain.TerrainList.Parser.ParseFrom(response.ResponseData);
                List<Dnd.Terrain.Terrain> terrains = terrainList.Terrains.ToList();

                handleTerrainGenerationResponse(terrains);
                break;

            case ServerResponseType.ClientJoined:
                MainThreadDispatch.RunOnMainThread(() =>
                {
                    Client client = DnD.Service.Client.Parser.ParseFrom(response.ResponseData);
                    Debug.Log("[ServerResponseHandler:: handleResponse] Received Client joined update");
                    this.serviceHandler.joinClient(client);
                });
                return;

            case ServerResponseType.ClientUpdateResponse:
                MainThreadDispatch.RunOnMainThread(() =>
                {
                    Client client = Client.Parser.ParseFrom(response.ClientContext.ClientData[1]);
                    Debug.Log("[ServerResponseHandler:: handleResponse] Received Client update");
                });
                return;

            case ServerResponseType.TileFetchResponse:
                Debug.Log("[ServerResponseHandler:: handleResponse -> TielGenerationResponse] Received tile data.");
                Tiles tiles = Tiles.Parser.ParseFrom(response.ResponseData);
                handleTileFetchResponse(tiles);
                MainThreadDispatch.RunOnMainThread(() =>
                {
                    viewHandler.createTiles(tilesToVectorInt(tiles));
                    viewHandler.finishMissedTiles();
                });
                return;
        }
    }

    private List<Vector2Int> tilesToVectorInt(Tiles tiles)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach(Tile tile in tiles.Tiles_)
        {
            Vector2Int v = new Vector2Int();
            v.x = (int)tile.PosX;
            v.y = (int)tile.PosY;
            result.Add(v);
        }

        return result;
    }

    private void handleTileFetchResponse(Tiles tiles)
    {
        List<Tile> tilesList = tiles.Tiles_.ToList();

        foreach(Tile tile in tilesList)
        {
            WorldData.addTileToWolrd(tile);
            Debug.Log("[ServerResponseHandler:: handleTerrainGenerationResponse] Added Tile: posX: " + tile.PosX + ", posY: " + tile.PosY);
        }
    }

    private void handleTerrainGenerationResponse(List<Dnd.Terrain.Terrain> terrains)
    {
        DnD.Player.TerrainData localPlayerTerrainData = ServerConnectivityInstance.service.localGameCLient.Player.TerrainData;
        if(localPlayerTerrainData == null)
        {
            Debug.LogError("Null");
        }
        PlayerMovement.isMoving = false;
        foreach (Dnd.Terrain.Terrain terrain in terrains)
        {
            Debug.LogWarning("[Server Response Handler] Getting terrain at " + "x:" + terrain.PosX + ", y:" + terrain.PosY);
            WorldData.addToTerrainDataNew(terrain);
            //WorldData.addToTerrainData(terrain);

            localPlayerTerrainData.ExistingTerrainPositions.Add(terrain.PosX);
            localPlayerTerrainData.ExistingTerrainPositions.Add(terrain.PosY);

            Debug.Log("[ServerResponseHandler:: handleTerrainGenerationResponse] Added Terrain: posX: " + terrain.PosX + ", posY: " + terrain.PosY);
        }
        WorldData.tilesPopulated = true;
        PlayerMovement.isMoving = true;
    }
}
