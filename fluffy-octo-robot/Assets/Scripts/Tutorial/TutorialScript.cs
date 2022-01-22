using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{

    public GameObject[] scenes;
    public Text buttonText;
    int counter = 0;

    protected void Update()
    {
        if (counter < scenes.Length-1)
        {
            buttonText.text = "Next";
        } else
        {
            buttonText.text = "End";
        }
    }


    public void nextScene()
    {
        if (counter < scenes.Length-1)
        {
            scenes[counter].SetActive(false);
            counter++;
            scenes[counter].SetActive(true);
        } else
        {
            SceneManager.LoadScene(sceneName: "MainMenu");
        }
        
    }
}
