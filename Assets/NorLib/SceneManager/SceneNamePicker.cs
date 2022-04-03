using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorLib.SceneManagement
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneNamePicker : PropertyAttribute
    {
        public bool showPath = true;

        public SceneNamePicker() { }

        public SceneNamePicker(bool showPath)
        {
            this.showPath = showPath;
        }
    }
}