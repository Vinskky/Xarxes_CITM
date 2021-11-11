using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;


public class Server : MonoBehaviour
{
    Socket socketServer = null;

    //Sockets Listening (Despues cambiar por lista de users)
    ArrayList listenList;
    //Sockets Writing
    ArrayList writeList;
    //Sockets Error
    ArrayList errorList;

    ArrayList acceptedClients;

    IPEndPoint ip = null;

    int serverPort = 6969;

   
    // Start is called before the first frame update
    void Start()
    {
        listenList = new ArrayList();
        writeList = new ArrayList();
        errorList = new ArrayList();

        socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ip = new IPEndPoint(IPAddress.Any, serverPort);
        socketServer.Bind(ip);
        socketServer.Listen(10);

        Debug.Log("Server: Connected to port " + ip.Port + "with addres " + ip.Address);
    }

    // Update is called once per frame
    void Update()
    {
        listenList.Add(socketServer);
        while(socketServer.Poll(1000, SelectMode.SelectRead))
        {
            acceptedClients.Add(socketServer.Accept());
        }
    }
}
