using UnityEngine;
using DnD.Service;
using System;

public class PlayerUpdateHandler
{
    public void update(PlayerUpdateType playerUpdateType)
    {
        switch (playerUpdateType.UpdateTypeCase)
        {
            case PlayerUpdateType.UpdateTypeOneofCase.PlayerTransformType:
                PlayerTransformType transformType = playerUpdateType.PlayerTransformType;
                switch (transformType)
                {
                    case PlayerTransformType.UpdateAlongX:
                        break;

                    case PlayerTransformType.UpdateAlongY:
                        break;
                }
                break;

            case PlayerUpdateType.UpdateTypeOneofCase.PlayerStatusType:
                PlayerStatusType statusType = playerUpdateType.PlayerStatusType;
                switch (statusType)
                {
                    case PlayerStatusType.Join:
                        // Player joined!
                        break;

                    case PlayerStatusType.Leave:
                        // Player left!
                        break;
                }
                break;
        }
    }
}
