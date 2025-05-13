// AudioManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sonidos de Ambiente")]
    public Sound[] ambientSounds;
    [Header("Sonidos SFX")]
    public Sound[] sfxSounds;

    private Dictionary<string, AudioSource> _sources = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
    public void PlayAmbient(string name)
    {
        if (_sources.TryGetValue(name, out var src)) src.Play();
        else Debug.LogWarning($"[AudioManager] No ambient sound '{name}'");
    }

    public void StopAmbient(string name)
    {
        if (_sources.TryGetValue(name, out var src)) src.Stop();
        else Debug.LogWarning($"[AudioManager] No ambient sound '{name}'");
    }

    public void FadeOutAmbient(string name, float duration)
    {
        if (_sources.TryGetValue(name, out var src))
            StartCoroutine(Fader.FadeOut(src, duration));
        else Debug.LogWarning($"[AudioManager] No ambient sound '{name}'");
    }

    // SFX
    public void PlaySfx(string name)
    {
        if (_sources.TryGetValue(name, out var src)) src.PlayOneShot(src.clip, src.volume);
        else Debug.LogWarning($"[AudioManager] No SFX '{name}'");
    }

    public void StopSfx(string name)
    {
        if (_sources.TryGetValue(name, out var src)) src.Stop();
        else Debug.LogWarning($"[AudioManager] No SFX '{name}'");
    }
}
