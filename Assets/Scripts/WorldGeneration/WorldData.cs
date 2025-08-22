using UnityEngine;
using Dnd.Terrain;
using DnD.Terrain;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;

public class WorldData
{
    public static List<Dnd.Terrain.Terrain> terrainData = new List<Dnd.Terrain.Terrain>();

    public static Dictionary<string, Tile> worldTileData = new Dictionary<string, Tile>();

    public static bool tilesPopulated = false;

    public static void addToTerrainData(Dnd.Terrain.Terrain terrain)
    {
        if (terrainExists(terrain)) { PlayerMovement.isMoving = true; return; }

        MainThreadDispatch.RunOnMainThread(() => { popFarthestTerrain(); });

        terrainData.Add(terrain);
        Debug.Log(":: [World Data] Terrain Added. x:" + terrain.PosX + ", y:" + terrain.PosY);
        List<Tile> tilesdata = terrain.TileData.ToList();

        foreach (Tile tile in tilesdata)
        {
            Vector3 tilePos = new Vector3(tile.PosX, tile.PosY, 0);
            string hashCode = PlayerView.getVector3HashCode(tilePos);
            Debug.Log("-> [World Data]: tile X:" + tile.PosX + ", Y:" + tile.PosY + " added of terrain X:" + terrain.PosX + ", Y: " + terrain.PosY);

            worldTileData[hashCode] = tile;
        }
        PlayerMovement.isMoving = true;
    }

    public static void popFromTerrainData(Dnd.Terrain.Terrain terrain)
    {
        Dnd.Terrain.Terrain lastTerrain = terrainData.ElementAt(0);
        terrainData.Remove(terrain);
        removeFromWorldTileData(terrain);

        Debug.LogWarning("[World Generator] Poped terrain X: " + lastTerrain.PosX + ", Y:" + lastTerrain.PosY);
    }

    public static void popFarthestTerrain()
    {
        float maxDistance = 750.0f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        var terrainsToRemove = terrainData
    .Where(terrain => Vector2.Distance(player.transform.position, new Vector2(terrain.PosX, terrain.PosY)) > maxDistance)
    .ToList();

        foreach (Dnd.Terrain.Terrain terrain in terrainsToRemove)
        {
            popFromTerrainData(terrain);
        }
    }

    public static void removeFromWorldTileData(Dnd.Terrain.Terrain terrain)
    {
        foreach(Tile tile in terrain.TileData)
        {
            string hashCode = PlayerView.getVector3HashCode(new Vector3(tile.PosX, tile.PosY, 0));
            worldTileData.Remove(hashCode);
        }
    }

    public static bool terrainExists(Dnd.Terrain.Terrain terrain) {
        foreach(Dnd.Terrain.Terrain currTerrain in terrainData)
        {
            //Debug.Log(currTerrain.PosX + ":" + currTerrain.PosY);
            if(currTerrain.PosX == terrain.PosX && currTerrain.PosY == terrain.PosY)
            {
                return true;
            }
        }

        return false;
    }

    public static void handleTileDataUpdate(TileItemData tileItemData)
    {
        TileItemDataType type = tileItemData.Type;

        switch (type)
        {
            case TileItemDataType.Delete:
                Vector3 tilePos = new Vector3(tileItemData.PosX, tileItemData.PosY, 0);
                string hashCode = PlayerView.getVector3HashCode(tilePos);

                Tile currTile = WorldData.worldTileData[hashCode];

                TerrainHandler.deleteTile(currTile);
                break;

            case TileItemDataType.Update:
                
                break;
        }
    }
}
