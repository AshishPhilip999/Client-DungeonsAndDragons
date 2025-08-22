using UnityEngine;
using Dnd.Terrain;

public class TileItem : MonoBehaviour, Damageable
{
    public int totalHealth = 0;
    public int health = 0;
    public int armorHealth = 0;

    public float tilePosX;
    public float tilePosY;

    public float terrainPosX;
    public float terrainPosY;

    public int getArmorHealth()
    {
        return this.armorHealth;
    }

    public int getHealth()
    {
        return this.health;
    }

    public int getTotalHealth()
    {
        return this.totalHealth;
    }

    private void Start()
    {
        tilePosX = transform.position.x;
        tilePosY = transform.position.y;
    }

    private void Update()
    {
        if (this.totalHealth <= 0) { this.destroy(); }
    }

    public void damage(int damagePoints)
    {
        this.totalHealth -= damagePoints;
    }

    public void destroy()
    {
        Vector3 tilePos = new Vector3(tilePosX, tilePosY, 0);
        string hashCode = PlayerView.getVector3HashCode(tilePos);

        /// Updating deleted item in client.
        Tile currTile = WorldData.worldTileData[hashCode];
        TerrainHandler.deleteTile(currTile);

        /// Updating deleted item in server.
        ClientRequestHandler.updateTileItemData(this);
    }
}
