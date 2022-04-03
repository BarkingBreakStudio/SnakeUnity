using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NorLib.SceneManagement
{
    public class SceneInitializer : MonoBehaviour
    {

        [SerializeField]
        SceneLoaderChannel SceneMgrCh;

        void Awake()
        {
            if (SceneMgrCh)
            {
                Debug.Log("InitScene");
                SceneMgrCh.LoadPersistentScene();
            }
        }

        void Start()
        {

        }

        void Update()
        {

        }
    }
}