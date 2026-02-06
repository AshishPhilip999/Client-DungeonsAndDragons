using UnityEngine;
using Dnd.Terrain;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;

public class WorldData
{
    public static List<Dnd.Terrain.Terrain> terrainData = new List<Dnd.Terrain.Terrain>();

    public static Dictionary<float, Dictionary<float, Dnd.Terrain.Terrain>> worldTerrainXMap = new Dictionary<float, Dictionary<float, Dnd.Terrain.Terrain>>();

    public static Dictionary<string, Tile> worldTileData = new Dictionary<string, Tile>();

    public static bool tilesPopulated = false;

    public static Dnd.Terrain.Tile getTile(float posX, float posY)
    {
        float[] terrainPos = PlayerView.GetCurrentTerrainPos(posX, posY);
        Dictionary<float, Dnd.Terrain.Terrain> terrainYMap;
        if (worldTerrainXMap.TryGetValue(terrainPos[0], out terrainYMap))
        {
            Debug.Log("Found in X: ");
            Dnd.Terrain.Terrain existinTerrain;
            if (terrainYMap.TryGetValue(terrainPos[1], out existinTerrain))
            {
                int posKey = PlayerView.getVector2IntKey(posX, posY);
                Debug.Log("posKey: " + posKey);
                return existinTerrain.TilePosDataMap[posKey];
            }
        }

        Debug.LogError("[WorldData::getTile] Could not find terrain data for posX:" + posX + ", posY:" + posY);
        return null;
    }

    public static void addToTerrainDataNew(Dnd.Terrain.Terrain terrain)
    {
        Dictionary<float, Dnd.Terrain.Terrain> terrainYMap;
        if (worldTerrainXMap.TryGetValue(terrain.PosX, out terrainYMap))
        {
            Dnd.Terrain.Terrain existinTerrain;
            if (!terrainYMap.TryGetValue(terrain.PosY, out existinTerrain))
            {
                Debug.LogWarning("[WorldData::addToTerrainDataNew] Inserted Y");
                terrainYMap[terrain.PosY] = terrain;
                worldTerrainXMap[terrain.PosX] = terrainYMap;

            } else
            {
                Debug.LogError("[WorldData::addToTerrainDataNew] Duplicate insert of world data");
                return;
            }
        } else
        {
            Debug.LogWarning("[WorldData::addToTerrainDataNew] Inserted X and Y");
            terrainYMap = new Dictionary<float, Dnd.Terrain.Terrain>();
            terrainYMap[terrain.PosY] = terrain;
            worldTerrainXMap[terrain.PosX] = terrainYMap;
        }

        Debug.LogWarning("[World Data] Terrain Added. x:" + terrain.PosX + ", y:" + terrain.PosY);
    }

    public static void addToTerrainData(Dnd.Terrain.Terrain terrain)
    {
        if (terrainExists(terrain)) { Debug.Log("[World Data] Terrain already exists"); PlayerMovement.isMoving = true; return; }

        //popFarthestTerrain();

        terrainData.Add(terrain);
        Debug.LogWarning("[World Data] Terrain Added. x:" + terrain.PosX + ", y:" + terrain.PosY);
        List<Tile> tilesdata = terrain.TileData.ToList();

        foreach (Tile tile in tilesdata)
        {
            Vector3 tilePos = new Vector3(tile.PosX, tile.PosY, 0);
            string hashCode = PlayerView.getVector3HashCode(tilePos);
            Debug.Log("[WorldData:: addToTerrainData] tile variant index: " + tile.Variant);

            worldTileData[hashCode] = tile;
        }
        tilesPopulated = true;
        PlayerMovement.isMoving = true;
        Debug.Log("{" + tilesdata.First().PosX + "," + tilesdata.First().PosY + "}" + ":" + "{" + tilesdata.Last().PosX + "," + tilesdata.Last().PosY + "}");
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
}
