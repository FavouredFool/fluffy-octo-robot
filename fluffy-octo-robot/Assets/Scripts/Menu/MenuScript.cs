using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UNET;

public class MenuScript : NetworkBehaviour
{
    public Text errorText;

    string IP;
    string errormessage;

    private void Start()
    {
        Test t = FindObjectOfType<Test>();

        t.disconnect();
    }

    public void ButtonPlayAsSinglePlayer()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("host game, play in single player");
            PlayersManager.Instance.SetRole(Role.SINGLE);

            SceneManager.LoadScene(sceneName: "TileScene");
        }
    }


    public void ButtonPlayAsHost()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("host game");
            PlayersManager.Instance.SetRole(Role.GOD);

            SceneManager.LoadScene(sceneName: "TileScene");
        }
    }

    public void ButtonSearchForHost()
    {
        if (OnValidateIPv4())
        {
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = IP;

            // Versuchen eine Verbindung aufzubauen
            if (NetworkManager.Singleton.StartClient()) {
                Debug.Log("search for host");
                PlayersManager.Instance.SetRole(Role.HUMAN);
            }
        }

        errorText.text = errormessage;
    }

    public void GetIPInput(string IP)
    {
        this.IP = IP;
    }

    public bool OnValidateIPv4()
    {
        Debug.Log(IP);

        if (String.IsNullOrEmpty(IP))
        {
            errormessage = "Bitte das \"IPv4\"-Feld ausfuellen.";
            return false;
        }

        string[] splitValues = IP.Split('.');
        if (splitValues.Length != 4)
        {
            errormessage = "IPv4 ist nicht valide.";
            return false;
        }

        byte tempForParsing;

        bool valid = splitValues.All(r => byte.TryParse(r, out tempForParsing));

        if (valid)
        {
            errormessage = "";
        } else
        {
            errormessage = "IPv4 ist nicht valide.";
        }
        return valid;
    }

    public void ButtonQuit()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void ButtonTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
