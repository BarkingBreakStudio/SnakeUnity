using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace NorLib
{
    public class BaseGraphicsSettingsUIToolkit : MonoBehaviour
    {

        

        [Header("Monitor")]
        public UnityEngine.UIElements.DropdownField dropdownResolutions;
        public UnityEngine.UIElements.Toggle toggleFullscreen;
        public UnityEngine.UIElements.DropdownField dropdownVSyncCount;
        public UnityEngine.UIElements.TextField inputFieldFpsLimit;

        [Header("Graphics")]
        public UnityEngine.UIElements.DropdownField dropdownQuality;

        [Header("Audio")]
        public AudioMixer AudioMixer;
        public UnityEngine.UI.Slider sliderVolumeMaster;
        public UnityEngine.UIElements.Slider sliderBackgroundMusic;
        public UnityEngine.UIElements.Slider sliderSoundEffectMusic;

        public UnityEngine.UIElements.Button muteButton;
        public UnityEngine.UIElements.Button unmuteButton;
        private float masterVolumeBeforeMuting = 0f;

        [Header("Others")]
        public UnityEngine.UI.Button Refresh;

        private BaseGraphicsSettings graphicSettings;


        private Vector2Int[] ScreenResolutions;

        private void Awake()
        {
            graphicSettings = new BaseGraphicsSettings();
        }

        private void OnEnable()
        {
            var rve = GetComponent<UIDocument>().rootVisualElement;
            toggleFullscreen = rve.Q<UnityEngine.UIElements.Toggle>("NorLibFullScreen");

            dropdownResolutions = rve.Q<UnityEngine.UIElements.DropdownField>("NorLibScreenResolution");
            dropdownVSyncCount = rve.Q<UnityEngine.UIElements.DropdownField>("NorLibVSync");
            inputFieldFpsLimit = rve.Q<UnityEngine.UIElements.TextField>("NorLibFPSLimit");
            dropdownQuality = rve.Q<UnityEngine.UIElements.DropdownField>("NorLibGraphicsQuality");

            muteButton = rve.Q<UnityEngine.UIElements.Button>("NorLibMute");
            unmuteButton = rve.Q<UnityEngine.UIElements.Button>("NorLibUnmute");

            sliderBackgroundMusic = rve.Q<UnityEngine.UIElements.Slider>("NorLibBackgroundMusic");
            sliderSoundEffectMusic = rve.Q<UnityEngine.UIElements.Slider>("NorLibSoundeffectMusic");

            RefreshOptions();
            InitCallBacks();
        }


        // Start is called before the first frame update
        void Start()
        {
            //RefreshOptions();
            //InitCallBacks();
        }

        public void RefreshOptions()
        {
            if (toggleFullscreen is not null)
                toggleFullscreen.value = graphicSettings.IsFullscreen();

            if (dropdownQuality is not null)
            {
                List<string> options = new();
                foreach (var qualityName in graphicSettings.GetQualitySettingNames())
                {
                    options.Add(qualityName);
                }
                dropdownQuality.choices = options;
                dropdownQuality.index = graphicSettings.GetQualitySettingLevel();
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

            if (muteButton is not null && unmuteButton is not null)
            {
                float volume;
                if (AudioMixer.GetFloat("Master_Volume", out volume))
                {
                    if (volume < -79.9f)
                    {
                        unmuteButton.style.display = DisplayStyle.Flex;
                        muteButton.style.display = DisplayStyle.None;
                    }
                    else
                    {
                        unmuteButton.style.display = DisplayStyle.None;
                        muteButton.style.display = DisplayStyle.Flex;
                    }
                }
            }

            if (sliderBackgroundMusic is not null)
            {
                float volume;
                if (AudioMixer.GetFloat("Music_Volume", out volume))
                {
                    sliderBackgroundMusic.value = Mathf.Pow(10, volume / 20);
                    sliderBackgroundMusic.lowValue = 0.0001f;
                    sliderBackgroundMusic.highValue = 1f;
                }
            }

            if (sliderSoundEffectMusic is not null)
            {
                float volume;
                if (AudioMixer.GetFloat("Sfx_Volume", out volume))
                {
                    sliderSoundEffectMusic.value = Mathf.Pow(10, volume / 20);
                    sliderSoundEffectMusic.lowValue = 0.0001f;
                    sliderSoundEffectMusic.highValue = 1f;
                }
            }

            if (dropdownResolutions is not null)
            {
                var options = new List<Dropdown.OptionData>();
                var resolutions = new List<Vector2Int>(graphicSettings.GetScreenResolutions());
                dropdownResolutions.choices.Clear();
                foreach (var resolution in resolutions)
                {
                    options.Add(new Dropdown.OptionData(Rest2String(resolution)));
                    dropdownResolutions.choices.Add(Rest2String(resolution));
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
                dropdownResolutions.index = index;
                ScreenResolutions = resolutions.ToArray();
                //dropdownResolutions.value = index;
            }

            if (dropdownVSyncCount is not null)
            {
                var options = new List<string>();
                for (int i = 0; i <= 4; i++)
                {
                    options.Add("Vsync " + i);
                }
                dropdownVSyncCount.choices = options;
                dropdownVSyncCount.index = QualitySettings.vSyncCount;
            }

            if (inputFieldFpsLimit is not null)
            {
                inputFieldFpsLimit.value = graphicSettings.GetFrameLimit().ToString();
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
            toggleFullscreen?.RegisterValueChangedCallback(toggleFullscreenValueChanged);
            dropdownResolutions?.RegisterValueChangedCallback(resolutionValueChanged2);
            dropdownVSyncCount?.RegisterValueChangedCallback(vSyncValueChanged2);
            inputFieldFpsLimit?.RegisterValueChangedCallback(fpsLimitValueChanged2);
            dropdownQuality?.RegisterValueChangedCallback(qualitySettingsChanged2);
            muteButton?.RegisterCallback<ClickEvent>(muteButtonClicked);
            unmuteButton?.RegisterCallback<ClickEvent>(unmuteButtonClicked);
            sliderBackgroundMusic?.RegisterValueChangedCallback(sliderBackgroundMusicValueChanged);
            sliderSoundEffectMusic?.RegisterValueChangedCallback(sliderSoundEffectValueChanged);

            //toggleFullscreen?.onValueChanged.AddListener(fullscreenValueChanged);
            //dropdownQuality?.onValueChanged.AddListener(qualitySettingsChanged);
            sliderVolumeMaster?.onValueChanged.AddListener(masterVolumeValueChnaged);
            //dropdownResolutions?.onValueChanged.AddListener(resolutionValueChanged);
            //dropdownVSyncCount?.onValueChanged.AddListener(vSyncValueChanged);
            //inputFieldFpsLimit?.onValueChanged.AddListener(fpsLimitValueChanged);
            Refresh?.onClick.AddListener(RefreshButtonPressed);
        }

        /*********Music**************/
        private void muteButtonClicked(ClickEvent evt)
        {
            float volume;
            if (AudioMixer.GetFloat("Master_Volume", out volume))
            {
                masterVolumeBeforeMuting = volume;
            }

            AudioMixer.SetFloat("Master_Volume", -80f);
            unmuteButton.style.display = DisplayStyle.Flex;
            muteButton.style.display = DisplayStyle.None;
        }

        private void unmuteButtonClicked(ClickEvent evt)
        {
            AudioMixer.SetFloat("Master_Volume", masterVolumeBeforeMuting);
            unmuteButton.style.display = DisplayStyle.None;
            muteButton.style.display = DisplayStyle.Flex;
        }

        private void sliderBackgroundMusicValueChanged(ChangeEvent<float> evt)
        {
            AudioMixer.SetFloat("Music_Volume", Mathf.Log10(evt.newValue) * 20);
        }

        private void sliderSoundEffectValueChanged(ChangeEvent<float> evt)
        {
            AudioMixer.SetFloat("Sfx_Volume", Mathf.Log10(evt.newValue) * 20);
        }
        /************************/

        private void qualitySettingsChanged2(ChangeEvent<string> evt)
        {
            qualitySettingsChanged(dropdownQuality.index);
        }

        private void fpsLimitValueChanged2(ChangeEvent<string> evt)
        {
            fpsLimitValueChanged(evt.newValue);
        }

        private void vSyncValueChanged2(ChangeEvent<string> evt)
        {
            vSyncValueChanged(dropdownVSyncCount.index);
        }

        private void resolutionValueChanged2(ChangeEvent<string> evt)
        {
            resolutionValueChanged(dropdownResolutions.index);
        }

        private void toggleFullscreenValueChanged(ChangeEvent<bool> evt)
        {
            fullscreenValueChanged(evt.newValue);
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
            //toggleFullscreen?.onValueChanged.RemoveListener(fullscreenValueChanged);
            //dropdownQuality?.onValueChanged.RemoveListener(qualitySettingsChanged);
            sliderVolumeMaster?.onValueChanged.RemoveListener(masterVolumeValueChnaged);
            //dropdownResolutions?.onValueChanged.RemoveListener(resolutionValueChanged);
            //dropdownVSyncCount?.onValueChanged.RemoveListener(vSyncValueChanged);
            //inputFieldFpsLimit?.onValueChanged.RemoveListener(fpsLimitValueChanged);
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