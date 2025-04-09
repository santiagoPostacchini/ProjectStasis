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
        cameraController = GetComponent<PlayerCam>();
        sensibilitySlider.minValue = 10f;
        sensibilitySlider.maxValue = 1000f;

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
