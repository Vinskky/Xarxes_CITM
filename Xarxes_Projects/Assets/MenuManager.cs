using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instanceMenu;

    [Header("Protocol Types")]
    public Text protocolTypeClient;
    public Text protocolTypeServer;
    public Text consoleClient;
    public Text consoleServer;

    private GameObject serverObj;
    private GameObject clientObj;
    public GameObject clientButton;
    public static string textTestClient { get; set; }
    public static string textTestServer { get; set; }
    public static List<string> consoleTestClient { get; set; }
    public static List<string> consoleTestServer { get; set; }

    public Canvas playModeCanvas;
    public Canvas mainMenuCanvas;

    bool tcpActive = false;
    bool tcpActiveB = false;
    bool udpActive = false;

    private void Start()
    {
        consoleServer.text = "";
        consoleClient.text = "";
        mainMenuCanvas.enabled = true;
        playModeCanvas.enabled = false;

        consoleTestClient = new List<string>();
        consoleTestServer = new List<string>();

    }

    private void Update()
    {
        protocolTypeClient.text = textTestClient;
        protocolTypeServer.text = textTestServer;
        
        if (consoleTestServer.Count > 0)
            DisplayServerList();


        if (consoleTestClient.Count > 0)
            DisplayClientList();

        if (udpActive == true && GameObject.FindGameObjectWithTag("UDP_Server") != null && GameObject.FindGameObjectWithTag("UDP_Client") != null)
        {
            serverObj = GameObject.FindGameObjectWithTag("UDP_Server");
            clientObj = GameObject.FindGameObjectWithTag("UDP_Client");
            clientObj.GetComponent<UDP_Client>().Init();
            serverObj.GetComponent<UDP_Server>().Init();

            GameObject.FindGameObjectWithTag("TCP_Server").SetActive(false); 
            GameObject.FindGameObjectWithTag("TCP_Client").SetActive(false);
            GameObject.FindGameObjectWithTag("TCP_ServerB").SetActive(false);
            GameObject.FindGameObjectWithTag("TCP_ClientB").SetActive(false);
            clientButton.SetActive(false);

            udpActive = false;
        }

        if(tcpActive == true && GameObject.FindGameObjectWithTag("TCP_Server") != null && GameObject.FindGameObjectWithTag("TCP_Client") != null)
        {
            serverObj = GameObject.FindGameObjectWithTag("TCP_Server");
            clientObj = GameObject.FindGameObjectWithTag("TCP_Client");
            clientObj.GetComponent<TCP_Client>().Init();
            serverObj.GetComponent<TCP_Server>().Init();

            GameObject.FindGameObjectWithTag("UDP_Server").SetActive(false);
            GameObject.FindGameObjectWithTag("UDP_Client").SetActive(false);
            GameObject.FindGameObjectWithTag("TCP_ServerB").SetActive(false);
            GameObject.FindGameObjectWithTag("TCP_ClientB").SetActive(false);
            clientButton.SetActive(false);

            tcpActive = false;
        }

        if (tcpActiveB == true && GameObject.FindGameObjectWithTag("TCP_ServerB") != null && GameObject.FindGameObjectWithTag("TCP_ClientB") != null)
        {
            serverObj = GameObject.FindGameObjectWithTag("TCP_ServerB");
            clientObj = GameObject.FindGameObjectWithTag("TCP_ClientB");
            clientObj.GetComponent<TCP_ClientB>().Init();
            serverObj.GetComponent<TCP_ServerB>().Init();

            GameObject.FindGameObjectWithTag("UDP_Server").SetActive(false);
            GameObject.FindGameObjectWithTag("UDP_Client").SetActive(false);
            GameObject.FindGameObjectWithTag("TCP_Server").SetActive(false);
            GameObject.FindGameObjectWithTag("TCP_Client").SetActive(false);

            tcpActiveB = false;
        }

    }

    public void UDP_Button()
    {
        mainMenuCanvas.enabled = false;
        playModeCanvas.enabled = true;

        SceneManager.LoadScene("Client", LoadSceneMode.Additive);
        SceneManager.LoadScene("Server", LoadSceneMode.Additive);

        udpActive = true;
    }

    public void TCP_Button()
    {
        mainMenuCanvas.enabled = false;
        playModeCanvas.enabled = true;

        SceneManager.LoadScene("Client", LoadSceneMode.Additive);
        SceneManager.LoadScene("Server", LoadSceneMode.Additive);

        tcpActive = true;
    }

    public void TCP2_Button()
    {
        mainMenuCanvas.enabled = false;
        playModeCanvas.enabled = true;

        SceneManager.LoadScene("Client", LoadSceneMode.Additive);
        SceneManager.LoadScene("Server", LoadSceneMode.Additive);

        tcpActiveB = true;
    }

    public void AddNewClient()
    {
        Instantiate(clientObj);
    }

    public void ReturnButton()
    {
        mainMenuCanvas.enabled = true;
        playModeCanvas.enabled = false;

        SceneManager.UnloadScene(1);
        SceneManager.UnloadScene(2);

        if(clientButton.activeSelf == false)
            clientButton.SetActive(true);


        consoleServer.text = "";
        consoleClient.text = "";

        consoleTestServer.Clear();
        consoleTestClient.Clear();


        udpActive = false;
        tcpActive = false;
        tcpActiveB = false;
    }
    public void CloseApp()
    {
        Application.Quit();
    }
    void DisplayServerList()
    {
        string[] arrayStr = consoleTestServer.ToArray();
        string newStr = string.Join("\n", arrayStr);
        consoleServer.text = newStr;
    }

    void DisplayClientList()
    {
        string[] arrayStr = consoleTestClient.ToArray();
        string newStr = string.Join("\n", arrayStr);
        consoleClient.text = newStr;
    }
}
