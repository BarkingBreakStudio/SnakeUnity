using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NorLib.SceneManagement;

namespace NorLib.Sound
{
    public class DestroyOtherAudioListeners : MonoBehaviour
    {
        private void Awake()
        {
            ClearOtherListeners();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }


        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            ClearOtherListeners();
        }

        private void ClearOtherListeners()
        {
            var myListener = GetComponent<AudioListener>();
            if (myListener != null)
            {
                var audioListeners = FindObjectsOfType<AudioListener>();
                foreach (var audioListener in audioListeners)
                {
                    if (audioListener != myListener)
                    {
                        Destroy(audioListener);
                    }
                }
            }
        }
    }
}