using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace NorLib
{
    public class BaseGraphicsSettingsUI : MonoBehaviour
    {

        [Header("Monitor")]
        public Dropdown dropdownResolutions;
        public Toggle toggleFullscreen;
        public Dropdown dropdownVSyncCount;
        public InputField inputFieldFpsLimit;

        [Header("Graphics")]
        public Dropdown dropdownQuality;

        [Header("Audio")]
        public AudioMixer AudioMixer;
        public Slider sliderVolumeMaster;

        [Header("Others")]
        public Button Refresh;

        private BaseGraphicsSettings graphicSettings;


        private Vector2Int[] ScreenResolutions;

        private void Awake()
        {
            graphicSettings = new BaseGraphicsSettings();
        }


        // Start is called before the first frame update
        void Start()
        {
            RefreshOptions();
            InitCallBacks();
        }

        public void RefreshOptions()
        {
            if (toggleFullscreen)
                toggleFullscreen.isOn = graphicSettings.IsFullscreen();

            if (dropdownQuality)
            {
                var options = new List<Dropdown.OptionData>();
                foreach (var qualityName in graphicSettings.GetQualitySettingNames())
                {
                    options.Add(new Dropdown.OptionData(qualityName));
                }
                dropdownQuality.options = options;
                dropdownQuality.value = graphicSettings.GetQualitySettingLevel();
            }

            if (sliderVolumeMaster)
            {
                float volume;
                if (AudioMixer.GetFloat("Master_Volume", out volume))
                {
                    sliderVolumeMaster.value = Mathf.Pow(10, volume / 20);
                    sliderVolumeMaster.minValue = 0.0001f;
                }
            }

            if (dropdownResolutions)
            {
                var options = new List<Dropdown.OptionData>();
                var resolutions = new List<Vector2Int>(graphicSettings.GetScreenResolutions());
                foreach (var resolution in resolutions)
                {
                    options.Add(new Dropdown.OptionData(Rest2String(resolution)));
                }
                Vector2Int curResolution = graphicSettings.GetCurrentScreen();
                Dropdown.OptionData curOption = new Dropdown.OptionData(Rest2String(curResolution));
                int index = resolutions.IndexOf(curResolution);
                if (index == -1)
                {
                    options.Add(curOption);
                    resolutions.Add(curResolution);
                    index = options.Count - 1;
                }
                dropdownResolutions.options = options;
                ScreenResolutions = resolutions.ToArray();
                dropdownResolutions.value = index;
            }

            if (dropdownVSyncCount)
            {
                var options = new List<Dropdown.OptionData>();
                for (int i = 0; i <= 4; i++)
                {
                    options.Add(new Dropdown.OptionData("Vsync " + i));
                }
                dropdownVSyncCount.options = options;
                dropdownVSyncCount.value = QualitySettings.vSyncCount;
            }

            if (inputFieldFpsLimit)
            {
                inputFieldFpsLimit.text = graphicSettings.GetFrameLimit().ToString();
            }
        }

        private void resolutionValueChanged(int index)
        {
            //RemoveCallbacks();
            graphicSettings.SetCurrentScreen(ScreenResolutions[index]);
            //InitCallBacks();
        }

        private void RefreshButtonPressed()
        {
            RemoveCallbacks();
            RefreshOptions();
            InitCallBacks();
        }

        public void InitCallBacks()
        {
            toggleFullscreen?.onValueChanged.AddListener(fullscreenValueChanged);
            dropdownQuality?.onValueChanged.AddListener(qualitySettingsChanged);
            sliderVolumeMaster?.onValueChanged.AddListener(masterVolumeValueChnaged);
            dropdownResolutions?.onValueChanged.AddListener(resolutionValueChanged);
            dropdownVSyncCount?.onValueChanged.AddListener(vSyncValueChanged);
            inputFieldFpsLimit?.onValueChanged.AddListener(fpsLimitValueChanged);
            Refresh?.onClick.AddListener(RefreshButtonPressed);
        }

        private void fpsLimitValueChanged(string text)
        {
            int limit;
            if (!int.TryParse(text, out limit))
            {
                limit = -1;
            }
            graphicSettings.SetFrameLimit(limit);
        }

        private void vSyncValueChanged(int value)
        {
            QualitySettings.vSyncCount = value;
        }

        public void RemoveCallbacks()
        {
            toggleFullscreen?.onValueChanged.RemoveListener(fullscreenValueChanged);
            dropdownQuality?.onValueChanged.RemoveListener(qualitySettingsChanged);
            sliderVolumeMaster?.onValueChanged.RemoveListener(masterVolumeValueChnaged);
            dropdownResolutions?.onValueChanged.RemoveListener(resolutionValueChanged);
            dropdownVSyncCount?.onValueChanged.RemoveListener(vSyncValueChanged);
            inputFieldFpsLimit?.onValueChanged.RemoveListener(fpsLimitValueChanged);
            Refresh?.onClick.RemoveListener(RefreshButtonPressed);
        }

        private void masterVolumeValueChnaged(float value)
        {
            AudioMixer.SetFloat("Master_Volume", Mathf.Log10(value) * 20);
        }

        private void qualitySettingsChanged(int value)
        {
            graphicSettings.SetQualitySettingLevel(value);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void fullscreenValueChanged(bool value)
        {
            graphicSettings.SetFullscreen(value);
        }


        private string Rest2String(Vector2Int vec)
        {
            return vec.x + " x " + vec.y;
        }
    }
}