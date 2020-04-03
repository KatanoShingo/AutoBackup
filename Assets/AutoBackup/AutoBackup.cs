using System.Collections;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.SceneManagement;

namespace AutoBackup
{
    [InitializeOnLoad]
    public class AutoBackup
    {

        static double nextTime = 0;

        static AutoBackup()
        {

            nextTime = EditorApplication.timeSinceStartup + Interval * 60;
            EditorApplication.update += () =>
            {
                if ( nextTime < EditorApplication.timeSinceStartup)
                {
                    nextTime = EditorApplication.timeSinceStartup + Interval * 60;

                    if (IsAutoBackup && !EditorApplication.isPlaying)
                    {
                        Debug.Log("backup scene " + System.DateTime.Now);
                        Backup();
                    }
                }
            };
        }
        


        private static readonly string autoBackup = "auto backup";
        static bool IsAutoBackup
        {
            get
            {
                string value = EditorUserSettings.GetConfigValue(autoBackup);
                return !string.IsNullOrEmpty(value) && value.Equals("True");
            }
            set
            {
                EditorUserSettings.SetConfigValue(autoBackup, value.ToString());
            }
        }

        private static readonly string autoBackupInterval = "save scene interval";
        static int Interval
        {
            get
            {

                string value = EditorUserSettings.GetConfigValue(autoBackupInterval);
                if (value == null)
                {
                    value = "5";
                }
                return int.Parse(value);
            }
            set
            {
                if (value < 5)
                    value = 5;
                EditorUserSettings.SetConfigValue(autoBackupInterval, value.ToString());
            }
        }


        [PreferenceItem("Auto Backup")]
        static void ExampleOnGUI()
        {
            IsAutoBackup = EditorGUILayout.BeginToggleGroup("auto backup", IsAutoBackup);
            EditorGUILayout.Space();
            
            Interval = EditorGUILayout.IntField("interval(min) mini5min", Interval);
            EditorGUILayout.EndToggleGroup();
        }

        [MenuItem("File/Backup/Backup")]
        public static void Backup()
        {
            string expoertPath = "Backup/" + EditorSceneManager.GetActiveScene().path;

            Directory.CreateDirectory(Path.GetDirectoryName(expoertPath));

            if (string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))
                return;

            byte[] data = File.ReadAllBytes(EditorSceneManager.GetActiveScene().path);
            File.WriteAllBytes(expoertPath, data);
        }

        [MenuItem("File/Backup/Rollback")]
        public static void RollBack()
        {
            string expoertPath = "Backup/" + EditorSceneManager.GetActiveScene().path;

            byte[] data = File.ReadAllBytes(expoertPath);
            File.WriteAllBytes(EditorSceneManager.GetActiveScene().path, data);
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }

    }
}