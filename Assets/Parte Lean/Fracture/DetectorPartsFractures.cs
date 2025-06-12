using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

public class DetectorPartsFractures : MonoBehaviour
{
    [SerializeField] private StasisGunEntity diosa1;
    [SerializeField] private StasisGunEntity diosa2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChooseGod(DestroyedPieceController part)
    {
        int randomValue = Random.Range(0, 2);
        int a = Random.Range(0, 6); // 0 es falso
        if (a < 4) return;
        if(randomValue == 0)
        {
            //diosa1.targets.Add(part.gameObject);
            
            diosa1.TryStasis(part.transform,part);
        }
        else
        {
            //diosa2.targets.Add(part.gameObject);
            diosa2.TryStasis(part.transform,part);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        DestroyedPieceController part = other.GetComponent<DestroyedPieceController>();
        if (part)
        {
            ChooseGod(part);
            Debug.Log("Agregado el elemento a la diosa");
            
        }
    }
}
