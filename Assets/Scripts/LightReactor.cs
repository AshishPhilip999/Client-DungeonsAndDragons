using UnityEngine;

public class LightReactor : MonoBehaviour
{
    public GameObject player;
    SpriteRenderer thisRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisRenderer = gameObject.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float nonModDistance = nonModularDistance(transform.position, player.transform.position);
        Debug.Log("[LightReactor] Non Mod Distance: " + nonModDistance);
        if (nonModDistance < -0.5f)
        {
            thisRenderer.sortingLayerID = SortingLayer.NameToID("Opaque");
        } else
        {
            thisRenderer.sortingLayerID = SortingLayer.NameToID("Render 2");
        }
    }

    public float nonModularDistance(Vector3 origin, Vector3 target)
    {
        // Difference in Y
        float deltaY = target.y - origin.y;

        // If target is above origin → make it negative
        if (deltaY > 0)
            return -Mathf.Abs(deltaY);

        // If target is below origin → make it positive
        else if (deltaY < 0)
            return Mathf.Abs(deltaY);

        // Same level
        return 0f;
    }
}
