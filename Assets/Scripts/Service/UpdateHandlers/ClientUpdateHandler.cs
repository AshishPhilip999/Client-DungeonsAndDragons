using UnityEngine;
using DnD.Service;

public class ClientUpdateHandler
{
    private PlayerUpdateHandler playerUpdateHandler;

    public ClientUpdateHandler()
    {
        this.playerUpdateHandler = new PlayerUpdateHandler();
    }

    public void update(ClientContext contextType)
    {
        switch(contextType.ContextTypeCase)
        {
            case ClientContext.ContextTypeOneofCase.PlayerUpdateType:

                playerUpdateHandler.update(contextType.PlayerUpdateType);
                break;
        }
    }
}
