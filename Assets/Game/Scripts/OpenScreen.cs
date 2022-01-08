using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class OpenScreen : MonoBehaviour
{
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private GameObject SceneText;
    [SerializeField] private PlayableDirector TimeLine;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SceneManager.LoadScene("GameUpdate");
        
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKey(KeyCode.Q))
        {
            SceneText.SetActive(false);
            instructionPanel.SetActive(true);
        }

        else
        {
            SceneText.SetActive(true);
            instructionPanel.SetActive(false);
        }

    }

    
}
