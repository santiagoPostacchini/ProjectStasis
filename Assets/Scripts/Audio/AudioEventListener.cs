using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioEventListener : MonoBehaviour
    {
        [Serializable]
        public struct SoundDelayPair
        {
            public string soundName;
            public float delay;
        }

        [Serializable]
        public struct SoundEventGroup
        {
            public string eventName;
            public SoundDelayPair[] soundDelays;
            public bool isStopEvent;
        }

        [Tooltip("Define aquí grupos por evento; marca Stop para los que deban parar.")]
        public SoundEventGroup[] soundEventGroups;

        private AudioSource _audioSource;
        private Dictionary<string, SoundEventGroup> _groups;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            // Asegurarse de que al comenzar NO hay nada sonando:
            _audioSource.Stop();
            _audioSource.clip = null;
            _audioSource.loop = false;
            _groups = new Dictionary<string, SoundEventGroup>();
            foreach (var g in soundEventGroups)
                _groups[g.eventName] = g;
        }

        private void OnEnable()
        {
            foreach (var g in soundEventGroups)
                EventManager.Subscribe(g.eventName, HandleEvent);
        }

        private void OnDisable()
        {
            foreach (var g in soundEventGroups)
                EventManager.Unsubscribe(g.eventName, HandleEvent);
        }

        private void HandleEvent(string eventName, GameObject sender)
        {
            if (!_groups.TryGetValue(eventName, out var group)) return;

            if (group.isStopEvent)
            {
                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                    _audioSource.loop = false;
                    _audioSource.clip = null;
                }

                foreach (var sd in group.soundDelays)
                {
                    var s = AudioManager.Instance.GetSfx(sd.soundName);
                    if (s && _audioSource.clip == s.clip && _audioSource.isPlaying)
                    {
                        _audioSource.Stop();
                        _audioSource.loop = false;
                    }
                }
            }
            else
            {
                StartCoroutine(PlayGroup(group));
            }
        }

        private IEnumerator PlayGroup(SoundEventGroup group)
        {
            foreach (var sd in group.soundDelays)
            {
                if (sd.delay > 0f)
                    yield return new WaitForSeconds(sd.delay);

                var s = AudioManager.Instance.GetSfx(sd.soundName);
                if (!s) continue;

                if (s.loop)
                {
                    _audioSource.clip = s.clip;
                    _audioSource.loop = true;
                    _audioSource.volume = s.volume;
                    _audioSource.Play();
                }
                else
                {
                    _audioSource.PlayOneShot(s.clip, s.volume);
                }
            }
        }
    }
}