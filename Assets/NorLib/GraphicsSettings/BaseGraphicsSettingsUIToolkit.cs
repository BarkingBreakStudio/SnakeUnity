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
        /*Screen steetings*/
        UnityEngine.UIElements.DropdownField dropdownResolutions;
        UnityEngine.UIElements.Toggle toggleFullscreen;
        UnityEngine.UIElements.DropdownField dropdownVSyncCount;
        UnityEngine.UIElements.TextField inputFieldFpsLimit;
        /*Graphics settings*/
        UnityEngine.UIElements.DropdownField dropdownQuality;
        /*Audio slider*/
        public AudioMixer AudioMixer;
        UnityEngine.UIElements.Slider sliderBackgroundMusic;
        UnityEngine.UIElements.Slider sliderSoundEffectMusic;
        /*Audio mute/unmute*/
        UnityEngine.UIElements.Button muteButton;
        UnityEngine.UIElements.Button unmuteButton;
        float masterVolumeBeforeMuting = 0f;

        /*API to change settings*/
        BaseGraphicsSettings graphicSettings;

        /*poossible screen resolutions*/
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


        public void InitCallBacks()
        {
            toggleFullscreen?.RegisterValueChangedCallback(toggleFullscreenValueChanged);
            dropdownResolutions?.RegisterValueChangedCallback(resolutionValueChanged);
            dropdownVSyncCount?.RegisterValueChangedCallback(vSyncValueChanged);
            inputFieldFpsLimit?.RegisterValueChangedCallback(fpsLimitValueChanged);
            dropdownQuality?.RegisterValueChangedCallback(qualitySettingsChanged);
            muteButton?.RegisterCallback<ClickEvent>(muteButtonClicked);
            unmuteButton?.RegisterCallback<ClickEvent>(unmuteButtonClicked);
            sliderBackgroundMusic?.RegisterValueChangedCallback(sliderBackgroundMusicValueChanged);
            sliderSoundEffectMusic?.RegisterValueChangedCallback(sliderSoundEffectValueChanged);
        }

        public void RemoveCallBacks()
        {
            toggleFullscreen?.UnregisterValueChangedCallback(toggleFullscreenValueChanged);
            dropdownResolutions?.UnregisterValueChangedCallback(resolutionValueChanged);
            dropdownVSyncCount?.UnregisterValueChangedCallback(vSyncValueChanged);
            inputFieldFpsLimit?.UnregisterValueChangedCallback(fpsLimitValueChanged);
            dropdownQuality?.UnregisterValueChangedCallback(qualitySettingsChanged);
            muteButton?.UnregisterCallback<ClickEvent>(muteButtonClicked);
            unmuteButton?.UnregisterCallback<ClickEvent>(unmuteButtonClicked);
            sliderBackgroundMusic?.UnregisterValueChangedCallback(sliderBackgroundMusicValueChanged);
            sliderSoundEffectMusic?.UnregisterValueChangedCallback(sliderSoundEffectValueChanged);
        }

        public void RefreshGui()
        {
            RemoveCallBacks();
            RefreshOptions();
            InitCallBacks();
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

        private void masterVolumeValueChnaged(float value)
        {
            AudioMixer.SetFloat("Master_Volume", Mathf.Log10(value) * 20);
        }

        private void sliderBackgroundMusicValueChanged(ChangeEvent<float> evt)
        {
            AudioMixer.SetFloat("Music_Volume", Mathf.Log10(evt.newValue) * 20);
        }

        private void sliderSoundEffectValueChanged(ChangeEvent<float> evt)
        {
            AudioMixer.SetFloat("Sfx_Volume", Mathf.Log10(evt.newValue) * 20);
        }
        
        /*******Scrren & Graphics*****************/
        private void resolutionValueChanged(ChangeEvent<string> evt)
        {
            int index = dropdownResolutions.index;
            graphicSettings.SetCurrentScreen(ScreenResolutions[index]);
        }

        private void toggleFullscreenValueChanged(ChangeEvent<bool> evt)
        {
            graphicSettings.SetFullscreen(evt.newValue);
        }

        private void fpsLimitValueChanged(ChangeEvent<string> evt)
        {
            int limit;
            if (!int.TryParse(evt.newValue, out limit))
            {
                limit = -1;
            }
            graphicSettings.SetFrameLimit(limit);
        }

        private void vSyncValueChanged(ChangeEvent<string> evt)
        {
            graphicSettings.SetVsyncCount(dropdownVSyncCount.index);
        }


        private void qualitySettingsChanged(ChangeEvent<string> evt)
        {
            int vsyncCount = graphicSettings.GetVsyncCount();
            graphicSettings.SetQualitySettingLevel(dropdownQuality.index);
            graphicSettings.SetVsyncCount(vsyncCount);
        }


        private string Rest2String(Vector2Int vec)
        {
            return vec.x + " x " + vec.y;
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}