using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTutorial : MonoBehaviour
{
    public TypeUI typeUI;
    public Tutorial tutorial;
    public Dictionary<TypeUI, GameObject> DictionaryByTypeUI = new Dictionary<TypeUI, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        DictionaryByTypeUI.Add(TypeUI.ButtonWASD, tutorial.buttonWASD);
       

        DictionaryByTypeUI.Add(TypeUI.ButtonCtrol, tutorial.ctrl);
        DictionaryByTypeUI.Add(TypeUI.ButtonSpace, tutorial.space);
        DictionaryByTypeUI.Add(TypeUI.ButtonE, tutorial.buttonE);

        DictionaryByTypeUI.Add(TypeUI.Mouse, tutorial.mouse);
        DictionaryByTypeUI.Add(TypeUI.LeftClick, tutorial.leftClick);
        DictionaryByTypeUI.Add(TypeUI.Arrows, tutorial.arrows);

        DictionaryByTypeUI.Add(TypeUI.Stasis, tutorial.stasis);

    }

    private void OnTriggerEnter(Collider other)
    {
        Player.Player player = other.GetComponentInParent<Player.Player>();
        if(player != null)
        {
            if (DictionaryByTypeUI.TryGetValue(typeUI, out GameObject uiElement))
            {
                Debug.Log("Colisiono");
                tutorial.Activate(uiElement);
                Destroy(gameObject);
            }
        }
    }
}
