using UnityEngine;
using UnityEditor;
using System.Collections;

namespace AutoBackup
{
    public class SceneBackup : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            bool manualSave = AutoBackup.IsManualSave;
            if (manualSave)
            {
                AutoBackup.Backup();
            }

            return paths;
        }
    }
}