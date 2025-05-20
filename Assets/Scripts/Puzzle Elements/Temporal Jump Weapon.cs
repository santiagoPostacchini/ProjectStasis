//using System.Collections;
//using UnityEngine;

//public class TeleportJumpWeapon : MonoBehaviour
//{
//    [Header("Teleport Anchors")]
//    [Tooltip("Transform that marks the destination on the future level (its Y should represent the floor level).")]
//    [SerializeField] private Transform futureTeleportAnchor;

//    [Tooltip("Transform that marks the destination on the past level (its Y should represent the floor level).")]
//    [SerializeField] private Transform pastTeleportAnchor;

//    [Header("Teleport Settings")]
//    [Tooltip("Cooldown time (in seconds) between teleports.")]
//    [SerializeField] private float teleportCooldown = 0.5f;
//    [Tooltip("If the computed horizontal offset is less than this value, a fallback offset is used.")]
//    [SerializeField] private float minOffsetThreshold = 0.1f;
//    [Tooltip("Fallback offset distance along the player's forward direction if computed offset is too small.")]
//    [SerializeField] private float fallbackOffsetDistance = 2.0f;




//    // When true, we assume the player is on the past level and wants to teleport to the future level.
//    // When false, the reverse applies.
//    public bool teleportToFuture = true;
//    private bool canTeleport = true;

//    /// <summary>
//    /// Called by the Player when the teleport key is pressed.
//    /// Initiates the teleport process.
//    /// </summary>
//    public void ActivateTeleport()
//    {
//        if (!canTeleport)
//            return;

//        StartCoroutine(DoTeleport());
//    }

//    private IEnumerator DoTeleport()
//    {
//        canTeleport = false;
//        PhysicsObject physObj = GetComponent<PhysicsObject>();
//        if (physObj != null && physObj.IsGrabbed)
//        {
//            physObj.Drop();

//            PlayerInteractor interactor = physObj.GetComponentInParent<PlayerInteractor>();
//            if (interactor != null)
//            {
//                interactor.ClearHands();
//            }
//        }
//        // (Optional) Drop any held object if needed.
//        PlayerInteractor playerInteractor = GetComponentInChildren<PlayerInteractor>();
//        if (playerInteractor?._objectGrabbable != null)
//        {
//            ObjectUpdater updater = playerInteractor._objectGrabbable.GetComponent<ObjectUpdater>();
//            if (updater != null)
//            {
//                playerInteractor._objectGrabbable.Drop();
//                playerInteractor.ClearHands();
//            }
//        }

//        // Determine the origin and destination anchors.
//        // When teleportToFuture is true, assume the player is on the past level:
//        // origin = pastTeleportAnchor, destination = futureTeleportAnchor.

//        Transform originAnchor = teleportToFuture ? pastTeleportAnchor : futureTeleportAnchor;
//        Transform destinationAnchor = teleportToFuture ? futureTeleportAnchor : pastTeleportAnchor;

//        // Get the Player component (assumed to be on the same GameObject).
//        Player player = GetComponent<Player>();
//        if (player == null)
//        {
//            Debug.LogWarning("TeleportJumpWeapon: No Player component found!");
//            yield break;
//        }

//        // Compute the horizontal offset relative to the origin anchor.
//        Vector3 horizontalOffset = new Vector3(
//            player.transform.position.x - originAnchor.position.x,
//            0f,
//            player.transform.position.z - originAnchor.position.z);


//        float verticalOffsetY = teleportToFuture ? 50f : -50f;
//        // If the horizontal offset is too small, use a fallback offset along the player's forward direction.
//        if (horizontalOffset.magnitude < minOffsetThreshold)
//        {
//            horizontalOffset = player.transform.forward * fallbackOffsetDistance;
//        }

//        // --- NEW SECTION: Calculate player-to-floor offset on the current level ---
//        // --- Calculate player-to-floor offset on the current level ---
//        float playerOffsetAboveFloor = 0f;
//        RaycastHit hit;
//        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 100f, player.whatIsGround))
//        {
//            // You might adjust the offset depending on your player's height.
//            playerOffsetAboveFloor = player.transform.position.y - hit.point.y - 1f;
//        }
//        else
//        {
//            playerOffsetAboveFloor = 0f;
//        }

//        // Use the destination anchor's Y plus the player's offset above the floor
//        Vector3 destination = new Vector3(
//            destinationAnchor.position.x + horizontalOffset.x,
//            destinationAnchor.position.y + playerOffsetAboveFloor,
//            destinationAnchor.position.z + horizontalOffset.z);

//        // Teleport the player
//        player.transform.position = destination;

//        // Toggle the teleport state so that next teleport goes to the alternate level.
//        teleportToFuture = !teleportToFuture;

//        // Wait for the defined cooldown before allowing the next teleport.
//        yield return new WaitForSeconds(teleportCooldown);
//        canTeleport = true;
//    }
//}
/*───────────────────────────────────────────────────────────────
 *  TeleportJumpWeapon.cs ― Viaje temporal con pos‑proceso URP
 *----------------------------------------------------------------
 *  • Aplica un desenfoque (Depth‑of‑Field + efectos extra) antes
 *    y después del salto usando un Volume global controlado por
 *    TimeTravelPostFXManager.
 *  • Conserva la lógica de offset horizontal/vertical y el manejo
 *    de objetos que el jugador pueda estar sujetando.
 *───────────────────────────────────────────────────────────────*/
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player.Player))]
public class TeleportJumpWeapon : MonoBehaviour
{
    /* ════════════════  REFERENCIAS  ════════════════ */
    [Header("Teleport Anchors")]
    [Tooltip("Transform en el nivel FUTURO (su Y debe ser la altura del suelo).")]
    [SerializeField] private Transform futureAnchor;

    [Tooltip("Transform en el nivel PASADO (su Y debe ser la altura del suelo).")]
    [SerializeField] private Transform pastAnchor;

    [Header("Post‑procesado (Volume)")]
    [SerializeField] private TimeTravelPostFXManager fxManager;   // arrastrar PP‑Manager
    [SerializeField] private float blurInTime = 0.25f;
    [SerializeField] private float blurOutTime = 0.40f;

    /* ════════════════  AJUSTES  ════════════════ */
    [Header("Teleport Settings")]
    [Tooltip("Enfriamiento entre saltos (s).")]
    [SerializeField] private float teleportCooldown = 0.5f;

    [Tooltip("Umbral de offset mínimo (m).")]
    [SerializeField] private float minOffsetThreshold = 0.1f;

    [Tooltip("Offset de emergencia al frente (m).")]
    [SerializeField] private float fallbackOffset = 2f;

    /* ════════════════  ESTADO  ════════════════ */
    public bool teleportToFuture = true;   // true = Pasado → Futuro
    private bool canTeleport = true;

    /* ============================================================ */
    public void ActivateTeleport()
    {
        if (canTeleport)
            StartCoroutine(DoTeleport());
    }

    /* ============================================================ */
    private IEnumerator DoTeleport()
    {
        canTeleport = false;

        /* ── 1) Fundido de desenfoque (entrada) ─────────────────── */
        fxManager?.BeginBlur(blurInTime);
        yield return new WaitForSecondsRealtime(blurInTime);

        /* ── 2) Soltar objetos en la mano, si los hubiera ───────── */
        PhysicsObject physObj = GetComponent<PhysicsObject>();
        if (physObj != null && physObj.IsGrabbed)
        {
            physObj.Drop();
            physObj.GetComponentInParent<PlayerInteractor>()?.ClearHands();
        }

        PlayerInteractor interactor = GetComponentInChildren<PlayerInteractor>();
        if (interactor?._objectGrabbable != null)
        {
            interactor._objectGrabbable.Drop();
            interactor.ClearHands();
        }

        /* ── 3) Calcular origen / destino ───────────────────────── */
        Transform origin = teleportToFuture ? pastAnchor : futureAnchor;
        Transform dest = teleportToFuture ? futureAnchor : pastAnchor;

        Player.Player player = GetComponent<Player.Player>();

        /* Offset horizontal relativo al anchor ------------------- */
        Vector3 hOffset = new Vector3(
            player.transform.position.x - origin.position.x,
            0f,
            player.transform.position.z - origin.position.z);

        if (hOffset.sqrMagnitude < minOffsetThreshold * minOffsetThreshold)
            hOffset = player.transform.forward * fallbackOffset;

        /* Offset vertical (altura sobre el piso) ----------------- */
        float yOffset = 0f;
        if (Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit,
                            100f, player.whatIsGround))
        {
            yOffset = player.transform.position.y - hit.point.y - 1f; // mitad cápsula
        }

        /* Mover al jugador --------------------------------------- */
        Vector3 newPos = dest.position + hOffset;
        newPos.y += yOffset;
        player.transform.position = newPos;

        teleportToFuture = !teleportToFuture;

        /* ── 4) Fundido de desenfoque (salida) ─────────────────── */
        fxManager?.EndBlur(blurOutTime);
        yield return new WaitForSecondsRealtime(blurOutTime);

        /* ── 5) Cool‑down final ────────────────────────────────── */
        yield return new WaitForSecondsRealtime(teleportCooldown);
        canTeleport = true;
    }
}
