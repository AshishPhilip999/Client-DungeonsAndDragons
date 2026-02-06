using UnityEngine;
using DnD.Service;
using DnD.NPCs;
using System.Collections.Generic;

public class ClientsHandler : MonoBehaviour
{
    public GameObject defaultPlayerPlaceHolder;
    public GameObject defaulyNPCPlaceHolder;

    public Dictionary<string, GameObject> connectedPlayers = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> npcs = new Dictionary<string, GameObject>();

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

    public void addNPC(NPCData npcData)
    {
        Vector3 spawnPosition = new Vector3(npcData.PosX, npcData.PosY, 0);

        GameObject npcObject = Instantiate(defaulyNPCPlaceHolder, spawnPosition, Quaternion.identity);
        npcs.Add(npcData.NpcID, npcObject);
        Debug.Log("[ClientsHandler] Added new NPC");
    }

    public void updateNPC(NPCData npcData)
    {
        GameObject npcObject = npcs[npcData.NpcID];

        NPCMover mover = npcObject.GetComponent<NPCMover>();
        Vector3 targetPos = new Vector3(npcData.PosX, npcData.PosY, 0);

        mover.SetTarget(targetPos, 1f);   // 1 second travel time
        Debug.Log("[ClientsHandler] Updated NPC");
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
