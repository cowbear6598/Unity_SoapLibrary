using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using Soap.Update;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Soap.Sound
{
    public enum BGMTransitionType
    {
        Directly = 0,
        Fade = 1
    }
    
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        protected override bool IsNeedDontDestoryOnLoad => true;

        // 音效
        private int _sound2DIndex;
        public int sound2DIndex
        {
            get => _sound2DIndex;
            set => _sound2DIndex = value > effectSounds.Count - 1 ? 0 : value;
        }

        [SerializeField] private List<AudioSource> effectSounds = new List<AudioSource>();

        // 可以轉成協程的方式，不過 MEC 要用到接收 bool 的話需要 PRO 版，內建與 Update 效能測試中
        private Dictionary<AsyncOperationHandle<AudioClip>, List<AudioSource>> effectAsyncHandleDictionary = new Dictionary<AsyncOperationHandle<AudioClip>, List<AudioSource>>();
        
        // 音樂
        [SerializeField] private AudioSource bgmSound = null;
        private bool IsChangingBGM = false;

        private AsyncOperationHandle<AudioClip> bgmAsyncHandle;
        
        private void OnEnable()
        {
            if (UpdateManager.Instance)
                UpdateManager.Instance.UpdateEvent += UpdateEvent;
        }

        private void OnDisable()
        {
            if (UpdateManager.Instance)
                UpdateManager.Instance.UpdateEvent -= UpdateEvent;
        }

        private void UpdateEvent()
        {
            if(effectAsyncHandleDictionary.Count == 0) return;

            foreach (var _data in effectAsyncHandleDictionary)
            {
                if (_data.Value.Count > 0)
                {
                    for (int i = 0; i < _data.Value.Count; i++)
                    {
                        if (!_data.Value[i].isPlaying)
                        {
                            _data.Value.Remove(_data.Value[i]);
                        }
                    }
                }
                else
                {
                    Addressables.Release(_data.Key);
                    
                    if (effectAsyncHandleDictionary.ContainsKey(_data.Key))
                        effectAsyncHandleDictionary.Remove(_data.Key);
                }
            }
        }

        public void Play2D(AssetReferenceAudioClip _clip, float _volume = 1, float _pitch = 1)
        {
            AudioSource _sound = GetFreeEffectSound();

            if (_sound != null)
            {
                _sound.spatialBlend = 0.0f;
                _sound.volume = _volume;
                _sound.pitch = _pitch;

                Addressables.LoadAssetAsync<AudioClip>(_clip).Completed += _handle =>
                {
                    _sound.PlayOneShot(_handle.Result);
                  
                    Effect_AddReleaseAssetHandle(_handle, _sound);
                };
            }
        }

        public void Play3D(AssetReferenceAudioClip _clip, Vector3 _pos, float _volume = 1, float _pitch = 1)
        {
            AudioSource _sound = GetFreeEffectSound();

            if (_sound != null)
            {
                _sound.transform.position = _pos;

                _sound.spatialBlend = 1.0f;
                _sound.volume = _volume;
                _sound.pitch = _pitch;

                Addressables.LoadAssetAsync<AudioClip>(_clip).Completed += _handle =>
                {
                    _sound.PlayOneShot(_handle.Result);
                    
                    Effect_AddReleaseAssetHandle(_handle, _sound);
                };
            }
        }

        private void Effect_AddReleaseAssetHandle(AsyncOperationHandle<AudioClip> _handle,AudioSource _sound)
        {
            if (effectAsyncHandleDictionary.ContainsKey(_handle))
            {
                effectAsyncHandleDictionary[_handle].Add(_sound);
            }
            else
            {
                effectAsyncHandleDictionary.Add(_handle, new List<AudioSource>{_sound});
            }
        }
        
        public void PlayBGM(AssetReferenceAudioClip _clip, BGMTransitionType _type,float _fadeSpeed = 1)
        {
            if (IsChangingBGM)
            {
                Debug.LogWarning("BGM 正在切換中");
                return;
            }

            IsChangingBGM = true;

            Addressables.LoadAssetAsync<AudioClip>(_clip).Completed += _handle =>
            {
                switch (_type)
                {
                    case BGMTransitionType.Directly:
                        BGM_ReleaseAndSetAsset(_handle);
                        IsChangingBGM = false;
                        break;
                    case BGMTransitionType.Fade:
                        Timing.RunCoroutine(BGM_Fade(_handle,_fadeSpeed));
                        break;
                }
            };
        }

        private IEnumerator<float> BGM_Fade(AsyncOperationHandle<AudioClip> _clipHandle,float _fadeSpeed)
        {
            while (bgmSound.volume > 0)
            {
                bgmSound.volume -= Time.deltaTime * _fadeSpeed;
                yield return Timing.WaitForOneFrame;
            }

            BGM_ReleaseAndSetAsset(_clipHandle);
            
            while (bgmSound.volume < 1)
            {
                bgmSound.volume += Time.deltaTime * _fadeSpeed;
                yield return Timing.WaitForOneFrame;
            }

            IsChangingBGM = false;
        }

        private void BGM_ReleaseAndSetAsset(AsyncOperationHandle<AudioClip> _clipHandle)
        {
            if (bgmSound.clip != null)
            {
                bgmSound.Stop();
                bgmSound.clip = null;

                if (bgmAsyncHandle.Result != null)
                {
                    Addressables.Release(bgmAsyncHandle);
                }
            }
            
            bgmAsyncHandle = _clipHandle;
                        
            bgmSound.clip = _clipHandle.Result;
            bgmSound.Play();
        }

        private AudioSource GetFreeEffectSound()
        {
            AudioSource _sound = null;

            if (!effectSounds[sound2DIndex].isPlaying)
            {
                _sound = effectSounds[sound2DIndex];
            }
            else
            {
                bool IsAllSoundPlaying = true;
                
                for (int i = 0; i < effectSounds.Count; i++)
                {
                    if (!effectSounds[i].isPlaying)
                    {
                        IsAllSoundPlaying = false;
                        _sound = effectSounds[i];
                    }
                }

                if (IsAllSoundPlaying)
                {
                    _sound = CreateNewSound();
                }
            }

            sound2DIndex++;

            return _sound;
        }

        private AudioSource CreateNewSound()
        {
            Debug.LogWarning("音效播放不夠用，建立新的音效中");
            
            GameObject g = new GameObject("EffectSound");
            g.transform.SetParent(transform);

            AudioSource _sound = g.AddComponent<AudioSource>();

            effectSounds.Add(_sound);

            return _sound;
        }

        public void SetEffectSound(AudioSource _sound)
        {
            effectSounds.Add(_sound);
        }

        public void SetBGMSound(AudioSource _sound)
        {
            bgmSound = _sound;
        }
    }
}

[Serializable]
public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid) : base(guid){}
}
