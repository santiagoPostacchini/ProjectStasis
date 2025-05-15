using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensibilityUI : MonoBehaviour
{
    public Slider sensibilitySlider;
    public PlayerCam cameraController; // arrastralo desde el Inspector
    public bool isActive = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraController = GetComponent<PlayerCam>();
        sensibilitySlider.minValue = 0.5f;
        sensibilitySlider.maxValue = 700f;
        cameraController.sens = 500;
        //sensibilidadSlider.onValueChanged.AddListener(UpdateSensitivity);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isActive = !isActive;
            if (!isActive)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            sensibilitySlider.gameObject.SetActive(isActive);
        }
    }

    public void UpdateSensitivity(float value)
    {
        cameraController.sens = value;
    }
}
