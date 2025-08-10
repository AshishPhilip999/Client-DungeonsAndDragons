using UnityEngine;
using Dnd.Terrain;
using DnD.Terrain;
using System.Collections.Generic;

public class TerrainHandler: MonoBehaviour
{
    public static List<GameObject> tiles;
    public static void deleteTile(Tile tile)
    {
        Vector3 tilePos = new Vector3(tile.PosX, tile.PosY, 0);
        string hashCode = PlayerView.getVector3HashCode(tilePos);

        tile.Type = TileType.StandardGrass;

        List<GameObject> deletedObject = null;
        if (PlayerView.viewPositionObjects.TryGetValue(hashCode, out deletedObject))
        {
            if (deletedObject != null)
            {
                GameObject tileItem = deletedObject[0];
                Destroy(tileItem);
                deletedObject.RemoveAt(0);
                Debug.Log("[Terrain Handler: deleteTile] Tile deleted at x: " + tile.PosX + ", y:" + tile.PosY);
            }
        }
    }
}
