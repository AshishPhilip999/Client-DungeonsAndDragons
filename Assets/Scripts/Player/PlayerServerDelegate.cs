using UnityEngine;
using System.Collections.Generic;
public interface PlayerViewHandler
{
    void createTiles(List<Vector2Int> currTiles);
    void finishMissedTiles();
}

public class PlayerServerDelegate
{
    
}
