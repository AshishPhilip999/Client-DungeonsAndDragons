using UnityEngine;
using DnD.Service;
using DnD.Player;

public class ServiceHandler
{
    private ClientUpdateHandler clientUpdateHandler;
    private ClientsHandler clientsHandler;

    public ServiceHandler()
    {
        this.clientUpdateHandler = new ClientUpdateHandler();
        this.clientsHandler = new ClientsHandler();
    }

    public void clientUpdate(ClientContext clientContext)
    {
        clientUpdateHandler.update(clientContext);
    }

    public void joinClient(Client client)
    {
        Player player = client.Player;
        this.clientsHandler.addPlayer(client.ClientID, player);
    }
}
