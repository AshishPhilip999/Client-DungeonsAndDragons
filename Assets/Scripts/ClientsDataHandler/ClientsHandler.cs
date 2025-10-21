using UnityEngine;
using DnD.Service;
using System.Collections.Generic;

public class ClientsHandler : MonoBehaviour
{
    public GameObject defaultPlayerPlaceHolder;
    public Dictionary<string, GameObject> connectedPlayers = new Dictionary<string, GameObject>();

    public void updatePlayerData(Client client)
    {
        GameObject player;

        if (connectedPlayers.TryGetValue(client.ClientID, out player) && player != null)
        {
            updatePlayer(player, client.Player);
        }
        else
        {
            addPlayer(client.ClientID, client.Player);
        }
    }

    public void addPlayer(string ClientID, DnD.Player.Player newPlayer)
    {
        // Instatiating player
        GameObject playerObject = Instantiate(defaultPlayerPlaceHolder, new Vector3(newPlayer.PosX, newPlayer.PosY, 0), Quaternion.identity);

        connectedPlayers.Add(ClientID, playerObject);
        Debug.Log("[ClientsHandler] Added new Player");

    }

    public void updatePlayer(GameObject player, DnD.Player.Player currPlayer)
    {
        Debug.Log("[ClientHandler:: updatePlayer] Other player curr position posX: " + currPlayer.PosX + ", posY: " + currPlayer.PosY);
        player.transform.position = new Vector3(currPlayer.PosX, currPlayer.PosY);
    }
}
