using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TCP_ServerB : MonoBehaviour
{
    bool exit = false;
    bool closeServer = false;

    bool backupSend = true;

    //This socket accepts clients
    Socket socketServer;

    //This socket receive and send the data 
    Socket socketClient;

    IPEndPoint ip;

    int serverPort = 4333;

    public string message = "pong";

    Thread thread;

    // Start is called before the first frame update
    void Start()
    {
        /*MenuManager.textTestServer = "TCP Server";

        socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ip = new IPEndPoint(IPAddress.Any, serverPort);
        socketServer.Bind(ip);

        socketServer.Listen(10);

        Receiving();*/
    }

    public void Init()
    {
        MenuManager.textTestServer = "TCP Server B";

        socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ip = new IPEndPoint(IPAddress.Any, serverPort);
        socketServer.Bind(ip);

        socketServer.Listen(10);

        Receiving();
    }

     // Update is called once per frame
    void Update()
    {
        if (closeServer)
        {
            socketClient.Close();
            socketServer.Close();
            
            thread.Abort();
        }
    }

    void Sending()
    {
        if (socketClient != null)
        {
            byte[] data = Encoding.ASCII.GetBytes(message); ;
            int bytesSend = socketClient.Send(data);

            if (bytesSend == message.Length)
            {
                Debug.Log("Server: Send Correctly " + message);
                MenuManager.consoleTestServer.Add("Server: Send Correctly " + message);

            }
            else
            {
                Debug.Log("Server: Error not send anything");
                MenuManager.consoleTestServer.Add("Server: Error not send anything");
            }
        }
    }

    void Receiving()
    {
        thread = new Thread(threadRecivingServerData);
        thread.Start();
    }

    void threadRecivingServerData()
    {
        //socketClient = socketServer.Accept();


        while (!exit)
        {
            if (socketClient == null)
            {
                socketClient = socketServer.Accept();

                Debug.Log("Server: Client connected in the server " + ip.Address + " at port " + ip.Port);
                MenuManager.consoleTestServer.Add("Server: Client connected in the server " + ip.Address + " at port " + ip.Port);
            }

            byte[] data = new byte[68];

            try
            {
                int receivedBytes = socketClient.Receive(data);

                string msgRecieved = Encoding.ASCII.GetString(data);
                string finalMsg = msgRecieved.Trim('\0');
                if (receivedBytes > 0)
                {
                    if (msgRecieved.Contains("ping"))
                    {
                        Debug.Log("Server: Received Correctly: " + finalMsg);
                        MenuManager.consoleTestServer.Add("Server: Received Correctly: " + finalMsg);

                        Thread.Sleep(500);
                        Sending();
                    }
                    else if (msgRecieved.Contains("disconnect"))
                    {
                        Debug.Log("Server: Received Correctly: " + finalMsg);
                        MenuManager.consoleTestServer.Add("Server: Received Correctly: " + finalMsg);
                        socketClient.Close();
                        socketClient = null;
                        continue;
                    }
                    else if (msgRecieved.Contains("abort"))
                    {
                        Debug.Log("Server: Disconnect");
                        MenuManager.consoleTestServer.Add("Server: Disconnect");
                        socketClient.Close();
                        socketClient = null;
                        socketServer.Close();
                        exit = true;
                        break;
                    }
                }
            }
            catch
            {
                if (thread.IsAlive)
                {
                    Debug.Log("Server: No response from Client");
                    MenuManager.consoleTestServer.Add("Server: No response from Client");
                    if (backupSend == true)
                    {
                        Sending();
                        backupSend = false;
                    }
                    else
                    {
                        Debug.Log("Server: There is no Client found, closing Server");
                        MenuManager.consoleTestServer.Add("Server: There is no Client found, closing Server");
                        socketServer.Close();
                        socketServer = null;
                        exit = true;

                        Debug.Log("Server: Disconnect");
                        MenuManager.consoleTestServer.Add("Server: Disconnect");
                        break;
                    }
                }
            }
        }
    }


    private void OnDestroy()
    {
        if (thread != null && thread.IsAlive && exit == false)
        {
            closeServer = true;
        }
    }
}
