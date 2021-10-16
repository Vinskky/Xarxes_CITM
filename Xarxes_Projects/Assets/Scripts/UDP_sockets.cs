using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDP_sockets : MonoBehaviour
{

    Socket socket;
    Socket socket2;

    IPEndPoint ip;

    EndPoint remote;

    public int recivingPort = 5000;
    public int sendingPort = 5001;

    public string message = "ping";

    Thread thread;

    // Start is called before the first frame update
    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //La tuya
        ip = new IPEndPoint(IPAddress.Any, recivingPort);
        socket.Bind(ip);

        //Envias
        remote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), sendingPort);
        socket2.Bind(remote);

        Sending();

        Recieving();

    }

    // Update is called once per frame
    void Update()
    {

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

    void Recieving()
    {
        thread = new Thread(threadRecivingData);
        thread.Start();
    }

    void threadRecivingData()
    {
        Debug.LogWarning("Starting Thread!");
   
        byte[] dataSize = new byte[68];
        int bytesRecive = socket2.ReceiveFrom(dataSize, ref remote);
        if (bytesRecive > 0)
        {
            Debug.Log("Recieved Correctly " + Encoding.UTF8.GetString(dataSize));
            Debug.Log("Sent Response: pong");
        }
        else
        {
            Debug.Log("Error");
        }
    }

    private void OnDestroy()
    {
        if(thread != null && thread.IsAlive)
        {
            thread.Abort();
        }
    }
}