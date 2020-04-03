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
        public static readonly string manualSaveKey = "autobackup@manualSave";

        static double nextTime = 0;

        static AutoBackup()
        {
            IsManualSave = true;

            nextTime = EditorApplication.timeSinceStartup + Interval;
            EditorApplication.update += () =>
            {
                if ( nextTime < EditorApplication.timeSinceStartup)
                {
                    nextTime = EditorApplication.timeSinceStartup + Interval;

                    IsManualSave = false;

                    if (IsSaveSceneTimer && IsAutoBackup && !EditorApplication.isPlaying)
                    {
                        if (IsBackupScene)
                        {
                            Debug.Log("backup scene " + System.DateTime.Now);
                            Backup();
                        }
                    }
                    IsManualSave = true;
                }
            };
        }

        public static bool IsManualSave
        {
            get
            {
                return EditorPrefs.GetBool(manualSaveKey);
            }
            private set
            {
                EditorPrefs.SetBool(manualSaveKey, value);
            }
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


        private static readonly string autoBackupScene = "auto backup scene";
        static bool IsBackupScene
        {
            get
            {
                string value = EditorUserSettings.GetConfigValue(autoBackupScene);
                return !string.IsNullOrEmpty(value) && value.Equals("True");
            }
            set
            {
                EditorUserSettings.SetConfigValue(autoBackupScene, value.ToString());
            }
        }

        private static readonly string autoBackupSceneTimer = "auto backup scene timer";
        static bool IsSaveSceneTimer
        {
            get
            {
                string value = EditorUserSettings.GetConfigValue(autoBackupSceneTimer);
                return !string.IsNullOrEmpty(value) && value.Equals("True");
            }
            set
            {
                EditorUserSettings.SetConfigValue(autoBackupSceneTimer, value.ToString());
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
                    value = "60";
                }
                return int.Parse(value);
            }
            set
            {
                if (value < 60)
                    value = 60;
                EditorUserSettings.SetConfigValue(autoBackupInterval, value.ToString());
            }
        }


        [PreferenceItem("Auto Backup")]
        static void ExampleOnGUI()
        {
            bool isAutoBackup = EditorGUILayout.BeginToggleGroup("auto backup", IsAutoBackup);

            IsAutoBackup = isAutoBackup;
            EditorGUILayout.Space();
            
            IsBackupScene = EditorGUILayout.ToggleLeft("backup scene", IsBackupScene);
            IsSaveSceneTimer = EditorGUILayout.BeginToggleGroup("save scene interval", IsSaveSceneTimer);
            Interval = EditorGUILayout.IntField("interval(sec) min60sec", Interval);
            EditorGUILayout.EndToggleGroup();
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