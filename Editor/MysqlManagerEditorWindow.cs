using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Soap.Internet
{
    public class MysqlManagerEditorWindow : EditorWindow
    {
        private string fileName = "MysqlManagerSetting.asset";

        // 序列化對象
        private SerializedObject serializedObject;
        
        private string defalutDomainName = "127.0.0.1";

        [SerializeField]
        private List<string> domainList;
        private SerializedProperty domainListProperty;

        [MenuItem("Soap/Internet/MysqlSetting")]
        public static void ShowWindow()
        {
            GetWindow(typeof(MysqlManagerEditorWindow));
        }

        private void Awake()
        {
            // 將此類序列化
            serializedObject = new SerializedObject(this);

            // 獲取這類別可序列化的屬性
            domainList = new List<string>();
            
            domainListProperty = serializedObject.FindProperty("domainList");
            
            if (PlayerPrefs.GetInt("MysqlManager_DomainCount") > 0)
            {
                for (int i = 0; i < PlayerPrefs.GetInt("MysqlManager_DomainCount"); i++)
                {
                    domainList.Add(string.IsNullOrEmpty(PlayerPrefs.GetString("MysqlManager_Domain" + i)) ? defalutDomainName : PlayerPrefs.GetString("MysqlManager_Domain" + i));
                }
            }
            else
            {
                domainList.Add(defalutDomainName);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("初始化以及設定 Mysql 連線資訊", EditorStyles.boldLabel);

            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(domainListProperty, true);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
                
            if (GUILayout.Button("初始化與設定"))
            {
                CreateOrSetDomain();
            }
        }

        private void CreateOrSetDomain()
        {
            MysqlManagerScriptableObject mysqlManagerSO = null;
                
            if (File.Exists("Assets/Resources/" + fileName))
            { 
                mysqlManagerSO = (MysqlManagerScriptableObject) EditorGUIUtility.Load("Assets/Resources/" + fileName);

                mysqlManagerSO.domainList = domainList;
            }
            else
            {
                mysqlManagerSO = CreateInstance<MysqlManagerScriptableObject>();

                mysqlManagerSO.domainList = domainList;

                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                    
                AssetDatabase.CreateAsset(mysqlManagerSO, "Assets/Resources/" + fileName);
            }

            EditorUtility.FocusProjectWindow();
                
            Selection.activeObject = mysqlManagerSO;

            PlayerPrefs.SetInt("MysqlManager_DomainCount", domainList.Count);
            
            for (int i = 0; i < domainList.Count; i++)
            {
                PlayerPrefs.SetString("MysqlManager_Domain" + i, domainList[i]);
            }
            
            PlayerPrefs.Save();
                
            EditorUtility.SetDirty(mysqlManagerSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}