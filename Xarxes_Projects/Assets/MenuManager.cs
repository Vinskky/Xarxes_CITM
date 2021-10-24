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


    public static string textTestClient { get; set; }
    public static string textTestServer { get; set; }
    public static List<string> consoleTestClient { get; set; }
    public static List<string> consoleTestServer { get; set; }

    public Canvas playModeCanvas;
    public Canvas mainMenuCanvas;

    bool tcpActive = false;
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

        if(consoleTestServer != null)
        {
            for (int i = 0; i < consoleTestServer.Count; ++i)
            {
                consoleServer.text +=  consoleTestServer[i].ToString() + "\n";
            } 
        }


        if(consoleTestClient != null)
        {
              for (int i = 0; i < consoleTestClient.Count; ++i)
            {
                consoleClient.text += consoleTestClient[i].ToString() + "\n";
            }
        }

        if (udpActive == true)
        {
            GameObject.FindGameObjectWithTag("TCP_Server").SetActive(false); 
            GameObject.FindGameObjectWithTag("TCP_Client").SetActive(false); 

            udpActive = false;
        }

        if(tcpActive == true)
        {
            GameObject.FindGameObjectWithTag("UDP_Server").SetActive(false);
            GameObject.FindGameObjectWithTag("UDP_Client").SetActive(false);

            tcpActive = false;
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

    }
}
