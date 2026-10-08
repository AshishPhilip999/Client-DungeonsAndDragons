using UnityEngine;
using System;

public class GameEditor : MonoBehaviour
{
    public static void createGameObject(GameObject gameObject, Vector2 position)
    {
        MainThreadDispatch.RunOnMainThread(() => { Instantiate(gameObject, position, Quaternion.identity); });
    }
}
