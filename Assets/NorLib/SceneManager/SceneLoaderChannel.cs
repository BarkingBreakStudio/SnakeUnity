using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace NorLib.SceneManagement
{
    [CreateAssetMenu(menuName = "BasicTools/SceneMgr/Scene Manager Channel")]
    public class SceneLoaderChannel : ScriptableObject
    {

        [SceneNamePicker]
        public string PersistentScene;

        public enum e_Cmd
        {
            Load,
            LoadAdditive,
            Unload,
        }

        public class SceneLoderParam
        {
            public string Scene;
            public string Cmd = "";
        }

        public event UnityAction<SceneLoderParam> SceneLoaderRequested = delegate { };


        public void LoadScene(string scene)
        {
            SceneLoaderRequested.Invoke(new SceneLoderParam { Scene = scene, Cmd = nameof(e_Cmd.Load) });
        }

        public void LoadSceneAdditive(string scene)
        {
            SceneLoaderRequested.Invoke(new SceneLoderParam { Scene = scene, Cmd = nameof(e_Cmd.LoadAdditive) });
        }

        public void UnloadScene(string scene)
        {
            SceneLoaderRequested.Invoke(new SceneLoderParam { Scene = scene, Cmd = nameof(e_Cmd.Unload) });
        }

        public void LoadPersistentScene()
        {
            if (!SceneManagerEx.IsSceneLoaded(PersistentScene))
            {
                SceneManager.LoadScene(PersistentScene, LoadSceneMode.Additive);
            }
        }

        public delegate IEnumerator CrossFadeHandler();
        public event CrossFadeHandler CrossFadeStarted;
        public event CrossFadeHandler CrossFadeEnded;

        public IEnumerator StartCrossFade()
        {
            yield return CrossFadeStarted?.Invoke();
        }

        public IEnumerator EndCrossFade()
        {
            yield return CrossFadeEnded?.Invoke();
        }

    }
}