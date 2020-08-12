using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Soap.Internet
{
    public class MysqlManagerEditorWindow : EditorWindow
    {
        private string fileName = "MysqlManagerSetting.asset";

        private string defalutDomainName = "127.0.0.1";
        private string domainName;

        [MenuItem("Soap/Internet/MysqlSetting")]
        public static void ShowWindow()
        {
            GetWindow(typeof(MysqlManagerEditorWindow));
        }

        private void Awake()
        {
            domainName = string.IsNullOrEmpty(PlayerPrefs.GetString("MysqlManager_Domain")) ? defalutDomainName : PlayerPrefs.GetString("MysqlManager_Domain");
        }

        private void OnGUI()
        {
            GUILayout.Label("初始化以及設定 Mysql 連線資訊", EditorStyles.boldLabel);

            domainName = EditorGUILayout.TextField("網域名 (domain): ", domainName);

            if (GUILayout.Button("初始化與設定"))
            {
                if (File.Exists("Assets/Resources/" + fileName))
                {
                    MysqlManagerScriptableObject mysqlManagerSO = (MysqlManagerScriptableObject) EditorGUIUtility.Load("Assets/Resources/" + fileName);

                    mysqlManagerSO.domainName = domainName;
                }
                else
                {
                    MysqlManagerScriptableObject asset = CreateInstance<MysqlManagerScriptableObject>();

                    asset.domainName = domainName;

                    if (AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateAsset(asset, "Assets/Resources/" + fileName);
                        AssetDatabase.SaveAssets();

                        EditorUtility.FocusProjectWindow();

                        Selection.activeObject = asset;
                    }
                    else
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");

                        AssetDatabase.CreateAsset(asset, "Assets/Resources/" + fileName);
                        AssetDatabase.SaveAssets();

                        EditorUtility.FocusProjectWindow();

                        Selection.activeObject = asset;
                    }
                }

                PlayerPrefs.SetString("MysqlManager_Domain", domainName);
                PlayerPrefs.Save();
            }
        }
    }
}