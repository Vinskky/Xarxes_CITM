using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TCP_Client : MonoBehaviour
{
    bool exit = false;
    bool firstTimeSend = true;
    bool backUpSend = false;

    Socket socket;

    IPEndPoint ip;

    int serverPort = 7979;

    public string message = "ping";

    Thread thread;

    int countPongs = 1;

    // Start is called before the first frame update
    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);

        Receiving();
    }

    // Update is called once per frame
    void Update()
    {
        if (firstTimeSend)
            Sending();
    }

    void Sending()
    {
        if (socket != null)
        {
            firstTimeSend = false;
            byte[] data = Encoding.ASCII.GetBytes(message); ;
            int bytesSend = socket.Send(data);

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
        socket.Connect(ip);

        Debug.Log("Client: Connected to the server " + ip.Address + " at port " + ip.Port);

        while (!exit)
        {
            byte[] data = new byte[68];

            try
            { 
                int receivedBytes = socket.Receive(data);

                string msgRecieved = Encoding.ASCII.GetString(data);

                if (receivedBytes > 0)
                {
                    if (msgRecieved.Contains("pong"))
                    {
                        Debug.Log("Client: Recieved Correctly: " + Encoding.ASCII.GetString(data));
                        Sending();
                        countPongs++;
                        if(countPongs == 5)
                        {   
                            message = "abort";
                            Sending();
                            continue;
                        }
                    }
                }
            }
            catch
            {
                if(socket != null && backUpSend == false)
                {
                    Debug.Log("Client: Did not get message from server.");
                    backUpSend = true;
                    message = "ping";
                    Sending();
                    /*//Create new client
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(ip);
                    message = "ping";
                    Sending();*/
                }
                else if(socket != null && backUpSend == true)
                {
                    socket.Close();
                    socket = null;
                    exit = true;

                }
            }
        }
    }
 
}
