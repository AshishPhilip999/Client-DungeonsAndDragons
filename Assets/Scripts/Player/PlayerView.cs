using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System.Linq;
using Dnd.Terrain;
using System.Reflection;

public class PlayerView : MonoBehaviour
{
    public Transform playerPosition;
    public int viewDistance;
    public GameObject tile;
    public float tileSize;

    public int direction = 1; // Should make it a set of only -1 and 1

    public List<GameObject> tiles;

    public List<GameObject> standardTreeVariants;
    public List<GameObject> standardGrassVariants;
    public List<GameObject> rockVariants;
    public List<GameObject> woodenCabinVariants;

    private Vector3[,] playerViewPosData;

    public int viewLengthX;
    public int viewLengthY;

    private Dictionary<string, List<GameObject>> viewPositionObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        viewPositionObjects = new Dictionary<string, List<GameObject>>();
        viewLengthX = (int)(((viewDistance / tileSize) + 1) * 2) - 1;
        viewLengthY = (int)(((viewDistance / tileSize) + 1) * 2) - 1;
        playerViewPosData = new Vector3[viewLengthX, viewLengthY];

        playerViewPosData = populateViewData(transform.position);
        generateTiles(this.playerViewPosData);
    }

    public void updateTiles(Vector3 playerPosition, int direction)
    {
        this.direction = direction;
        Vector3[,] newViewData = populateViewData(playerPosition);
        if (isSameViewData(newViewData, playerViewPosData))
        {
            Debug.Log("[Player View] Same view data generated");
            return;
        }
        List<Vector3> newTiles = getDifference(newViewData, playerViewPosData);
        List<Vector3> oldTiles = getDifference(playerViewPosData, newViewData);

        playerViewPosData = newViewData;

        createTiles(newTiles);
        removeTiles(oldTiles);
    }

    private static bool isSameViewData(Vector3[,] a, Vector3[,] b)
    {
        if (a == null || b == null)
            return false;

        if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
            return false;

        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                if (a[i, j] != b[i, j])
                    return false;
            }
        }

        return true;
    }

    private Vector3[,] populateViewData(Vector3 playerPosition)
    {
        Vector3[,] viewPositionData = new Vector3[viewLengthX, viewLengthY];
        float[] roundOfPosition = roundOfPlayerPosition(playerPosition);
        //Debug.Log(roundOfPosition[0] + "," + roundOfPosition[1]);
        float currXPos = roundOfPosition[0] - viewDistance;
        float currYPos = roundOfPosition[1] + viewDistance;

        for(int i = 0; i < viewPositionData.GetLength(0); i++)
        {
            for(int j = 0; j < viewPositionData.GetLength(1); j++)
            {
                viewPositionData[i, j] = new Vector3(currXPos, currYPos, 0);
                currXPos += tileSize;
            }
            currYPos -= tileSize;
            currXPos = roundOfPosition[0] - viewDistance;
        }
        return viewPositionData;
    }

    private float[] roundOfPlayerPosition(Vector3 playerPosition)
    {
        float roundOfX = -1;
        float roundOfY = -1;

        float posX = playerPosition.x;
        float posY = playerPosition.y;

        bool isXNeg = (posX < 0) ? true : false;
        bool isYNeg = (posY < 0) ? true : false;

        float absPosX = Mathf.Abs(posX);
        float absPosY = Mathf.Abs(posY);

        int currRoundOfX = Mathf.Abs((int)posX);
        int currRoundOfY = Mathf.Abs((int)posY);

        for(float i = currRoundOfX; i <= currRoundOfX + 1; i += tileSize)
        {
            if(i > absPosX)
            {
                float currRoundOfXLength = i - absPosX;

                float prevRoundOfX = i - tileSize;

                float prevRoundOfXLength = absPosX - prevRoundOfX;

                if (currRoundOfXLength < prevRoundOfXLength)
                {
                    roundOfX = i;
                } else
                {
                    roundOfX = prevRoundOfX;
                }
                break;

            } else if (i == absPosX)
            {
                roundOfX = i;
                break;
            }
        }

        for (float i = currRoundOfY; i <= currRoundOfY + 1; i += tileSize)
        {
            if (i > absPosY)
            {
                float currRoundOfYLength = i - absPosY;

                float prevRoundOfY = i - tileSize;

                float prevRoundOfYLength = absPosY - prevRoundOfY;

                if (currRoundOfYLength < prevRoundOfYLength)
                {
                    roundOfY = i;
                }
                else
                {
                    roundOfY = prevRoundOfY;
                }
                break;

            }
            else if (i == absPosY)
            {
                roundOfY = i;
                break;
            }
        }

        if (roundOfX == -1 || roundOfY == -1) { Debug.LogError("[Player View] Error calculating round of Values"); }

        if (isXNeg) { roundOfX *= -1; }
        if (isYNeg) { roundOfY *= -1; }

        return new float[] { roundOfX, roundOfY };
    }

    public static List<Vector3> getDifference(Vector3[,] arr1, Vector3[,] arr2)
    {
        List<Vector3> arr1Flat = flattern(arr1);
        List<Vector3> arr2Flat = flattern(arr2);

        return arr1Flat.Where(v1 => !arr2Flat.Any(v2 => v1 == v2)).ToList();
    }

    private static List<Vector3> flattern(Vector3[,] arr)
    {
        var result = new List<Vector3>();
        foreach (var item in arr)
            result.Add(item);
        return result;
    }

    public int topOrderInLayer = 0;
    public int bottomOrderInLayer = 0;
    private void createTiles(List<Vector3> newTiles)
    {
        int currentApplyLayer = 0;
        if (direction == -1 )
        {
            topOrderInLayer--;
            bottomOrderInLayer--;
            currentApplyLayer = topOrderInLayer;
        } else if (direction == 1)
        {
            topOrderInLayer++;
            bottomOrderInLayer++;
            currentApplyLayer = bottomOrderInLayer;
        }

        foreach(Vector3 tileData in newTiles)
        {
            List<GameObject> tileObjects = new List<GameObject>();

            string hashCode = getVector3HashCode(tileData);
            Tile tile = WorldData.worldTileData[hashCode];
            int variantIndex = tile.Variant;

            GameObject instanceTile = getTileFromTileType(tile.Type, variantIndex);
            GameObject newTile = Instantiate(instanceTile, new Vector3(tileData.x, tileData.y, 0), Quaternion.identity);
            newTile.transform.position += new Vector3(tile.TileOffSetX, tile.TileOffSetY, 0);

            newTile.isStatic = true;
            newTile.GetComponent<SpriteRenderer>().sortingOrder = getSortingOrderByPosition(tileData.y);
            TileSpawnPositioner tileSpawnPositioner = newTile.GetComponent<TileSpawnPositioner>();
            if (tileSpawnPositioner != null)
            {
                for (int index = 0; index < tile.SpawnPositionIndicies.Count; index++)
                {
                    int spawnObjectIndex = tile.SpawnPositionObjects[index];
                    int spawnPositionIndex = tile.SpawnPositionIndicies[index];
                    GameObject currSpawnObject = Instantiate(tileSpawnPositioner.spawnPositionObjects[spawnObjectIndex],
                        tileSpawnPositioner.spawnPositions[spawnPositionIndex].transform.position, Quaternion.identity);
                    currSpawnObject.GetComponent<SpriteRenderer>().sortingOrder = newTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
                    tileSpawnPositioner.objectsSpawned.Add(currSpawnObject);
                    currSpawnObject.transform.SetParent(newTile.transform, true);
                }
            }

            if (tile.Type != TileType.LightPatchGrass )
            {
                GameObject grass = getTileFromTileType(TileType.LightPatchGrass, 0);
                GameObject instanceGrass = Instantiate(grass, new Vector3(tileData.x, tileData.y, 0), Quaternion.identity);
                instanceGrass.isStatic = true;
                instanceGrass.GetComponent<SpriteRenderer>().sortingOrder = getSortingOrderByPosition(tileData.y);
                tileObjects.Add(instanceGrass);
            }

            tileObjects.Add(newTile);

            addToViewPositionObjects(tileData, tileObjects);            
        }
    }

    private void removeTiles(List<Vector3> oldTiles)
    {
        foreach (Vector3 tileData in oldTiles)
        {
            foreach(GameObject obj in getObjectFromViewPositionObjectsMap(tileData))
            {
                Destroy(obj);
            }
            removeFromViewPositionObjects(tileData);
        }
    }

    private void generateTiles(Vector3[,] viewData)
    {
        for(int i = 0; i < viewLengthY; i++)
        {
            for(int j = 0; j < viewLengthX; j++)
            {
                List<GameObject> tileObjects = new List<GameObject>();

                string hashCode = getVector3HashCode(viewData[i, j]);
                Tile tileData = WorldData.worldTileData[hashCode];
                int variantIndex = tileData.Variant;

                GameObject instanceTile = getTileFromTileType(tileData.Type, variantIndex);

                Debug.Log(viewData[i, j].x + "," + viewData[i, j].y);
                GameObject currTile = Instantiate(instanceTile, viewData[i,j] , Quaternion.identity);
                currTile.transform.position += new Vector3(tileData.TileOffSetX, tileData.TileOffSetY, 0);

                TileSpawnPositioner tileSpawnPositioner = currTile.GetComponent<TileSpawnPositioner>();
                currTile.isStatic = true;
                currTile.GetComponent<SpriteRenderer>().sortingOrder = getSortingOrderByPosition(viewData[i, j].y);
                if (tileSpawnPositioner != null)
                {
                    for (int index = 0; index < tileData.SpawnPositionIndicies.Count; index++)
                    {
                        int spawnObjectIndex = tileData.SpawnPositionObjects[index];
                        int spawnPositionIndex = tileData.SpawnPositionIndicies[index];
                        GameObject currSpawnObject = Instantiate(tileSpawnPositioner.spawnPositionObjects[spawnObjectIndex],
                            tileSpawnPositioner.spawnPositions[spawnPositionIndex].transform.position, Quaternion.identity);
                        currSpawnObject.GetComponent<SpriteRenderer>().sortingOrder = currTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
                        tileSpawnPositioner.objectsSpawned.Add(currSpawnObject);
                        currSpawnObject.transform.SetParent(currTile.transform, true);
                    }
                }

                if (tileData.Type != TileType.LightPatchGrass)
                {
                    GameObject grass = getTileFromTileType(TileType.LightPatchGrass, 0);
                    GameObject instanceGrass = Instantiate(grass, viewData[i, j], Quaternion.identity);
                    instanceGrass.isStatic = true;
                    instanceGrass.GetComponent<SpriteRenderer>().sortingOrder = getSortingOrderByPosition(viewData[i, j].y); ;
                    tileObjects.Add(instanceGrass);
                }

                tileObjects.Add(currTile);

                addToViewPositionObjects(viewData[i, j], tileObjects);
            }
            bottomOrderInLayer++;
        }
    }

    private int getSortingOrderByPosition(float yPos)
    {
        yPos *= -1;
        return (int)(yPos / tileSize);
    }

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
                return null;
        }
    }

    private List<GameObject> getObjectFromViewPositionObjectsMap(Vector3 position)
    {
        string hashCode = getVector3HashCode(position);
        if (this.viewPositionObjects.TryGetValue(hashCode, out var obj)) {
            return obj;
        }

        Debug.LogError("[Player View] Object was not found. Maybe trying to delete same object twice.");
        return null;
    }

    private void addToViewPositionObjects(Vector3 position, List<GameObject> tile)
    {
        string hashCode = getVector3HashCode(position);
        this.viewPositionObjects[hashCode] = tile;
    }

    private void removeFromViewPositionObjects(Vector3 position)
    {
        string hashCode = getVector3HashCode(position);
        this.viewPositionObjects.Remove(hashCode);
    }

    public static string getVector3HashCode(Vector3 v)
    {
        return $"{v.x:F4},{v.y:F4},{v.z:F4}";
    }
}
