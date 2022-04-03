using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var rve = GetComponent<UIDocument>().rootVisualElement;
        rve.Query<Button>().ToList().ForEach(button =>
        {
            button.RegisterCallback<ClickEvent>((evt) => buttonClicked(button, evt));
            button.RegisterCallback<MouseOverEvent>((evt) => buttonMouseOvered(button, evt));
        }
        );
    }

    private void buttonMouseOvered(Button button, MouseOverEvent evt)
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {

    }

    private void buttonClicked(Button btn, ClickEvent evt)
    {
    
        if (btn.name == "BtnQuit")
        {
            Application.Quit();
        }
        else if(btn.name == "BtnStartGame")
        {
            SceneManager.LoadScene(1);
        }
    }

}
