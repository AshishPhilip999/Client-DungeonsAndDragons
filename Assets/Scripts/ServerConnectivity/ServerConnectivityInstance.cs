using UnityEngine;
using DnD.Renderer;
using Google.Protobuf;

public class ServerConnectivityInstance : MonoBehaviour
{
    public GameObject defaultPlayer;
    public GameObject defaulyNPCPlaceHolder;

    public static ServerConnection service;
    public static Transform player;
    public static ClientsHandler clientsHandler;

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

        clientsHandler = new ClientsHandler();
        clientsHandler.defaultPlayerPlaceHolder = defaultPlayer;
        clientsHandler.defaulyNPCPlaceHolder = defaulyNPCPlaceHolder;


        ServerListener serverListener = new ServerListener();
        serverListener.responseHandler.viewHandler = player.gameObject.GetComponent<PlayerView>();
        serverListener.Listen(serverConnection.netWorkStream);
    }
}
