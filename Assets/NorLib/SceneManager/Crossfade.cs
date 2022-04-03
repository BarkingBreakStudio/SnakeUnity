using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NorLib.SceneManagement
{
    public class Crossfade : MonoBehaviour
    {
        [SerializeField]
        SceneLoaderChannel sceneMgrCh;

        Image img;
        GraphicRaycaster graRayCaster;

        private void Awake()
        {
            img = GetComponent<Image>();
            graRayCaster = GetComponentInParent<GraphicRaycaster>();
        }

        void Start()
        {

        }

        void Update()
        {

        }

        private void OnEnable()
        {
            graRayCaster.enabled = false;
            sceneMgrCh.CrossFadeStarted += StartCrossfade;
            sceneMgrCh.CrossFadeEnded += EndCrossfade;
        }

        private void OnDisable()
        {
            sceneMgrCh.CrossFadeStarted -= StartCrossfade;
            sceneMgrCh.CrossFadeEnded -= EndCrossfade;
        }

        public IEnumerator StartCrossfade()
        {
            graRayCaster.enabled = true;
            while (img.color.a < 0.99f)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Clamp01(img.color.a + 1f * Time.deltaTime));
                yield return null;
            }

        }

        public IEnumerator EndCrossfade()
        {
            while (img.color.a > 0.01f)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Clamp01(img.color.a - 1f * Time.deltaTime));
                yield return null;
            }
            graRayCaster.enabled = false;
        }
    }
}