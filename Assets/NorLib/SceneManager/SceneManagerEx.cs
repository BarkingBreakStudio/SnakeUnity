using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NorLib.SceneManagement
{
    public class SceneManagerEx
    {

        public static void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Additive);
        }

        public static bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (sceneName == SceneManager.GetSceneAt(i).path)
                {
                    return true;
                }
            }
            return false;
        }
    }
}