using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{

    public enum eGameStarte
    {
        StartScreen,
        Playing,
        Paused,
    }

    static eGameStarte _gameState = eGameStarte.StartScreen;

    public delegate void DelStateChanged(eGameStarte newState);
    public static event DelStateChanged StateChanged;


    public static void StartGame()
    {
        if(_gameState == eGameStarte.StartScreen)
        {
            setState(eGameStarte.Playing);
        }
    }

    public static void PauseGame()
    {
        if (_gameState == eGameStarte.Playing)
        {
            setState(eGameStarte.Paused);
        }
    }

    public static void ResumeGame()
    {
        if (_gameState == eGameStarte.Paused)
        {
            setState(eGameStarte.Playing);
        }
    }

    public static void StopGame()
    {
        setState(eGameStarte.StartScreen);
    }

    public static void PlayerLost()
    {
        setState(eGameStarte.StartScreen);
    }

    public static void PlayerWon()
    {
        setState(eGameStarte.StartScreen);
    }



    private static void setState(eGameStarte state)
    {
        if (_gameState != state)
        {
            _gameState = state;
            StateChanged?.Invoke(state);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
