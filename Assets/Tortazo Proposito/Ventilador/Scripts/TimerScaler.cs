using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerScaler : MonoBehaviour
{
    public void SetGameSpeed(float speedMultiplier)
    {
        Time.timeScale = speedMultiplier;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
    public void SetGameSpeed0()
    {
        SetGameSpeed(0.05f);
    }
}
