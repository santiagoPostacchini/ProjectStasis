using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    Vector3 lastMousePosition;


    public GameObject mouse;
    private float timerMouse;
    public GameObject arrows;
    private float timerarrows;
    public GameObject leftClick;
    public GameObject buttonWASD;
    public GameObject buttonW;
    public GameObject buttonS;
    public GameObject buttonA;
    public GameObject buttonD;
    private float timerW;
    private float timerS;
    private float timerA;
    private float timerD;
    public GameObject ctrl;
    private float timerCtrl;
    public GameObject space;
    private float timerSpace;
    public GameObject buttonE;
    private float timerE;

    public GameObject stasis;
    public float timerStasis;

    private float time = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        Activate(buttonWASD);
    }

    // Update is called once per frame
    void Update()
    {
        MoveMouse();

        if (Input.GetMouseButtonDown(0))
        {
            LeftClick();
        }

        if(!arrows.gameObject.activeSelf && !leftClick.gameObject.activeSelf)
        {
            timerMouse += Time.deltaTime;
            if(timerMouse > 4)
            {
                Desactivate(mouse);
                timerMouse = 0;
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (!buttonWASD.gameObject.activeSelf) return;
            timerW += Time.deltaTime;
            if (timerW > time)
            {
                Desactivate(buttonW);
                timerW = 0;
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (!buttonWASD.gameObject.activeSelf) return;
            timerS += Time.deltaTime;
            if (timerS > time)
            {
                Desactivate(buttonS);
                timerS = 0;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (!buttonWASD.gameObject.activeSelf) return;
            timerA += Time.deltaTime;
            if (timerA > time)
            {
                Desactivate(buttonA);
                timerA = 0;
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (!buttonWASD.gameObject.activeSelf) return;
            timerD += Time.deltaTime;
            if (timerD > time)
            {
                Desactivate(buttonD);
                timerD = 0;
            }
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (!buttonE.gameObject.activeSelf) return;
            Desactivate(buttonE);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!ctrl.gameObject.activeSelf) return;
            timerCtrl += Time.deltaTime;
            if (timerCtrl > time)
            {
                Desactivate(ctrl);
                timerCtrl = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!space.gameObject.activeSelf) return;
            Desactivate(space);
        }
    }

    public void Activate(GameObject gameObject)
    {
        if (gameObject == null) return;
        gameObject.SetActive(true);
    }
    public void Desactivate(GameObject gameObject)
    {
        if (gameObject == null) return;
        gameObject.SetActive(false);
    }
    
    public void MoveMouse()
    {
        if (!mouse.gameObject.activeSelf) return;
        if (!arrows.gameObject.activeSelf) return;
        if (Input.mousePosition != lastMousePosition)
        {
            lastMousePosition = Input.mousePosition;
            timerarrows += Time.deltaTime;
            if (timerarrows > 1) arrows.gameObject.SetActive(false);
        }
    }
    public void LeftClick()
    {
        if (leftClick.gameObject.activeSelf)
        {

            leftClick.gameObject.SetActive(false);
        }
        if (stasis.gameObject.activeSelf)
        {

            stasis.gameObject.SetActive(false);
        }
    }

}
public enum TypeUI
{
    ButtonWASD,
    ButtonE,
    ButtonCtrol,
    ButtonSpace,
    Mouse,
    LeftClick,
    Arrows,
    Stasis
}