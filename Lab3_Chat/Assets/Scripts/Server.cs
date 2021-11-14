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
        Brodcast,
        Disconnect,
        ChangeName
    }

    public string clientText = "";
    public MessageType type = MessageType.ClientToServer;
    public string clientName = "";
    public Color clientColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

    public List<string> clients = new List<string>();
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

        public ServerClient(Socket socket, IPEndPoint ip, string name, Color color)
        {
            clientSocket = socket;
            clientIp = ip;
            clientName = name;
            clientColor = color;
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

            ServerClient clients = new ServerClient(newSocket, newEndPoint, "", Color.black);

            acceptedClients.Add(clients);

            //All logs will go to the server console
            Debug.Log("Accepted new client with address: " + clients.clientIp.Address + " and port: " + clients.clientIp.Port);
        }

        for(int i = 0; acceptedClients.Count > 0 && i < acceptedClients.Count; ++i)
        {
            while (acceptedClients.Count > 0 && acceptedClients[i].clientSocket.Poll(1000, SelectMode.SelectRead))
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
                                        for(int x = 0; x < acceptedClients.Count; ++x)
                                        {
                                            if(message.clientName == acceptedClients[x].clientName)
                                            {
                                                message.clientName += "_" + UnityEngine.Random.Range(0, 100);
                                            }
                                        }
                                        //Welcome Package
                                        acceptedClients[i].clientName = message.clientName;
                                        acceptedClients[i].clientColor = message.clientColor;
                                        ServerClient thisClient = new ServerClient(acceptedClients[i].clientSocket, acceptedClients[i].clientIp, acceptedClients[i].clientName, acceptedClients[i].clientColor);

                                        thisClient.clientName = message.clientName;

                                        allClients.Add(thisClient);

                                        Message welcomePackage = message;

                                        if (thisClient.firstTimeConnect == true)
                                        {

                                            Debug.Log("Connected to the chat room");

                                            welcomePackage.clientText += message.clientName + " entered the chatroom.";
                                            foreach(ServerClient clients in allClients)
                                            {
                                                welcomePackage.clients.Add(clients.clientName);
                                            }
                                            welcomePackage.type = Message.MessageType.ClientToServer;

                                            string json = JsonUtility.ToJson(welcomePackage);

                                            byte[] welcomeData = Encoding.UTF8.GetBytes(json);

                                            thisClient.clientSocket.Send(welcomeData);

                                            Brodcast(welcomePackage);

                                            message.clientText = "Connected for the first time";

                                        }
                                        else
                                        {
                                            message.clientText = " reconnected";

                                            Debug.Log("Reconnected to the chat room");

                                        }

                                        //Brodcast(message);

                                        break;
                                    }

                                case Message.MessageType.ClientToClient:
                                    {
                                        //Direct Message
                                        Message dm = new Message();
                                        dm.clientName = message.clientName;
                                        dm.clientText = message.clientText;
                                        dm.type = Message.MessageType.ClientToClient;

                                        string json = JsonUtility.ToJson(dm);

                                        byte[] privateMsg = Encoding.UTF8.GetBytes(json);

                                        for (int w = 0; w < acceptedClients.Count; ++w)
                                        {
                                            if (acceptedClients[w].clientName == message.clientName)
                                            {
                                                acceptedClients[w].clientSocket.Send(privateMsg);
                                            }
                                        }

                                        break;
                                    }
                                case Message.MessageType.Brodcast:
                                    {
                                        //Chat Room 

                                        Brodcast(message);

                                        break;
                                    }
                                case Message.MessageType.Disconnect:
                                    {
                                        //Chat Room 
                                        Message disconect = new Message();
                                        disconect.clientName = message.clientName;
                                        disconect.type = Message.MessageType.Disconnect;

                                        string json = JsonUtility.ToJson(disconect);

                                        byte[] byebye = Encoding.UTF8.GetBytes(json);

                                        //acceptedClients.clientSocket.Send(welcomeData);

                                        for (int w = 0; w < allClients.Count; ++w)
                                        {
                                            if(allClients[w].clientName == message.clientName)
                                            {
                                                allClients.Remove(allClients[w]);
                                            }
                                        }

                                        for (int w = 0; w < acceptedClients.Count; ++w)
                                        {
                                            if (acceptedClients[w].clientName == message.clientName)
                                            {
                                                acceptedClients[w].clientSocket.Send(byebye);
                                                acceptedClients.Remove(acceptedClients[w]);
                                            }
                                        }

                                        message.clientText = message.clientName + " left the chatroom";
                                        message.clients.Remove(message.clientName);

                                        if (acceptedClients.Count > 0)
                                            Brodcast(message);
                                        
                                        break;
                                    }
                                case Message.MessageType.ChangeName:
                                    {
                                        string tmp = "";
                                        foreach(ServerClient clients in allClients)
                                        {
                                            if(clients.clientName == message.clientName)
                                            {
                                                tmp = clients.clientName;
                                                clients.clientName = message.clientText;
                                            }
                                        }

                                        foreach (ServerClient clients in acceptedClients)
                                        {
                                            if (clients.clientName == message.clientName)
                                            {
                                                clients.clientName = message.clientText;
                                            }
                                        }

                                        for(int x = 0;x < message.clients.Count;++x)
                                        {
                                            if(message.clients[x] == tmp)
                                            {
                                                message.clients[x] = message.clientText;
                                            }
                                        }
                                        message.clientName = message.clientText;
                                        message.clientText = tmp + " now is: " + message.clientName;

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

