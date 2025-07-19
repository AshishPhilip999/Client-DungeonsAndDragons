using UnityEngine;
using Google.Protobuf;
using DnD.Renderer;

public class TerrainGenerator : MonoBehaviour
{
    public  GameObject tile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //DnD.Renderer.Color colorData = new DnD.Renderer.Color();
        //colorData.Red = 1.0f;
        //colorData.Blue = 1.0f;
        //colorData.Green = 1.0f;

        //byte[] data = colorData.ToByteArray();
        //SpriteRenderer tileRender = tile.GetComponent<SpriteRenderer>();
        //tileRender.color = new UnityEngine.Color(1f, 0f, 0f);

        //Instantiate(tile, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void createTerrain(DnD.Renderer.Color color)
    {
        SpriteRenderer tileRender = tile.GetComponent<SpriteRenderer>();
        tileRender.color = new UnityEngine.Color(color.Red, color.Green, color.Blue, color.Alpha);

        Instantiate(tile, new Vector3(2, -1, 0), Quaternion.identity);
    }
}
