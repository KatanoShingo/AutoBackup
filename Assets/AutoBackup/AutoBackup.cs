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
        static bool isChangedHierarchy = false;

        static AutoBackup()
        {
            IsManualSave = true;
            EditorApplication.playModeStateChanged += (state) =>
            {
                if (IsAutoBackup && !EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
                {

                    IsManualSave = false;

                    if (IsSavePrefab)
                        AssetDatabase.SaveAssets();
                    if (IsSaveScene)
                    {
                        Debug.Log("save scene " + System.DateTime.Now);
                        EditorSceneManager.SaveOpenScenes();
                    }
                    IsManualSave = true;
                }
                isChangedHierarchy = false;
            };

            nextTime = EditorApplication.timeSinceStartup + Interval;
            EditorApplication.update += () =>
            {
                if (isChangedHierarchy && nextTime < EditorApplication.timeSinceStartup)
                {
                    nextTime = EditorApplication.timeSinceStartup + Interval;

                    IsManualSave = false;

                    if (IsSaveSceneTimer && IsAutoBackup && !EditorApplication.isPlaying)
                    {
                        if (IsSavePrefab)
                            AssetDatabase.SaveAssets();
                        if (IsSaveScene)
                        {
                            Debug.Log("save scene " + System.DateTime.Now);
                            EditorSceneManager.SaveOpenScenes();
                        }
                    }
                    isChangedHierarchy = false;
                    IsManualSave = true;
                }
            };

            EditorApplication.hierarchyChanged += () =>
            {
                if (!EditorApplication.isPlaying)
                    isChangedHierarchy = true;
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

        private static readonly string autoBackupPrefab = "auto backup prefab";
        static bool IsSavePrefab
        {
            get
            {
                string value = EditorUserSettings.GetConfigValue(autoBackupPrefab);
                return !string.IsNullOrEmpty(value) && value.Equals("True");
            }
            set
            {
                EditorUserSettings.SetConfigValue(autoBackupPrefab, value.ToString());
            }
        }

        private static readonly string autoBackupScene = "auto save scene";
        static bool IsSaveScene
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

        private static readonly string autoBackupSceneTimer = "auto save scene timer";
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


        [PreferenceItem("Auto Save")]
        static void ExampleOnGUI()
        {
            bool isAutoBackup = EditorGUILayout.BeginToggleGroup("auto save", IsAutoBackup);

            IsAutoBackup = isAutoBackup;
            EditorGUILayout.Space();

            IsSavePrefab = EditorGUILayout.ToggleLeft("save prefab", IsSavePrefab);
            IsSaveScene = EditorGUILayout.ToggleLeft("save scene", IsSaveScene);
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