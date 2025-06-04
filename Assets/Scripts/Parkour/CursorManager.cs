using UnityEngine;

public class CursorManager : MonoBehaviour
{
    void Start()
    {
        LockCursor(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            LockCursor(false);
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
            LockCursor(true);
    }


    public void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked; // bloquea y centra
            Cursor.visible = false;                 // lo oculta
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;   // lo libera
            Cursor.visible = true;                  // lo muestra
        }
    }
}
