using System.Collections;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.SceneManagement;
using System;

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
        
        private static readonly string autoBackupQuantity = "save scene quantity";
        static int Quantity
        {
            get
            {

                string value = EditorUserSettings.GetConfigValue(autoBackupQuantity);
                if (value == null)
                {
                    value = "1";
                }
                return int.Parse(value);
            }
            set
            {
                if (value < 1)
                    value = 1;
                EditorUserSettings.SetConfigValue(autoBackupQuantity, value.ToString());
            }
        }
        

        [PreferenceItem("Auto Backup")]
        static void ExampleOnGUI()
        {
            IsAutoBackup = EditorGUILayout.BeginToggleGroup("auto backup", IsAutoBackup);
            EditorGUILayout.Space();
            
            Interval = EditorGUILayout.IntField("interval(min) mini5", Interval);
            Quantity = EditorGUILayout.IntField("quantity      mini1", Quantity);
            EditorGUILayout.EndToggleGroup();
        }

        [MenuItem("File/Backup/Backup")]
        public static void Backup()
        {
            string expoertPath = "Backup/" + Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path) + DateTime.Now.ToString("MMddHHmm") + Path.GetExtension(EditorSceneManager.GetActiveScene().path);

            Directory.CreateDirectory(Path.GetDirectoryName(expoertPath));

            if (string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))
                return;

            byte[] data = File.ReadAllBytes(EditorSceneManager.GetActiveScene().path);
            File.WriteAllBytes(expoertPath, data);

            var files = Directory.GetFiles(Path.GetDirectoryName(expoertPath));
            if(files.Length <= Quantity)
            {
                return;
            }

            for (int i = 0; i < files.Length - Quantity; i++)
            {
                File.Delete(files[i]);
            }
        }

    }
}