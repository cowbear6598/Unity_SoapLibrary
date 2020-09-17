using System;
using System.Collections;
using System.Collections.Generic;
using Soap.Sound;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManagerEditorWindow : EditorWindow
{
    private SerializedObject serializedObject;

    private int effectSoundDefaultCount = 10;
    [SerializeField] private int effectSoundCount = 10;

    [SerializeField] private AudioMixerGroup bgmAudioMixerGroup = null;
    private SerializedProperty bgmAudioMixerGroupProperty;

    [SerializeField] private AudioMixerGroup effectAudioMixerGroup = null;
    private SerializedProperty effectAudioMixerGroupProperty;
    
    [MenuItem("Soap/SoundSetting")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SoundManagerEditorWindow));
    }

    private void Awake()
    {
        serializedObject = new SerializedObject(this);

        bgmAudioMixerGroupProperty = serializedObject.FindProperty("bgmAudioMixerGroup");
        effectAudioMixerGroupProperty = serializedObject.FindProperty("effectAudioMixerGroup");
        
        effectSoundCount = PlayerPrefs.GetInt("SoundManager_EffectSoundCount") == 0 ? effectSoundDefaultCount : PlayerPrefs.GetInt("SoundManager_EffectSoundCount");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("設定音效數量與建立 Manager", EditorStyles.boldLabel);
        
        serializedObject.Update();
        
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(bgmAudioMixerGroupProperty,new GUIContent{text = "背景音樂混合"});
        EditorGUILayout.PropertyField(effectAudioMixerGroupProperty,new GUIContent{text = "音效混合"});
        
        effectSoundCount = EditorGUILayout.IntField("音效數量", effectSoundCount);
        
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("建立"))
        {
            Create();
        }
    }

    private void Create()
    {
        // 建立音效數量
        PlayerPrefs.SetInt("SoundManager_EffectSoundCount",effectSoundCount);
        PlayerPrefs.Save();

        if (SoundManager.Instance)
            DestroyImmediate(SoundManager.Instance.gameObject);
        
        for (int i = 0; i < effectSoundCount; i++)
        {
            GameObject effectSoundObj = new GameObject("EffectSound");
            
            AudioSource effectSound = effectSoundObj.AddComponent<AudioSource>();
            effectSound.playOnAwake = false;

            if (effectAudioMixerGroup)
            {
                effectSound.outputAudioMixerGroup = effectAudioMixerGroup;
            }
            
            effectSoundObj.transform.SetParent(SoundManager.Instance.transform);
            
            SoundManager.Instance.SetEffectSound(effectSound);
        }
        
        // 建立背景音樂
        GameObject bgmSoundObj = new GameObject("BGM");

        AudioSource bgmSound = bgmSoundObj.AddComponent<AudioSource>();
        bgmSound.playOnAwake = false;
        bgmSound.loop = true;

        if (bgmAudioMixerGroup)
        {
            bgmSound.outputAudioMixerGroup = bgmAudioMixerGroup;
        }
        
        bgmSoundObj.transform.SetParent(SoundManager.Instance.transform);

        SoundManager.Instance.SetBGMSound(bgmSound);
    }
}
