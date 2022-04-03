using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
namespace NorLib.SceneManagement
{
    [CustomPropertyDrawer(typeof(SceneNamePicker))]
    public class SceneNamePickerPropertyDrawer : PropertyDrawer
    {
        private int selectedIndex;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SceneNamePicker picker = (SceneNamePicker)attribute;

            List<string> validSceneNames = GetValidSceneNames(picker.showPath);
            int selectedIndex = Mathf.Max(validSceneNames.ToList().IndexOf(property.stringValue), 0);
            selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, ToSceneDisplayString(validSceneNames));
            property.stringValue = validSceneNames[selectedIndex];
        }

        private List<string> GetValidSceneNames(bool includePath)
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            List<string> sceneNames = new List<string>(new string[] { "undefined" });
            sceneNames.AddRange(EditorBuildSettingsScene.GetActiveSceneList(scenes));
            return sceneNames;
        }

        private string[] ToSceneDisplayString(List<string> paths)
        {
            var displaNames = new List<string>();

            foreach (var path in paths)
            {
                string sTrim = path.TrimEnd(".unity");
                sTrim = sTrim.TrimStart("Assets/");
                displaNames.Add(sTrim);
            }

            return displaNames.ToArray();
        }
    }
}
#endif