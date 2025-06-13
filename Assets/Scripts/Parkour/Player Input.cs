using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Controls")]
    public string forward;
    public string backward;
    public string right;
    public string left;
    public string jump;
    public string slowWalk;
    public string crouch;
    public string dash;

    [Header("Directional Keys")]
    public Vector2 moveInputDir;
    private int moveInputDirX = 0;
    private int moveInputDirZ = 0;
    public bool canMove = true;
    private PlayerController2 playerController;
    private void Start()
    {
        playerController = GetComponent<PlayerController2>();
    }

    public Vector2 InputDir()
    {
        moveInputDirX = 0;
        moveInputDirZ = 0;
        if (Input.GetKey(KeyCode.W)) { moveInputDirZ += 1; }
        if (Input.GetKey(KeyCode.S)) { moveInputDirZ -= 1; }
        moveInputDirZ = Mathf.Clamp(moveInputDirZ, -1, 1);

        if (Input.GetKey(KeyCode.D)) { moveInputDirX += 1; }
        if (Input.GetKey(KeyCode.A)) { moveInputDirX -= 1; }
        moveInputDirX = Mathf.Clamp(moveInputDirX, -1, 1);

        moveInputDir = new Vector2(moveInputDirX, moveInputDirZ);
        moveInputDir.Normalize();
        return moveInputDir;
    }
    public bool PressedJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerController.CanJump())
        {

            return true;
        }
        return false;
    }
    public bool PressedWalk()
    {
        if (Input.GetKey(slowWalk)) { return true; }
        return false;
    }
    public bool PressedCrouch()
    {
        if (Input.GetKey(KeyCode.C)) { return true; }
        return false;
    }
    public bool HoldDash()
    {
        if (Input.GetKey(KeyCode.LeftShift)) { return true; }
        return false;
    }
    public bool ReleasedDash()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift)) { return true; }
        return false;
    }
}
