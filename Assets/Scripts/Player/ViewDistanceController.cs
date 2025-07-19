using UnityEngine;

public class ViewDistanceController : MonoBehaviour
{
    public Camera cam;
    public int viewDistance;

    public PlayerView playerView;

    private void Start()
    {
        playerView.viewDistance = viewDistance;
        cam.orthographicSize = viewDistance * 0.5f;
    }
}
