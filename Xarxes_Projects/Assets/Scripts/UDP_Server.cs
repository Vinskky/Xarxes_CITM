using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDP_Server : MonoBehaviour
{
    Socket socket;

    IPEndPoint ip;

    EndPoint remote;

    int recivingPort = 7979;
    int sendingPort = 6969;

    public string message = "pong";

    Thread thread;

    // Start is called before the first frame update
    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ip = new IPEndPoint(IPAddress.Any, recivingPort);
        socket.Bind(ip);

        remote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), sendingPort);

        Recieving();
    }

    // Update is called once per frame
    void Update()
    {
        Sending();
    }

    void Recieving()
    { 
        thread = new Thread(threadRecivingData);
        thread.Start();
    }

    void threadRecivingData()
    {
        Debug.Log("Starting Thread!");

        byte[] dataSize = new byte[68];
        int bytesRecive = socket.ReceiveFrom(dataSize, ref remote);

        if (bytesRecive > 0)
        {
            Debug.Log("Recieved Correctly " + Encoding.UTF8.GetString(dataSize));
        }
        else
        {
            Debug.Log("Error");
        }

        Thread.Sleep(500000);
    }
    void Sending()
    {

        byte[] data = Encoding.ASCII.GetBytes(message); ;
        int bytesSend = socket.SendTo(data, message.Length, SocketFlags.None, remote);

        if (bytesSend == message.Length)
        {
            Debug.Log("Send Correctly " + message);
        }
        else
            Debug.Log("Error");

    }
}