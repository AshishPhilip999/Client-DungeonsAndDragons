using UnityEngine;
using DnD.Renderer;
using Google.Protobuf;

public class ServerConnectivityInstance : MonoBehaviour
{
    public static ServerConnection service;
    public static Transform player;

    public ViewDistanceController viewDistanceController;

    [SerializeField]
    public TerrainGenerator tg;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ServerConnection serverConnection = new ServerConnection();
        serverConnection.connect(player.gameObject);

        service = serverConnection;

        ServerListener.tg = tg;
        ServerListener.Listen(serverConnection.netWorkStream);
    }
}
