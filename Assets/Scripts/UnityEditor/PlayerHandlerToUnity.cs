using UnityEngine;
using System;

/*
 * PlayerHandlerToUnity creates and readies GameObject instances to the unity Editor.
 * This builds appropriate objects to be updated directly to the game's editor.
 */
public class PlayerHandlerToUnity
{
    public GameObject playerPrefab;

    public void createPlayer()
    {
        GameEditor.createGameObject(playerPrefab, new Vector2());
    }
}
