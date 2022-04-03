using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorLib.Timer
{
    public class BasicStopWatch : MonoBehaviour
    {
        [SerializeField]
        float elapsedTime;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            elapsedTime += Time.deltaTime;
        }

        public virtual void ResetWatch()
        {
            elapsedTime = 0;
        }

        public float ElapsedTime
        {
            get
            {
                return elapsedTime;
            }
            set
            {
                elapsedTime = value;
            }
        }
    }
}