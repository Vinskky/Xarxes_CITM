using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDP_Client : MonoBehaviour
{
    bool exit = false;
    bool firstTimeSend = true;

    Socket socket;

    IPEndPoint ip;

    EndPoint remote;

    int serverPort = 7979;
    int clientPort = 6969;

    public string message = "ping";

    Thread thread;

    // Start is called before the first frame update
    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ip = new IPEndPoint(IPAddress.Any, clientPort);
        socket.Bind(ip);

        remote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);

        
        Recieving();
    }

    // Update is called once per frame
    void Update()
    {
       if(firstTimeSend)
        Sending();
    }


    void Sending()
    {
        firstTimeSend = false;
        byte[] data = Encoding.ASCII.GetBytes(message); ;
        int bytesSend = socket.SendTo(data, message.Length, SocketFlags.None, remote);

        if (bytesSend == message.Length)
        {
            Debug.Log("Client Send Correctly " + message);
        }
        else
            Debug.Log("Client Error sending");
    }

    void Recieving()
    {
        thread = new Thread(threadRecivingServerData);
        thread.Start();
    }

    void threadRecivingServerData()
    {
        Debug.Log("Starting Client Thread!");
        while (!exit)
        {

            byte[] dataSize = new byte[68];
            int bytesRecive = socket.ReceiveFrom(dataSize, ref remote);

            string msgRecieved = Encoding.ASCII.GetString(dataSize);
            if (bytesRecive > 0)
            {
                if(msgRecieved.Contains("pong"))
                {
                    Debug.Log("Client Recieved Correctly " + Encoding.ASCII.GetString(dataSize));
                    Thread.Sleep(500);
                    Sending();
                }
                
            }
            else
            {
                Debug.Log("Client Error");
            }

            Thread.Sleep(500);
        }
    }

    private void OnDestroy()
    {
        socket.Close();
    }
}
