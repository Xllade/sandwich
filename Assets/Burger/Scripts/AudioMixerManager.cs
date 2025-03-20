using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Burger
{
    public class AudioMixerManager: MonoBehaviour
    {
        [SerializeField] private AudioMixer _mixer;
        private float _bgmVolume = 0.5f;
        private float _sfxVolume = 1f;

        public void SetBGMVolume(float volume)
        {
            _bgmVolume = volume;
            _mixer.SetFloat("BGMVolume", volume == 0 ? -80 : Mathf.Log10(volume) * 20);
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = volume;
            _mixer.SetFloat("SFXVolume", volume == 0 ? -80 : Mathf.Log10(volume) * 20);
        }
    }
}