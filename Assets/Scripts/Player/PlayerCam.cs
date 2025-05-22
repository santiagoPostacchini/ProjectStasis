using UnityEngine;

namespace Player
{
    public class PlayerCam : MonoBehaviour
    {
        public float sens = 700f;

        public Transform orientation;

        float xRotation;
        float yRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
  

        private void Update()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;

            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -75f, 75f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
