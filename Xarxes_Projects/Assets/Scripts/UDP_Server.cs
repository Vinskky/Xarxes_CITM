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
    bool exit = false;
    //bool serverSend = false;

    //Sending a message to the Server if we don't have a response from the Client.
    bool backupSend = true;

    Socket socket;

    IPEndPoint ip;

    EndPoint remote;

    int serverPort = 7979;
    int clientPort = 6969;

    public string message = "pong";

    Thread thread;

    // Start is called before the first frame update
    void Start()
    {
        MenuManager.textTestServer = "UDP Server";

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ip = new IPEndPoint(IPAddress.Any, serverPort);
        socket.Bind(ip);

        remote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), clientPort);

        Receiving();
    }

    // Update is called once per frame
    void Update()
    {
            //Sending();
    }

    void Receiving()
    { 
        thread = new Thread(threadRecivingClientData);
        thread.Start();
    }

    void threadRecivingClientData()
    {
        Debug.Log("Server: Starting Thread!");
        MenuManager.consoleTestServer.Add("Server: Starting Thread!");
        while (!exit)
        {
            byte[] data = new byte[68];

            try
            {
                int bytesRecive = socket.ReceiveFrom(data, ref remote);

                string msgPing = Encoding.ASCII.GetString(data);
                string finalMsg = msgPing.Trim('\0');
                if (bytesRecive > 0)
                {
                    if (finalMsg.Contains("ping"))
                    {
                        Debug.Log("Server: Recieved Correctly: " + finalMsg);
                        MenuManager.consoleTestServer.Add("Server: Recieved Correctly: " + finalMsg);
                        
                        Thread.Sleep(500);
                        Sending();
                    }
                    else if (msgPing.Contains("abort"))
                    {
                        Debug.Log("Server: Disconnect");
                        MenuManager.consoleTestServer.Add("Server: Disconnect");
                        socket.Close();
                        //abortSocket.Close();
                        exit = true;
                       
                        break;
                    }
                }
                else
                {
                    Debug.Log("Server: Message Error, empty byte[]");
                    MenuManager.consoleTestServer.Add("Server: Message Error, empty byte[]");
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
                        Debug.Log("Server: There is no Client found");
                        MenuManager.consoleTestServer.Add("Server: There is no Client found");
                        //Disconnect Socket and Threat 
                        //Close App
                        socket.Close();
                        exit = true;

                        Debug.Log("Server: Disconnect");
                        MenuManager.consoleTestServer.Add("Server: Disconnect");
                        break;
                    }
                }
            }
        }
        
    }
    void Sending()
    {
        if (socket != null)
        {
            byte[] data = Encoding.ASCII.GetBytes(message); ;
            int bytesSend = socket.SendTo(data, message.Length, SocketFlags.None, remote);

            if (bytesSend == message.Length)
            {
                Debug.Log("Server: Send Correctly " + message);
            }
            else
                Debug.Log("Server: Error not send anything");
        }
    }

    private void OnDestroy()
    {
        if (thread.IsAlive && exit == false)
        {
            remote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);
            message = "abort";

            Sending();
        }
    }
}
