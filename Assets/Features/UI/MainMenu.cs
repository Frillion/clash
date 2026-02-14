using System;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private CanvasGroup _mainMenuGroup;

    private void Awake()
    {
        _mainMenuGroup = GetComponent<CanvasGroup>();
    }

    public void StartGame()
    {
        _mainMenuGroup.alpha = 0;
        GameManager.Instance.StartGame();
    }
}
