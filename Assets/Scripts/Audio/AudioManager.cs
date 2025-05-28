// AudioManager.cs

using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sonidos de Ambiente")]
        public Sound[] ambientSounds;
        [Header("Sonidos SFX")]
        public Sound[] sfxSounds;

        private readonly Dictionary<string, AudioSource> _sources = new Dictionary<string, AudioSource>();

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(transform.root.gameObject);
                RegisterSounds(ambientSounds);
                RegisterSounds(sfxSounds);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void RegisterSounds(Sound[] sounds)
        {
            foreach (var s in sounds)
            {
                var src = gameObject.AddComponent<AudioSource>();
                src.clip = s.clip;
                src.outputAudioMixerGroup = s.mixerGroup;
                src.loop = s.loop;
                src.volume = s.volume;
                src.playOnAwake = false;
                _sources[s.soundName] = src;

                if (s.playOnAwake)
                    src.Play();
            }
        }

        // Ambiente
        public void PlayAmbient(string vfxName)
        {
            if (_sources.TryGetValue(name, out var src)) src.Play();
            else Debug.LogWarning($"[AudioManager] No ambient sound '{vfxName}'");
        }

        public void StopAmbient(string vfxName)
        {
            if (_sources.TryGetValue(vfxName, out var src)) src.Stop();
            else Debug.LogWarning($"[AudioManager] No ambient sound '{vfxName}'");
        }

        public void FadeOutAmbient(string vfxName, float duration)
        {
            if (_sources.TryGetValue(vfxName, out var src))
                StartCoroutine(Fader.FadeOut(src, duration));
            else Debug.LogWarning($"[AudioManager] No ambient sound '{vfxName}'");
        }

        // SFX
        public void PlaySfx(string vfxName)
        {
            if (_sources.TryGetValue(vfxName, out var src)) src.PlayOneShot(src.clip, src.volume);
            else Debug.LogWarning($"[AudioManager] No SFX '{vfxName}'");
        }
    
        public void PlaySfxOnObject(string vfxName, AudioSource source)
        {
            if (_sources.TryGetValue(vfxName, out var src)) 
            {
                source.clip = src.clip;
                source.volume = src.volume;
                source.PlayOneShot(source.clip, source.volume);
            }
            else Debug.LogWarning($"[AudioManager] No SFX '{vfxName}'");
        }
        
        public void StopSfxOnObject(AudioSource source)
        {
            source.Stop();
        }
    
        public void StopSfx(string vfxName)
        {
            if (_sources.TryGetValue(vfxName, out var src)) src.Stop();
            else Debug.LogWarning($"[AudioManager] No SFX '{vfxName}'");
        }
    }
}
