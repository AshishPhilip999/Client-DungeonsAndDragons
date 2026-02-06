using UnityEngine;

public class NPCMover : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 targetPos;
    private float moveTime;
    private float elapsedTime;
    private bool moving = false;

    public void SetTarget(Vector3 target, float duration)
    {
        startPos = transform.position;
        targetPos = target;
        moveTime = duration;
        elapsedTime = 0f;
        moving = true;
    }

    private void Update()
    {
        if (!moving) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / moveTime);

        transform.position = Vector3.Lerp(startPos, targetPos, t);

        if (t >= 1f)
            moving = false;
    }
}
