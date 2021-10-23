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

    //Sending a message to the Client if we don't have a response from Server.
    bool backupSend = true;

    Socket socket;
    Socket abortSocket;

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


        Receiving();
    }

    // Update is called once per frame
    void Update()
    {
       if(firstTimeSend)
        Sending();
    }


    void Sending()
    {
        if (socket != null)
        {
            firstTimeSend = false;
            byte[] data = Encoding.ASCII.GetBytes(message); ;
            int bytesSend = socket.SendTo(data, message.Length, SocketFlags.None, remote);

            if (bytesSend == message.Length)
            {
                Debug.Log("Client: Send Correctly " + message);
            }

            else
                Debug.Log("Client: Error sending");
        }
    }

    void Receiving()
    {
        thread = new Thread(threadRecivingServerData);
        thread.Start();
    }

    void threadRecivingServerData()
    {
        Debug.Log("Client: Starting Thread!");
        while (!exit)
        {
            byte[] data = new byte[68];

            try
            {
                int bytesRecive = socket.ReceiveFrom(data, ref remote);

                string msgRecieved = Encoding.ASCII.GetString(data);

                if (bytesRecive > 0)
                {
                    if (msgRecieved.Contains("pong"))
                    {
                        Debug.Log("Client Recieved Correctly " + Encoding.ASCII.GetString(data));
                        Thread.Sleep(1000);
                        Sending();
                    }
                    else if (msgRecieved.Contains("abort"))
                    {
                        Debug.Log("Client: Disconnect");
                        socket.Close();
                        //abortSocket.Close();
                        exit = true;
                        
                        break;  
                    }

                }
                else
                {
                    Debug.Log("Client: Message Error, empty byte[]");
                }
            }
            catch 
            {
                if (thread.IsAlive)
                {
                    Debug.Log("Client: No response from Server");

                    if (backupSend == true)
                    {
                        Sending();
                        backupSend = false;
                    }
                    else
                    {
                        Debug.Log("Client: There is no Server found");
                        //Disconnect Socket and Threat 
                        //Close App
                        socket.Close();
                        exit = true;

                        Debug.Log("Client: Disconnect");
                        break;
                    }
                }
            }

        }
    }

    private void OnDestroy()
    {
        if(thread.IsAlive && exit == false)
        { 
            remote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), clientPort);
            message = "abort";
            Sending();
        }
    }
}
