using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using Google.Protobuf;
using DnD.Service;
using DnD.Player;

public class ServerConnection
{
    public bool isConnectedToServer = true;

    public TcpClient localClient;
    public NetworkStream netWorkStream;
    public Client localGameCLient;

    private string serverIPAddress = ServerDetails.serverIPAddress;
    private int serverPort = ServerDetails.serverPort;

    [SerializeField]
    private GameObject playerObject;

    public ServerConnection() { }

    public ServerConnection(string serverIPAddress, int serverPort)
    {
        this.serverIPAddress = serverIPAddress;
        this.serverPort = serverPort;
    }

    public void connect(GameObject playerObject)
    {
        try
        {
            this.playerObject = playerObject;
            localClient = new TcpClient(this.serverIPAddress, this.serverPort);

            int localPort = -1;

            ClientRequest clientRequest = new ClientRequest();
            clientRequest.ReqType = ClientRequestType.ClientConnection;

            Client client = new Client();
            client.PortNumber = localPort;
            client.LocalAddress = GetLocalIPAddress();
            client.ClientID = Guid.NewGuid().ToString();

            localGameCLient = client;

            byte[] clientData = client.ToByteArray();

            clientRequest.RequestData = ByteString.CopyFrom(clientData);

            byte[] data = clientRequest.ToByteArray();
            byte[] lengthPrefix = BitConverter.GetBytes(data.Length);
            //localClient.Send(data, data.Length, serverIPAddress, serverPort);
            this.netWorkStream = localClient.GetStream();

            this.netWorkStream.Write(lengthPrefix, 0, 4);
            netWorkStream.Write(data, 0, data.Length);
            netWorkStream.Flush();

            Debug.Log("[info] Server Connection Request sent!");

            this.localGameCLient.Player = new Player();
            this.localGameCLient.Player.PosX = playerObject.transform.position.x;
            this.localGameCLient.Player.PosY = playerObject.transform.position.y;

            this.localGameCLient.Player.ViewDistance = playerObject.GetComponent<PlayerView>().viewDistance;
            this.localGameCLient.Player.CurrentTerrainPosX = -1;
            this.localGameCLient.Player.CurrentTerrainPosY = -1;

        } catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry("localhost");
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
