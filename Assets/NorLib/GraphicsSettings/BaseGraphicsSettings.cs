using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorLib
{
    public class BaseGraphicsSettings
    {

        public virtual bool IsFullscreen()
        {
            return Screen.fullScreen;
        }

        public virtual void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
        }


        public virtual string[] GetQualitySettingNames()
        {
            return QualitySettings.names;
        }

        public virtual int GetQualitySettingLevel()
        {
            return QualitySettings.GetQualityLevel();
        }

        public virtual void SetQualitySettingLevel(int level)
        {
            QualitySettings.SetQualityLevel(level);
        }

        public virtual Vector2Int[] GetScreenResolutions()
        {
            Resolution[] resols = Screen.resolutions;
            List<Vector2Int> resols_i = new List<Vector2Int>();

            for (int i = 0; i < resols.Length; i++)
            {
                Vector2Int resol_i = new Vector2Int(resols[i].width, resols[i].height);
                if (!resols_i.Contains(resol_i))
                {
                    resols_i.Add(resol_i);
                }
            }

            return resols_i.ToArray();
        }

        public virtual Vector2Int GetCurrentScreen()
        {
            if (IsFullscreen())
            {
                Resolution resolution = Screen.currentResolution;
                return new Vector2Int(resolution.width, resolution.height);
            }
            else
            {
                return new Vector2Int(Screen.width, Screen.height);
            }

        }

        public virtual void SetCurrentScreen(Vector2Int resolution)
        {
            Screen.SetResolution(resolution.x, resolution.y, IsFullscreen());
        }

        /// <summary>
        /// Sets the V-sync mode
        /// </summary>
        /// <param name="count">0: Vsync deactivated, fps limited by TargetFrameRate. 1-4: fps = MonitorRefrashRate / count</param>
        public virtual void SetVsyncCount(int count)
        {
            QualitySettings.vSyncCount = count;
        }

        public virtual int GetVsyncCount()
        {
            return QualitySettings.vSyncCount;
        }

        /// <summary>
        /// Limits frame rate if VsyncCount equals 0
        /// </summary>
        /// <param name="limit">-1: unlimited. >1: upper limit of frame rate </param>
        public virtual void SetFrameLimit(int limit)
        {
            Application.targetFrameRate = limit;
        }

        public virtual int GetFrameLimit()
        {
            return Application.targetFrameRate;
        }
    }
}