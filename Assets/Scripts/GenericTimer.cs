using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTimer : MonoBehaviour
{

    [SerializeField]
    private string timerName;
    [SerializeField]
    private float interval = 1;
    [SerializeField]
    private float counter;

    private Action<GenericTimer> elapsedCallback;


    public float Interval
    {
        get { return interval; }
        set { interval = value > 0 ? value : 0; }
    }

    private void Init(Action<GenericTimer> elapsedCallback, string timerName, float interval)
    {
        this.timerName = timerName;
        this.Interval = interval;
        this.elapsedCallback = elapsedCallback;
        counter = 0;
    }

    // Start is called before the first frame update
    void Awake()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > interval)
        {
            //counter -= interval;
            counter = 0;
            elapsedCallback?.Invoke(this);
        }
    }


    public static GenericTimer CreateInstance(GameObject go, Action<GenericTimer> elapsedCallback, string timerName, float interval)
    {
        var gt = go.AddComponent<GenericTimer>();
        gt.Init(elapsedCallback, timerName, interval);
        gt.enabled = true;
        return gt;
    }

}
