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

    private bool listening = false;
    private bool sending = false;

    Thread thread;

    // Start is called before the first frame update
    void Start()
    {
        sending = true;

        thread = new Thread(threadListeningData);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //La tuya
        ip = new IPEndPoint(IPAddress.Any, recivingPort);
        socket.Bind(ip);

        //Envias
        remote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), sendingPort);
        socket2.Bind(remote);
    }

    // Update is called once per frame
    void Update()
    {
        if (sending)
        {
            int bytesSend = socket.SendTo(Encoding.UTF8.GetBytes(message), message.Length, SocketFlags.None, remote);

            if (bytesSend == message.Length)
            {
                Debug.Log("Send Correctly " + message);
                sending = false;
            }
            else
                Debug.Log("Error");
        }

        if(!listening && !sending)
        {
            listening = true;
            sending = true;
            thread.Start();
        }

    }

    void threadListeningData()
    {
        Debug.LogWarning("Starting Thread!");
   
        byte[] dataSize = new byte[68];
        int bytesRecive = socket2.ReceiveFrom(dataSize, ref remote);

        if (bytesRecive > 0)
        {
            Debug.Log("Recieved Correctly " + Encoding.UTF8.GetString(dataSize));
        }
        else
        {
            Debug.Log("Error");
        }
    }
}
