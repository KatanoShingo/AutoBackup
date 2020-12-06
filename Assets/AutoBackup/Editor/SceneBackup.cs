using UnityEngine;
using UnityEditor;
using System.Collections;

namespace AutoBackup
{
    public class SceneBackup : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            AutoBackup.Backup();

            return paths;
        }
    }
}