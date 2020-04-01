using UnityEngine;
using UnityEditor;
using System.Collections;


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