// Sound.cs
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Sound")]
public class Sound : ScriptableObject
{
    [Tooltip("Nombre único para referenciar este sonido")]
    public string soundName;

    [Tooltip("Clip de audio a reproducir")]
    public AudioClip clip;

    [Tooltip("Grupo del AudioMixer al que pertenecerá")]
    public AudioMixerGroup mixerGroup;

    [Tooltip("¿Se reproduce automáticamente al iniciar la escena?")]
    public bool playOnAwake = false;

    [Tooltip("¿Se repite en bucle?")]
    public bool loop = false;

    [Range(0f, 1f), Tooltip("Volumen inicial")]
    public float volume = 1f;
}
