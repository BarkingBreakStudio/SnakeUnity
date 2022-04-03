using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NorLib.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        SceneLoaderChannel sceneMgrCh;

        private void Awake()
        {
        }

        private void OnEnable()
        {
            if (sceneMgrCh) sceneMgrCh.SceneLoaderRequested += SceneMgrCh_LoadSceneRequested;
        }

        private void OnDisable()
        {
            if (sceneMgrCh) sceneMgrCh.SceneLoaderRequested -= SceneMgrCh_LoadSceneRequested;
        }


        private void SceneMgrCh_LoadSceneRequested(SceneLoaderChannel.SceneLoderParam slp)
        {
            switch (slp.Cmd)
            {
                case nameof(SceneLoaderChannel.e_Cmd.Load):
                    Debug.Log("Load Scene: " + slp.Scene);
                    StartCoroutine(LoadScene(slp.Scene));
                    break;
                case nameof(SceneLoaderChannel.e_Cmd.LoadAdditive):
                    Debug.Log("LoadSceneAdditive: " + slp.Scene);
                    StartCoroutine(LoadSceneAdditive(slp.Scene));
                    break;
                case nameof(SceneLoaderChannel.e_Cmd.Unload):
                    Debug.Log("UnloadScene: " + slp.Scene);
                    StartCoroutine(UnloadScene(slp.Scene));
                    break;
            }
        }


        public IEnumerator LoadScene(string sceneName)
        {
            Physics.autoSimulation = false;
            yield return sceneMgrCh.StartCrossFade();

            //delete all Scences except persisten scene 
            List<string> ScenesToDesroy = new List<string>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene curScene = SceneManager.GetSceneAt(i);

                if (curScene.path != sceneMgrCh.PersistentScene)
                {
                    ScenesToDesroy.Add(curScene.name);
                }
            }

            foreach (var scene in ScenesToDesroy)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
            }

            //load new scene
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(sceneName));

            yield return sceneMgrCh.EndCrossFade();
            Physics.autoSimulation = true;
        }

        public IEnumerator LoadSceneAdditive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        public IEnumerator UnloadScene(string sceneName)
        {
            yield return SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}