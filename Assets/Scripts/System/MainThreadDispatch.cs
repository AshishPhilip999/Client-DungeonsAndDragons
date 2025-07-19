using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatch : MonoBehaviour
{
    private static readonly Queue<Action> actions = new Queue<Action>();

    public static void RunOnMainThread(Action action)
    {
        lock (actions)
        {
            actions.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (actions)
        {
            while (actions.Count > 0)
            {
                actions.Dequeue().Invoke();
            }
        }
    }
}
