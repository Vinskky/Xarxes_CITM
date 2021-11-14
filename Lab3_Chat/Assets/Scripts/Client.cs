using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine.UI;



public class Client : MonoBehaviour
{

    public GameObject userListPanel;

    public GameObject chatPanel;

    public GameObject loginPanel;

    public GameObject blockPanel;

    public Text userList;

    public Text chatText;

    public Text nameText;

    public InputField loginField;

    public InputField chatField;

    Socket socket = null;

    IPEndPoint ip = null;

    int port = 5959;

    bool exit = false;

    bool backUpSend = false;

    bool destroyClient = false;

    bool isConnected = false;

    Thread thread;

    // Start is called before the first frame update
    void Start()
    {

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

        destroyClient = false;

        socket.Connect(ip);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return) && chatField.text.StartsWith("/") && chatField.text != null)
        {
            //Read which command is
            string[] data = chatField.text.Split(' ');

            string command = data[0];
            string userToSend = "";
            string mssg = "";

            if (command != "/whisper" && command != "/kick")
            {
                //only used for changeName, so we put _ on " ".
                for (uint i = 1; i < data.Length; ++i)
                {
                    mssg += data[i].ToString() + "_";
                }
            }
            else
            {
                userToSend = data[1].ToString();
                for (uint i = 2; i < data.Length; ++i)
                {
                    mssg += data[i].ToString() + " ";
                }
            }

            switch (command)
            {
                case "/help":
                    {
                        //Returns to user a message explaining all the commands.
                        CommandHelp();
                    }
                    break;
                case "/list":
                    {
                        //Returns to user a message with the list of active users.
                        CommandList();
                    }
                    break;
                case "/kick":
                    {
                        //User input name and if exist kicks him from the chat
                        CommandKick(userToSend);
                    }
                    break;
                case "/whisper":
                    {
                        CommandWhisper(userToSend, mssg);
                    }
                    break;
                case "/changeName":
                    {
                        CommandChangeName(mssg);
                    }
                    break;
            }

        }
        else if (Input.GetKeyDown(KeyCode.Return) && isConnected == false )
        {
            Message sendMsg = new Message();
            sendMsg.clientName = loginField.text;
            sendMsg.type = Message.MessageType.ClientToServer;

            Sending(sendMsg);

            isConnected = true;

            Debug.Log("Client: Connected to the server " + ip.Address + " at port " + ip.Port);
        }

        if (isConnected == true)
        {
            if (Input.GetKeyDown(KeyCode.Return) && chatField.text != "" && chatField.text != null && chatField.text.StartsWith("/") == false)
            {
                Message chatMsg = new Message();
                chatMsg.type = Message.MessageType.Brodcast;
                chatMsg.clientText = chatField.text;
                chatMsg.clientName = nameText.text;
                chatMsg.clientColor = nameText.color;

                Sending(chatMsg);
            }

            if (socket.Poll(1000, SelectMode.SelectRead))
            {
                try
                {

                    byte[] data = new byte[2048];

                    int receivedBytes = socket.Receive(data);

                    if (receivedBytes > 0)
                    {
                        string dataMsg = Encoding.UTF8.GetString(data, 0, receivedBytes);

                        Message message = JsonUtility.FromJson<Message>(dataMsg);

                        switch (message.type)
                        {
                            case Message.MessageType.ClientToServer:
                                {
                                    //Welcome Package

                                    nameText.text = message.clientName;
                                    loginPanel.SetActive(false);
                                    blockPanel.SetActive(false);

                                    //userList.text += message.clientName + "\n";

                                    break;
                                }

                            case Message.MessageType.ClientToClient:
                                {
                                    //Direct Message

                                    break;
                                }
                            case Message.MessageType.Brodcast:
                                {
                                    //Chat Room 

                                    string temp = "<b>" + message.clientName + ": " + "</b>" + message.clientText + "\n";
                                    chatText.text += temp;

                                    break;
                                }
                            default:
                                {
                                    Debug.Log("Stop trolling, what is this message, no correct message type");
                                    break;
                                }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());

                    /*if (socket != null && backUpSend == false)
                    {
                        Debug.Log("Client: Did not get message from server.");
                        backUpSend = true;

                        Message sendMsg = new Message();
                        sendMsg.clientName = inputField.text;
                        sendMsg.type = Message.MessageType.ClientToServer;

                        Sending(sendMsg);
                    }
                    else if (socket != null && backUpSend == true)
                    {
                        socket.Close();
                        socket = null;
                        exit = true;
                    }*/
                }
            }
        }
    }
    void Sending(Message message)
    {
        if (socket != null && socket.Poll(1000, SelectMode.SelectWrite))
        {
            //Serialize

            try
            {
                string json = JsonUtility.ToJson(message);

                byte[] data = Encoding.UTF8.GetBytes(json);

                int bytesSend = socket.Send(data);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }

    void CommandHelp()
    {
        //returns message with all conditions about 
        string helpString = "Welcome to this chatroom. You can use the next commands: /n" +
                            "/help: display information about all available commands. /n" +
                            "/list: display a list with all active users connected to the server. /n" +
                            "/kick + ' ' + username: Disconnects user from the server. /n" +
                            "/whisper + ' ' + username + ' ' + message: Sends exclusively a message to the selected user. /n" + 
                            "changeName + ' ' + newName: change you name";

    }
    void CommandList()
    {

    }

    void CommandKick(string user)
    {
        //check if user exist
        //desconect user
    }
    void CommandWhisper(string user, string msg)
    {
        //check if user exist
        //send to that user message
    }

    void CommandChangeName(string newName)
    {
        //modify class Message clientName to newName
        //update list
        //send message to all users saying now clientname is newName?
    }
}
