using UnityEngine;
using System.Collections;

public class ObjectTransformer : MonoBehaviour
{
    public PlayerAnimation playerAnimation;

    public void moveAlongX(float value, bool isFlipped)
    {
        Vector3 moveX = new Vector3(value * Time.deltaTime, 0, 0);
        transform.position += moveX;
        walk(isFlipped);
    }

    public void walk(bool isFlipped)
    {
        playerAnimation.walk(isFlipped);
    }

    public void idle()
    {
        playerAnimation.idle();
    }
    public void moveAlongY(float value, bool isFlipped)
    {
        Vector3 moveY = new Vector3(0, value * Time.deltaTime, 0);
        transform.position += moveY;
        walk(isFlipped);
    }
}
