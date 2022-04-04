using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{

    [SerializeField]
    VisualElement ve;
    List<Label> lblScores;
    GameManager.eGameStarte oldState;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        ve = GetComponent<UIDocument>().rootVisualElement;

        var buttons = ve.Query<Button>();
        foreach (var btn in buttons.ToList())
        {
            btn.RegisterCallback<ClickEvent>((evt) => buttonClicked(btn,evt));
        }

        lblScores = ve.Query<Label>("lbl_score").ToList();

        foreach(var lblScore in lblScores)
        {
            lblScore.text = "0";
        }

        GameManager.StateChanged += GameManager_StateChanged;
        GameManager.ScoreChanged += GameManager_ScoreChanged;
    }

    private void GameManager_ScoreChanged(int newScore)
    {
        foreach(var lblScore in lblScores)
        {
            lblScore.text = newScore + "";
        }
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= GameManager_StateChanged;
        GameManager.ScoreChanged -= GameManager_ScoreChanged;
    }

    private void GameManager_StateChanged(GameManager.eGameStarte newState)
    {
        if(newState == GameManager.eGameStarte.Playing && oldState == GameManager.eGameStarte.StartScreen)
        {
            ve.Q<VisualElement>("StartScreenBackground").style.display = DisplayStyle.None;
        }
        else if (newState == GameManager.eGameStarte.StartScreen)
        {
            ve.Q<VisualElement>("StartScreenBackground").style.display = DisplayStyle.Flex;
        }

        oldState = newState;
    }

    private void buttonClicked(Button sender ,ClickEvent evt)
    {
        Debug.Log("clicked: " + sender.name);

        switch (sender.name)
        {
            case "cmd_play":
                GameManager.StartGame();
                break;
            case "cmd_openOption":
                GameManager.PauseGame();
                ve.Q<VisualElement>("OptionsMenu").style.display = DisplayStyle.Flex;
                ve.Q<VisualElement>("StartScreenBackground").style.display = DisplayStyle.None;
                break;
            case "cmd_closeOption":
                GameManager.ResumeGame();
                ve.Q<VisualElement>("OptionsMenu").style.display = DisplayStyle.None;
                if (oldState == GameManager.eGameStarte.StartScreen)
                {
                    ve.Q<VisualElement>("StartScreenBackground").style.display = DisplayStyle.Flex;
                }
                break;
            case "cmd_close":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }
}
