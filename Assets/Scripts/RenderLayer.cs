using UnityEngine;

public class RenderLayer : MonoBehaviour
{
    public GameObject parent;
    public SpriteRenderer renderer;
    public GameObject pivot;

    public int initialSortingOrder;

    private void Start()
    {
        renderer = parent.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = (calculateSortinOrder(pivot.transform.position.y) * 10) + 6;
        initialSortingOrder = renderer.sortingOrder;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            renderer.sortingOrder = initialSortingOrder;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Vector2 playerPos = collision.gameObject.transform.position;

            float distance = pivot.transform.position.y - playerPos.y;
            //Debug.Log(distance);

            if (distance > 0)
            {
                renderer.sortingOrder = initialSortingOrder - 7;
            } else
            {
                renderer.sortingOrder = initialSortingOrder;
            }
        }
    }

    public static int calculateSortinOrder(float posY)
    {
        int floor = Mathf.RoundToInt(posY);
        //int additive = floor * 10;

        //return additive;
        return -floor;
    }
}
