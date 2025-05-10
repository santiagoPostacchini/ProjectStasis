// AudioManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sonidos de Ambiente")]
    public Sound[] ambientSounds;

    // (En el futuro puedes a�adir arrays para m�sica, SFX, etc.)

    private Dictionary<string, AudioSource> _sources = new Dictionary<string, AudioSource>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            RegisterSounds(ambientSounds);
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
            // A�adimos un AudioSource nuevo por cada Sound
            var src = gameObject.AddComponent<AudioSource>();
            src.clip = s.clip;
            src.outputAudioMixerGroup = s.mixerGroup;
            src.loop = s.loop;
            src.volume = s.volume;
            src.playOnAwake = false;        // Siempre false aqu�
            _sources[s.soundName] = src;

            // Si el asset pide reproducir en Awake, lo lanzamos manualmente
            if (s.playOnAwake)
                src.Play();
        }
    }

    // M�todos para Ambiente
    public void PlayAmbient(string name)
    {
        if (_sources.TryGetValue(name, out var src))
            src.Play();
        else
            Debug.LogWarning($"[AudioManager] No existe ambient sound '{name}'");
    }

    public void StopAmbient(string name)
    {
        if (_sources.TryGetValue(name, out var src))
            src.Stop();
    }

    public void FadeOutAmbient(string name, float duration)
    {
        if (_sources.TryGetValue(name, out var src))
            StartCoroutine(Fader.FadeOut(src, duration));
    }

    // Aqu� podr�as a�adir PlayMusic, PlaySfx, etc.
}
