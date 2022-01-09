using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Text errorText;

    string IP;
    string errormessage;


    public void ButtonPlayAsHost()
    {
        Debug.Log("host game");

        // Spiel starten und Verbindung bereitstellen
    }

    public void ButtonSearchForHost()
    {
        Debug.Log("search for host");

        if (OnValidateIPv4())
        {
            // Versuchen eine Verbindung aufzubauen
        }

        errorText.text = errormessage;


    }

    public void GetIPInput(string IP)
    {
        this.IP = IP;
    }

    public bool OnValidateIPv4()
    {
        if (String.IsNullOrEmpty(IP))
        {
            errormessage = "Bitte das \"IPv4\"-Feld ausfüllen.";
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
}
