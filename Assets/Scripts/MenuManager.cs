using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{

    [SerializeField]
    VisualElement ve;

    Label lblScore;
    int score = 0;

    GameManager.eGameStarte oldState;

    // Start is called before the first frame update
    void Start()
    {
        lblScore.text = score + "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        ve = GetComponent<UIDocument>().rootVisualElement;

        ve.Q<VisualElement>("PlayGameUI").style.display = DisplayStyle.None;

        var buttons = ve.Query<Button>();
        foreach (var btn in buttons.ToList())
        {
            btn.RegisterCallback<ClickEvent>((evt) => buttonClicked(btn,evt));
        }

        lblScore = ve.Q<Label>("lbl_Score");

        GameManager.StateChanged += GameManager_StateChanged;
        FruitSystem.FruitConsumed += FruitSystem_FruitConsumed;
    }

    private void FruitSystem_FruitConsumed()
    {
        score++;
        lblScore.text = score + "";
    }

    private void GameManager_StateChanged(GameManager.eGameStarte newState)
    {
        if(newState == GameManager.eGameStarte.Playing && oldState == GameManager.eGameStarte.StartScreen)
        {
            score = 0;
            lblScore.text = score + "";
        }
         else if (newState == GameManager.eGameStarte.StartScreen)
        {
            ve.Q<VisualElement>("OptionMenu").style.display = DisplayStyle.None;
            ve.Q<VisualElement>("Background").style.display = DisplayStyle.Flex;
            ve.Q<VisualElement>("PlayGameUI").style.display = DisplayStyle.None;
        }
    }

    private void buttonClicked(Button sender ,ClickEvent evt)
    {
        Debug.Log("clicked: " + sender.name);

        if(sender.name == "start")
        {
            GameManager.StartGame();
            ve.Q<VisualElement>("OptionMenu").style.display = DisplayStyle.None;
            ve.Q<VisualElement>("Background").style.display = DisplayStyle.None;
            ve.Q<VisualElement>("PlayGameUI").style.display = DisplayStyle.Flex;
        }
        else if(sender.name == "options")
        {
            ve.Q<VisualElement>("OptionMenu").style.display = DisplayStyle.Flex;
            ve.Q<VisualElement>("MainContent").style.display = DisplayStyle.None;
            ve.Q<VisualElement>("SecondContent").style.display = DisplayStyle.None;
        }
        else if(sender.name == "closeOptions")
        {
            ve.Q<VisualElement>("OptionMenu").style.display = DisplayStyle.None;
            ve.Q<VisualElement>("MainContent").style.display = DisplayStyle.Flex;
            ve.Q<VisualElement>("SecondContent").style.display = DisplayStyle.Flex;
            GameManager.ResumeGame();
        }
        else if(sender.name ==  "closeGame")
        {
            ve.Q<VisualElement>("OptionMenu").style.display = DisplayStyle.None;
            ve.Q<VisualElement>("Background").style.display = DisplayStyle.Flex;
            ve.Q<VisualElement>("PlayGameUI").style.display = DisplayStyle.None;
            GameManager.StopGame();
        } 
        else if(sender.name == "optionsGame")
        {
            GameManager.PauseGame();
            ve.Q<VisualElement>("OptionMenu").style.display = DisplayStyle.Flex;
        }
    }
}
