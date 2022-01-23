using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialScript : MonoBehaviour
{

    public GameObject[] scenes;
    public VideoPlayer[] videos;
    public Text buttonText;
    int counter = 0;
    int videoCounter = 0;

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
            Debug.Log(videoCounter);
            videos[Mathf.Max(0, videoCounter-1)].gameObject.SetActive(false);
            scenes[counter].SetActive(false);
            counter++;
            if (scenes[counter].GetComponent<RawImage>())
            {
                videos[videoCounter].gameObject.SetActive(true);
                videoCounter++;
            }
            scenes[counter].SetActive(true);

        } else
        {
            SceneManager.LoadScene(sceneName: "MainMenu");
        }
        
    }
}
