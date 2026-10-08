using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Dnd.Terrain;

public class PlayerView : MonoBehaviour, PlayerViewHandler
{
    public Transform playerPosition;
    public int viewDistance;

    public GameObject tile;

    public List<GameObject> tiles;
    public List<GameObject> standardTreeVariants;
    public List<GameObject> standardGrassVariants;
    public List<GameObject> rockVariants;
    public List<GameObject> woodenCabinVariants;

    public ViewDistanceController viewDistanceController;

    private Vector2Int[,] playerViewPosData;
    public List<Vector2Int> currentAddTiles = new List<Vector2Int>();

    private Dictionary<Vector2Int, List<GameObject>> viewPositionObjects;

    public int viewLengthX;
    public int viewLengthY;

    public int direction = 1;

    void Start()
    {
        viewPositionObjects = new Dictionary<Vector2Int, List<GameObject>>();

        viewDistance = viewDistanceController.viewDistance;

        viewLengthX = viewDistance * 2;
        viewLengthY = viewDistance * 2;

        playerViewPosData = PopulateViewData(playerPosition.position);
        currentAddTiles = Flatten(playerViewPosData);
        Debug.Log("[PlayerView:: Start] CurrentTileCount: " + currentAddTiles.Count);
        fetchTilesFromServer(playerPosition.position, currentAddTiles);
    }

    // =========================
    // GRID GENERATION (INT BASED)
    // =========================
    private Vector2Int[,] PopulateViewData(Vector3 playerPos)
    {
        int baseX = Mathf.RoundToInt(playerPos.x);
        int baseY = Mathf.RoundToInt(playerPos.y);

        int range = Mathf.CeilToInt(viewDistance);

        Vector2Int[,] data = new Vector2Int[viewLengthX, viewLengthY];

        for (int i = 0; i < viewLengthX; i++)
        {
            for (int j = 0; j < viewLengthY; j++)
            {
                int x = baseX - range + i;
                int y = baseY + range - j;

                data[i, j] = new Vector2Int(x, y);
            }
        }

        return data;
    }

    private static List<Vector2Int> Flatten(Vector2Int[,] arr)
    {
        var result = new List<Vector2Int>();
        foreach (var item in arr)
            result.Add(item);
        return result;
    }

    public static List<Vector2Int> GetDifference(Vector2Int[,] a, Vector2Int[,] b)
    {
        var aFlat = Flatten(a);
        var bFlat = Flatten(b);

        return aFlat.Where(v => !bFlat.Contains(v)).ToList();
    }

    // =========================
    // UPDATE FLOW
    // =========================
    public void fetchAndUpdateTiles(Vector3 playerPos, int direction)
    {
        this.direction = direction;

        var newViewData = PopulateViewData(playerPos);

        var newTiles = GetDifference(newViewData, playerViewPosData);
        var oldTiles = GetDifference(playerViewPosData, newViewData);

        Debug.Log("[PlayerView:: fetchAndUpdateTiles] New required tiles: " + newTiles.Count);

        currentAddTiles = newTiles;

        fetchTilesFromServer(playerPos, newTiles);

        playerViewPosData = newViewData;

        removeTiles(oldTiles);
    }

    // =========================
    // SERVER FETCH
    // =========================
    public void fetchTilesFromServer(Vector3 playerPos, List<Vector2Int> tiles)
    {
        // Your networking call here
         ClientRequestHandler.getTilesData(playerPos.x, playerPos.y, tiles);
    }

    // =========================
    // CREATE TILES (MAIN THREAD)
    // =========================
    private List<Vector2Int> missedTiles = new List<Vector2Int>();
    public void createTiles(List<Vector2Int> currTiles)
    {
        foreach (Vector2Int tilePos in currTiles)
        {
            Debug.Log("[PlayerView:: createTiles] Fetching tile data of tile posX: " + tilePos.x + ", posY: " + tilePos.y);

            Tile tile = WorldData.getTileFromWorld(tilePos.x, tilePos.y);
            if (tile != null)
            {
                createTile(tile, tilePos);
            } else
            {
                Debug.LogError("[PlayerView:: createTiles] Can't find tile of posX: " + tilePos.x + ", posY: " + tilePos.y);
                missedTiles.Add(tilePos);
            }
            
        }
    }

    private void createTile(Tile tile, Vector2Int tilePos)
    {
        List<GameObject> tileObjects = new List<GameObject>();

        GameObject prefab = getTileFromTileType(tile.Type, tile.Variant);

        Vector3 worldPos = new Vector3(tile.PosX, tile.PosY, 0);

        GameObject newTile = Instantiate(prefab, worldPos, Quaternion.identity);
        newTile.transform.position += new Vector3(tile.TileOffSetX, tile.TileOffSetY, 0);
        newTile.isStatic = true;

        TileSpawnPositioner tsp = newTile.GetComponent<TileSpawnPositioner>();

        if (tsp != null)
        {
            for (int i = 0; i < tile.SpawnPositionIndicies.Count; i++)
            {
                int objIndex = tile.SpawnPositionObjects[i];
                int posIndex = tile.SpawnPositionIndicies[i];

                GameObject obj = Instantiate(
                    tsp.spawnPositionObjects[objIndex],
                    tsp.spawnPositions[posIndex].transform.position,
                    Quaternion.identity
                );

                obj.transform.SetParent(newTile.transform, true);
                tsp.objectsSpawned.Add(obj);
            }
        }

        // Add grass layer
        if (tile.Type != TileType.LightPatchGrass)
        {
            GameObject grassPrefab = getTileFromTileType(TileType.LightPatchGrass, 0);
            GameObject grass = Instantiate(grassPrefab, worldPos, Quaternion.identity);
            grass.isStatic = true;

            tileObjects.Add(grass);
        }

        tileObjects.Add(newTile);

        viewPositionObjects[tilePos] = tileObjects;
    }

    public void finishMissedTiles()
    {
        foreach (Vector2Int tile in missedTiles)
        {
            Tile currTile = WorldData.getTileFromWorld(tile.x, tile.y);
            if (tile == null)
            {
                continue;
            }
            createTile(currTile, tile);
        }

    }

    // =========================
    // REMOVE TILES (SAFE)
    // =========================
    private void removeTiles(List<Vector2Int> oldTiles)
    {
        foreach (var tilePos in oldTiles)
        {
            WorldData.removeTileFromWorld(tilePos.x, tilePos.y);

            if (viewPositionObjects.TryGetValue(tilePos, out var objects))
            {
                foreach (var obj in objects)
                    Destroy(obj);

                viewPositionObjects.Remove(tilePos);
            }
            else
            {
                Debug.LogWarning($"[REMOVE MISS] {tilePos}");
            }
        }
        Debug.Log("World size: " + WorldData.getWorldTilesCount);
        Debug.Log("Map size: " + viewPositionObjects.Count);
    }

    // =========================
    // TILE PREFAB SELECTION
    // =========================
    private GameObject getTileFromTileType(TileType tileType, int variantIndex)
    {
        switch (tileType)
        {
            case TileType.StandardGrass:
                return standardGrassVariants[variantIndex];

            case TileType.LightPatchGrass:
                return tiles[1];

            case TileType.DarkPatchGrass:
                return tiles[2];

            case TileType.StandardTree:
                return standardTreeVariants[variantIndex];

            case TileType.Rock:
                return rockVariants[variantIndex];

            case TileType.GiantRock:
                return tiles[5];

            case TileType.WaterBody:
                return tiles[6];

            case TileType.WoodenCabin:
                return woodenCabinVariants[variantIndex];

            default:
                Debug.LogError("[PlayerView:: getTileFromTileType] cannot find tile of type");
                return null;
        }
    }
}