using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class Client : MonoBehaviour
{
    Socket socket = null;

    IPEndPoint ip = null;

    int port = 6969;

    // Start is called before the first frame update
    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
