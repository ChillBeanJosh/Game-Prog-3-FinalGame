using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainmenu : MonoBehaviour
{
    public Button Play;
    public Button Tutorial;


    private void Start()
    {
        Play.onClick.AddListener(StartGame);
        Tutorial.onClick.AddListener(StartTutorial);
    }

    private void StartGame()
    {
        sceneManager.Instance.LoadNewGame();
    }


    private void StartTutorial()
    {
        sceneManager.Instance.LoadScene(sceneManager.Scene.Tutorial);
    }
}
