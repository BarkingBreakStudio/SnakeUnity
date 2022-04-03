using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NorLib.Timer
{
    public class BasicTimer : MonoBehaviour
    {
        public UnityEvent EvtTimerTick;

        [SerializeField]
        private float interval = 1;
        [SerializeField]
        private float elepasedTime = 0;


        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            elepasedTime = interval;
        }

        // Update is called once per frame
        void Update()
        {
            if (elepasedTime > interval)
            {
                elepasedTime = interval;
            }
            elepasedTime -= Time.deltaTime;
            if (elepasedTime < 0)
            {
                EvtTimerTick.Invoke();
                elepasedTime += interval;
            }
        }

    }
}