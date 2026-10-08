using UnityEngine;

public class SetLayerOrder : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Transform parent;
    public int initialOrder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = this.GetComponent<SpriteRenderer>();
        initialOrder = renderer.sortingOrder;
    }

    // Update is called once per frame
    void Update()
    {
        renderer.sortingOrder = initialOrder + (RenderLayer.calculateSortinOrder(parent.position.y) * 10);
    }
}
