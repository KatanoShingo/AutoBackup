using System.Collections;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.SceneManagement;
using System;
using System.Linq;

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
                if (nextTime < EditorApplication.timeSinceStartup)
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
        public static bool IsAutoBackup
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
        public static int Interval
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
        public static int Quantity
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

        [SettingsProvider]
        public static SettingsProvider CreateAutoBackupProvider()
        {
            return new SettingsProvider("Preferences/Auto Backup", SettingsScope.User)
            {
                label = "Auto Backup",
                guiHandler = (searchContext) =>
                {
                    AutoBackup.IsAutoBackup = EditorGUILayout.BeginToggleGroup("auto backup", AutoBackup.IsAutoBackup);
                    EditorGUILayout.Space();
                    AutoBackup.Interval = EditorGUILayout.IntField("interval(min) mini5", AutoBackup.Interval);
                    AutoBackup.Quantity = EditorGUILayout.IntField("quantity      mini1", AutoBackup.Quantity);
                    EditorGUILayout.EndToggleGroup();
                }
            };
        }

        [MenuItem("File/Backup/Backup")]
        public static void Backup()
        {
            string scenePath = EditorSceneManager.GetActiveScene().path;

            if (string.IsNullOrEmpty(scenePath))
                return;

            string backupDir = "Backup";
            string exportPath = Path.Combine(
                backupDir,
                Path.GetFileNameWithoutExtension(scenePath) + DateTime.Now.ToString("MMddHHmm") + Path.GetExtension(scenePath)
            );

            Directory.CreateDirectory(backupDir);

            // バックアップ保存
            byte[] data = File.ReadAllBytes(scenePath);
            File.WriteAllBytes(exportPath, data);
            Debug.Log("バックアップ作成: " + exportPath);

            // バックアップの整理
            var files = new DirectoryInfo(backupDir)
                .GetFiles()
                .OrderBy(f => f.CreationTime)
                .ToArray();

            if (files.Length > Quantity)
            {
                int deleteCount = files.Length - Quantity;
                for (int i = 0; i < deleteCount; i++)
                {
                    try
                    {
                        files[i].Delete();
                        Debug.Log("古いバックアップ削除: " + files[i].Name);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("削除失敗: " + files[i].Name + "\n" + ex.Message);
                    }
                }
            }
        }

        [MenuItem("File/Backup/BackupRestore")]
        public static void BackupRestore()
        {
            string path = EditorUtility.OpenFilePanel("バックアップファイルを選択", "Backup", "unity");
            if (string.IsNullOrEmpty(path)) return;

            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            if (string.IsNullOrEmpty(currentScenePath))
            {
                EditorUtility.DisplayDialog("エラー", "現在のシーンを保存してから復元を行ってください。", "OK");
                return;
            }

            if (EditorUtility.DisplayDialog(
                "シーン復元の確認",
                "選択したバックアップファイルで現在のシーンを上書きします。\nこの操作は元に戻せません。\n\n本当に復元しますか？\n\n" + path,
                "はい、上書きする", "キャンセル"))
            {
                try
                {
                    File.Copy(path, currentScenePath, true);
                    AssetDatabase.Refresh();
                    EditorSceneManager.OpenScene(currentScenePath);
                    Debug.Log("バックアップからシーンを復元しました: " + path);
                }
                catch (Exception ex)
                {
                    Debug.LogError("バックアップの復元に失敗しました: " + ex.Message);
                    EditorUtility.DisplayDialog("エラー", "バックアップの復元に失敗しました:\n" + ex.Message, "OK");
                }
            }
        }
    }
}