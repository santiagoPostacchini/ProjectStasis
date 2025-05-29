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
        
        private readonly Dictionary<string, Sound> _sfxDict = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                RegisterSfx(sfxSounds);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void RegisterSfx(Sound[] sounds)
        {
            foreach (var s in sounds)
            {
                if (!_sfxDict.ContainsKey(s.soundName))
                    _sfxDict[s.soundName] = s;
                else
                    Debug.LogWarning($"[AudioManager] Duplicate SFX name: {s.soundName}");
            }
        }
        
        public Sound GetSfx(string soundName)
        {
            _sfxDict.TryGetValue(soundName, out var s);
            if (s == null)
                Debug.LogWarning($"[AudioManager] No SFX '{soundName}'");
            return s;
        }
    }
}