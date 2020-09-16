using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    // 是否需要使用 DontDestroyOnLoad
    protected abstract bool IsNeedDontDestoryOnLoad { get; }
    
    // 確認是否應用程式關閉或程式碼削除
    protected static bool IsShuttingDown = false;

    private static T instance;
    
    // 防止多執行續讀取使用
    private static object objectLock;

    private static readonly Type instanceType = typeof(T);
    
    public static T Instance
    {
        get
        {
            if (IsShuttingDown)
            {
                Debug.LogWarning("讀取 " + instanceType + " 錯誤，因為正在削除此程式碼");
                return null;
            }
            
            if(objectLock == null)
                objectLock = new object();

            lock (objectLock)
            {
                if (instance != null) return instance;

                instance = FindObjectOfType<T>();

                if (instance != null) return instance;

                instance = new GameObject(instanceType.Name).AddComponent(instanceType) as T;

                SingletonMonoBehaviour<T> singleton = instance as SingletonMonoBehaviour<T>;

                if (singleton != null && singleton.IsNeedDontDestoryOnLoad && !Application.isEditor)
                    DontDestroyOnLoad(instance);

                return instance;
            }
        }
    }

    private void OnApplicationQuit()
    {
        IsShuttingDown = true;
    }

    private void OnDestroy()
    {
        IsShuttingDown = true;
    }
}
