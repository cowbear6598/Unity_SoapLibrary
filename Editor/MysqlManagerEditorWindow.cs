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
                MysqlManagerScriptableObject mysqlManagerSO = null;
                
                if (File.Exists("Assets/Resources/" + fileName))
                { 
                    mysqlManagerSO = (MysqlManagerScriptableObject) EditorGUIUtility.Load("Assets/Resources/" + fileName);

                    mysqlManagerSO.domainName = domainName;
                }
                else
                {
                    mysqlManagerSO = CreateInstance<MysqlManagerScriptableObject>();

                    mysqlManagerSO.domainName = domainName;

                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }
                    
                    AssetDatabase.CreateAsset(mysqlManagerSO, "Assets/Resources/" + fileName);
                }

                EditorUtility.FocusProjectWindow();
                
                Selection.activeObject = mysqlManagerSO;
                
                PlayerPrefs.SetString("MysqlManager_Domain", domainName);
                PlayerPrefs.Save();
                
                EditorUtility.SetDirty(mysqlManagerSO);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}