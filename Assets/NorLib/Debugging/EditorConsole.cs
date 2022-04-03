using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorLib
{
    public class EditorConsole : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DebugLog(string messages)
        {
            Debug.Log(messages);
        }

        public void DebugWarning(string messages)
        {
            Debug.LogWarning(messages);
        }

        public void DebugError(string messages)
        {
            Debug.LogError(messages);
        }
    }
}