using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stasis.Parkour;

/// <summary>
/// PlayerController gestiona todos los movimientos de parkour:
/// – Vault (salto sobre obstáculos)
/// – Climb (escalada de bordes)
/// – Wallrun (carrera por paredes) y walljump (salto desde la pared)
/// Coordina los sensores DetectObs, el controlador RigidbodyFirstPersonController (rbfps)
/// y el Rigidbody para ejecutar física y animaciones de cámara.
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Configuración de Drag
    [Header("Configuración de Drag")]
    public float drag_grounded;    // Resistencia cuando el jugador está en el suelo
    public float drag_inair;       // Resistencia cuando está en el aire
    public float drag_wallrun;     // Resistencia durante el wallrun
    #endregion

    #region Sensores de Parkour
    [Header("Sensores de Parkour")]
    public DetectObs detectVaultObject;       // Detecta objeto para vault
    public DetectObs detectVaultObstruction;  // Detecta si hay obstáculo que impida vault
    public DetectObs detectClimbObject;       // Detecta objeto para climb
    public DetectObs detectClimbObstruction;  // Detecta si hay obstáculo que impida climb

    public DetectObs DetectWallL;  // Detecta pared a la izquierda
    public DetectObs DetectWallR;  // Detecta pared a la derecha
    #endregion

    #region Animaciones de Cámara
    [Header("Animator de Cámara")]
    public Animator cameraAnimator; // Controla animaciones de vault, climb y tilt de wallrun
    #endregion

    #region Parámetros de Wallrun
    [Header("Parámetros de Wallrun")]
    public float WallRunUpForce;              // Fuerza inicial hacia arriba en wallrun
    public float WallRunUpForce_DecreaseRate; // Ritmo de disminución de la fuerza
    private float upforce;                    // Fuerza actual durante el wallrun
    #endregion

    #region Parámetros de Walljump
    [Header("Parámetros de Walljump")]
    public float WallJumpUpVelocity;       // Velocidad vertical al saltar desde la pared
    public float WallJumpForwardVelocity;  // Velocidad horizontal al saltar desde la pared
    #endregion

    #region Estados de Parkour
    private bool WallRunning;        // True mientras se realiza wallrun
    private bool WallrunningLeft;    // True si wallrun en pared izquierda
    private bool WallrunningRight;   // True si wallrun en pared derecha
    private bool canwallrun;         // Permite un solo wallrun antes de aterrizar

    private bool IsParkour;          // True durante la interpolación de vault o climb
    #endregion

    #region Ajustes de Vault y Climb
    [Header("Ajustes de Vault y Climb")]
    private bool CanVault;           // Marca para iniciar vault
    public float VaultTime;          // Duración del vault
    public Transform VaultEndPoint;  // Punto final del vault

    private bool CanClimb;           // Marca para iniciar climb
    public float ClimbTime;          // Duración del climb
    public Transform ClimbEndPoint;  // Punto final del climb
    #endregion

    #region Referencias Internas e Interpolación
    private RigidbodyFirstPersonController rbfps;  // Controlador de movimiento
    private Rigidbody rb;                          // Componente Rigidbody
    private Vector3 RecordedStartPosition;         // Posición inicial del movimiento
    private Vector3 RecordedMoveToPosition;        // Posición final del movimiento
    private float t_parkour;                       // Valor normalizado (0–1) de la interpolación
    private float chosenParkourMoveTime;           // Duración seleccionada (vault o climb)
    #endregion

    /// <summary>
    /// Al iniciar, cachear referencias y permitir wallrun.
    /// </summary>
    void Start()
    {
        rbfps = GetComponent<RigidbodyFirstPersonController>();
        rb = GetComponent<Rigidbody>();
        canwallrun = true; // Permite un wallrun al comenzar
    }

    /// <summary>
    /// Ejecutado cada frame:
    /// 1. Actualiza drag según estado (suelo, aire, wallrun).
    /// 2. Comprueba inicio de vault.
    /// 3. Comprueba inicio de climb.
    /// 4. Realiza interpolación de vault/climb.
    /// 5. Detecta e inicia wallrun.
    /// 6. Ejecuta física del wallrun y walljump.
    /// </summary>
    void Update()
    {
        HandleDragAndGroundReset();
        TryInitiateVault();
        TryInitiateClimb();
        PerformParkourMovement();
        HandleWallrunDetection();
        HandleWallrunMovement();
    }

    #region Drag y Reinicio de Wallrun
    /// <summary>
    /// Ajusta el drag y resetea la posibilidad de wallrun al aterrizar.
    /// </summary>
    private void HandleDragAndGroundReset()
    {
        if (rbfps.Grounded)
        {
            rb.drag = drag_grounded;
            canwallrun = true; // Resetea wallrun
        }
        else
        {
            rb.drag = drag_inair;
        }
        if (WallRunning)
            rb.drag = drag_wallrun;
    }
    #endregion

    #region Lógica de Vault
    /// <summary>
    /// Comprueba sensores e input para activar vault.
    /// </summary>
    private void TryInitiateVault()
    {
        bool forward = Input.GetAxisRaw("Vertical") > 0f;
        bool jumpOrAir = Input.GetKey(KeyCode.Space) || !rbfps.Grounded;

        if (detectVaultObject.Obstruction &&
            !detectVaultObstruction.Obstruction &&
            !CanVault && !IsParkour && !WallRunning &&
            jumpOrAir && forward)
        {
            CanVault = true;
        }

        if (CanVault)
        {
            StartParkourMove(VaultEndPoint.position, VaultTime, "Vault");
            CanVault = false;
        }
    }
    #endregion

    #region Lógica de Climb
    /// <summary>
    /// Comprueba sensores e input para activar climb.
    /// </summary>
    private void TryInitiateClimb()
    {
        bool forward = Input.GetAxisRaw("Vertical") > 0f;
        bool jumpOrAir = Input.GetKey(KeyCode.Space) || !rbfps.Grounded;

        if (detectClimbObject.Obstruction &&
            !detectClimbObstruction.Obstruction &&
            !CanClimb && !IsParkour && !WallRunning &&
            jumpOrAir && forward)
        {
            CanClimb = true;
        }

        if (CanClimb)
        {
            StartParkourMove(ClimbEndPoint.position, ClimbTime, "Climb");
            CanClimb = false;
        }
    }
    #endregion

    #region Interpolación de Vault/Climb
    /// <summary>
    /// Inicia parámetros para mover al jugador sin física.
    /// </summary>
    private void StartParkourMove(Vector3 endPoint, float duration, string animTrigger)
    {
        rb.isKinematic = true;
        RecordedStartPosition = transform.position;
        RecordedMoveToPosition = endPoint;
        chosenParkourMoveTime = duration;
        t_parkour = 0f;
        IsParkour = true;
        cameraAnimator.CrossFade(animTrigger, 0.1f);
    }

    /// <summary>
    /// Realiza Lerp entre la posición inicial y final.
    /// Vuelve a activar física al acabar.
    /// </summary>
    private void PerformParkourMovement()
    {
        if (!IsParkour) return;

        t_parkour += Time.deltaTime / chosenParkourMoveTime;
        transform.position = Vector3.Lerp(
            RecordedStartPosition,
            RecordedMoveToPosition,
            t_parkour
        );

        if (t_parkour >= 1f)
        {
            IsParkour = false;
            rb.isKinematic = false; // Reactiva física
        }
    }
    #endregion

    #region Detección de Wallrun
    /// <summary>
    /// Detecta inicio de wallrun en pared izquierda o derecha.
    /// Cancela si falla el input o se pierde velocidad.
    /// </summary>
    private void HandleWallrunDetection()
    {
        bool forward = Input.GetAxisRaw("Vertical") > 0f;
        bool hasSpeed = rbfps.relativevelocity.magnitude >= 1f;

        // Iniciar wallrun izquierda
        if (DetectWallL.Obstruction && !rbfps.Grounded && !IsParkour && canwallrun)
        {
            WallrunningLeft = true;
            canwallrun = false;
            upforce = WallRunUpForce;
        }
        // Iniciar wallrun derecha
        if (DetectWallR.Obstruction && !rbfps.Grounded && !IsParkour && canwallrun)
        {
            WallrunningRight = true;
            canwallrun = false;
            upforce = WallRunUpForce;
        }

        // Cancelar si ya no cumple condiciones
        if ((WallrunningLeft && (!DetectWallL.Obstruction || !forward || !hasSpeed)) ||
            (WallrunningRight && (!DetectWallR.Obstruction || !forward || !hasSpeed)))
        {
            WallrunningLeft = WallrunningRight = false;
        }

        WallRunning = WallrunningLeft || WallrunningRight;
        rbfps.Wallrunning = WallRunning;

        cameraAnimator.SetBool("WallLeft", WallrunningLeft);
        cameraAnimator.SetBool("WallRight", WallrunningRight);
    }
    #endregion

    #region Movimiento y Salto de Wallrun
    /// <summary>
    /// Aplica la curva de fuerza vertical y maneja walljump y aterrizaje.
    /// </summary>
    private void HandleWallrunMovement()
    {
        if (!WallRunning) return;

        // Mantiene velocidad hacia arriba
        rb.velocity = new Vector3(
            rb.velocity.x,
            upforce,
            rb.velocity.z
        );
        upforce -= WallRunUpForce_DecreaseRate * Time.deltaTime;

        // Walljump al presionar espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 jumpVel = transform.forward * WallJumpForwardVelocity
                            + transform.up * WallJumpUpVelocity;
            rb.velocity = jumpVel;
            WallrunningLeft = WallrunningRight = false;
        }

        // Detener wallrun al aterrizar
        if (rbfps.Grounded)
        {
            WallrunningLeft = WallrunningRight = false;
        }
    }
    #endregion
}
