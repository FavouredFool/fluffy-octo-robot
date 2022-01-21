using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ToMenuButton : MonoBehaviour
{

    public Button menuButton;


    public void ToMenu()
    {
        SceneManager.LoadScene(sceneName: "MainMenu");
    }

}
