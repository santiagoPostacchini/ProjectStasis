using System.Collections;
using UnityEngine;

public static class Fader
{
    public static IEnumerator FadeOut(AudioSource src, float time)
    {
        float startVol = src.volume;
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            src.volume = Mathf.Lerp(startVol, 0f, t / time);
            yield return null;
        }
        src.Stop();
        src.volume = startVol;
    }
}
