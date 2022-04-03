using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
namespace NorLib
{
    public class BlenderImporter : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            if (assetPath.EndsWith(".blend"))
            {
                //General settings you want on every .blend file
                var importer = assetImporter as ModelImporter;
                if (importer)
                {
                    importer.importCameras = false;
                    importer.importLights = false;
                    importer.bakeAxisConversion = true; //fixes axis conversion
                }
            }
        }
    }
}
#endif