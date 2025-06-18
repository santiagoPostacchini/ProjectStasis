using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanColisionDetector : MonoBehaviour
{
    public TimerScaler timerScaler;


    public void ReduceTimerScaler()
    {
        timerScaler.SetGameSpeed(0.05f);
    }
    public void ResetTimerScaler()
    {
        timerScaler.SetGameSpeed(1f);
    }
    
}
