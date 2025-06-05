using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionObjects : MonoBehaviour
{
    public List<GameObject> firstList = new List<GameObject>();
    public List<GameObject> SecondList = new List<GameObject>();
    public List<GameObject> thirdList = new List<GameObject>();
    // Start is called before the first frame update
    public void GameObjectOn1()
    {
        foreach (var item in firstList)
        {
            item.gameObject.SetActive(true);
        }
    }
    public void GameObjectOn2()
    {
        foreach (var item in SecondList)
        {
            item.gameObject.SetActive(true);
        }
    }
    public void GameObjectOn3()
    {
        foreach (var item in thirdList)
        {
            item.gameObject.SetActive(true);
        }
    }
}
