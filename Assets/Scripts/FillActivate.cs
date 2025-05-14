using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillActivate : MonoBehaviour
{

    [SerializeField] private List<GameObject> list = new List<GameObject>();
    
    public void Activate()
    {
        foreach (var item in list)
        {
            item.gameObject.SetActive(true);
        }
    }
    public void Desactivate()
    {
        foreach (var item in list)
        {
            if (item.gameObject.activeSelf)
                item.gameObject.SetActive(false);
        }
    }
}
