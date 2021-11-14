using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;




//Serialize part
public class Message
{
    public enum MessageType
    {
        ClientToClient,
        ClientToServer,         //Welcome just user name
        Brodcast
    }

    public string clientText = " ";
    public MessageType type = MessageType.ClientToServer;
    public string clientName = " ";
    //Date of the message send
    //public System.DateTime currentTime;
}


public class Server : MonoBehaviour
{
    class ServerClient
    {
        public Socket clientSocket;
        public string clientName;
        public Color clientColor;
        public IPEndPoint clientIp;
        public bool firstTimeConnect;
        //Add more things if needed

        public ServerClient(Socket socket, IPEndPoint ip)
        {
            clientSocket = socket;
            clientIp = ip;
            clientName = "Guest";
            clientColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            firstTimeConnect = true;
        }
    }

    string userList;

    Socket socketServer = null;

    //Clients accepted
    List<ServerClient> acceptedClients;

    //Clients connected
    List<ServerClient> allClients;

    IPEndPoint ip = null;

    int serverPort = 5959;

    // Start is called before the first frame update
    void Start()
    {

        acceptedClients = new List<ServerClient>();
        allClients = new List<ServerClient>();

        //Create the server and Initialize it
        socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ip = new IPEndPoint(IPAddress.Any, serverPort);
        socketServer.Bind(ip);
        socketServer.Listen(10);

        Debug.Log("Server: Connected to port " + ip.Port + "with addres " + ip.Address);
    }

    // Update is called once per frame
    void Update()
    {
        while(socketServer.Poll(1000, SelectMode.SelectRead))
        {

            Socket newSocket = socketServer.Accept();
            IPEndPoint newEndPoint = (IPEndPoint)newSocket.RemoteEndPoint;

            ServerClient clients = new ServerClient(newSocket, newEndPoint);

            acceptedClients.Add(clients);

            //All logs will go to the server console
            Debug.Log("Accepted new client with address: " + clients.clientIp.Address + " and port: " + clients.clientIp.Port);
        }

        for(int i = 0; i < acceptedClients.Count; ++i)
        {
            while (acceptedClients[i].clientSocket.Poll(1000, SelectMode.SelectRead))
            {

                try
                {
                    byte[] data = new byte[2048];

                    int recv = acceptedClients[i].clientSocket.Receive(data);


                    if (recv > 0)
                    {
                        //Deserialize message
                        string dataMsg = Encoding.UTF8.GetString(data,0,recv);

                        //Message to broadcast
                        Message message = JsonUtility.FromJson<Message>(dataMsg);

                        if (acceptedClients[i].clientSocket.Poll(1000, SelectMode.SelectWrite))
                        {

                            switch (message.type)
                            {
                                case Message.MessageType.ClientToServer:
                                    {
                                        //Welcome Package

                                        ServerClient thisClient = new ServerClient(acceptedClients[i].clientSocket, acceptedClients[i].clientIp);

                                        thisClient.clientName = message.clientName;

                                        allClients.Add(thisClient);

                                        if (thisClient.firstTimeConnect == true)
                                        {
                                            Message welcomePackage = message;

                                            welcomePackage.clientText = "Welcome ";

                                            string json = JsonUtility.ToJson(welcomePackage);

                                            byte[] welcomeData = Encoding.UTF8.GetBytes(json);

                                            acceptedClients[i].clientSocket.Send(welcomeData);

                                            message.clientText = "User: " + thisClient.clientName + " connected for the first time";
                                            
                                        }
                                        else
                                        {
                                            message.clientText = "User: " + thisClient.clientName + " reconnected";
                                        }

                                        //Brodcast(message);

                                        break;
                                    }

                                case Message.MessageType.ClientToClient:
                                    {
                                        //Direct Message

                                        break;
                                    }
                                case Message.MessageType.Brodcast:
                                    {
                                        //Chat Room 

                                        Brodcast(message);

                                        break;
                                    }
                                default:
                                    {
                                        Debug.Log("Stop trolling, what is this message, no correct message type");
                                        break;
                                    }
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
        }


       /* //print Users List

        foreach (ServerClient x in connectedClients)
        {
            userList += x.clientName + "\n";
        }

        Brodcast(Encoding.UTF8.GetBytes(userList));*/

    }
    private void Brodcast(Message msg)
    {
        msg.type = Message.MessageType.Brodcast;

        string json = JsonUtility.ToJson(msg);

        byte[] data = Encoding.UTF8.GetBytes(json);

        foreach (ServerClient client in acceptedClients)
        {
            client.clientSocket.Send(data);
        }
    }

    private bool IsValidName(string name)
    {

        foreach (ServerClient client in allClients)
        {
            if (client.clientName == name)
            {
                return true;
            }
            else
                return false;
        }
        return false;
    }

    private void OnDestroy()
    {
        socketServer.Close();
        socketServer = null;
    }

}

