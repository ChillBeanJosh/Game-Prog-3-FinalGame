using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWinScreen : MonoBehaviour
{
    public Button playAgain;

    private void Start()
    {
        playAgain.onClick.AddListener(StartPlayAgain);
    }


    private void StartPlayAgain()
    {
        sceneManager.Instance.LoadMainMenu();
    }

}
