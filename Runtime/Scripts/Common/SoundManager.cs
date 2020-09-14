using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        
        // 音樂
        [SerializeField] private AudioSource bgmSound = null;
        private bool IsChangingBGM = false;
        
        public void Play2D(AssetReferenceAudioClip _clip, float _volume = 1, float _pitch = 1)
        {
            AudioSource sound = GetFreeEffectSound();

            if (sound != null)
            {
                sound.spatialBlend = 0.0f;
                sound.volume = _volume;
                sound.pitch = _pitch;

                Addressables.LoadAssetAsync<AudioClip>(_clip).Completed += _handle =>
                {
                    sound.PlayOneShot(_handle.Result);
                    Addressables.Release(_handle);
                };
            }
        }

        public void Play3D(AssetReferenceAudioClip _clip, Vector3 _pos, float _volume = 1, float _pitch = 1)
        {
            AudioSource sound = GetFreeEffectSound();

            if (sound != null)
            {
                sound.transform.position = _pos;

                sound.spatialBlend = 1.0f;
                sound.volume = _volume;
                sound.pitch = _pitch;

                Addressables.LoadAssetAsync<AudioClip>(_clip).Completed += _handle =>
                {
                    sound.PlayOneShot(_handle.Result);
                    Addressables.Release(_handle);
                };
            }
        }

        public void PlayBGM(AssetReferenceAudioClip _clip, BGMTransitionType _type)
        {
            if (IsChangingBGM)
            {
                Debug.LogWarning("BGM 正在切換中");
                return;
            }

            IsChangingBGM = true;
            
            switch (_type)
            {
                case BGMTransitionType.Directly:
                    bgmSound.Stop();
                    break;
                case BGMTransitionType.Fade:
                    break;
            }
        }

        private IEnumerator<float> BGM_Fade(AudioClip _clip)
        {
            
            
            yield return Timing.WaitForOneFrame;
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
