using UnityEngine;

public class LampController : MonoBehaviour
{
    public GameObject lamp;

    // Update is called once per frame
    void Update()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Find direction from object to mouse
        Vector2 direction = (mousePos - lamp.transform.position);

        // Get the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation (only around Z axis for 2D)
        lamp.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //Debug.Log("[LampController] Angle: " + direction);
    }
}
