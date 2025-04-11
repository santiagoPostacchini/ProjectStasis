using System.Collections;
using UnityEngine;

public class TeleportJumpWeapon : MonoBehaviour
{
    [Header("Teleport Anchors")]
    [Tooltip("Transform that marks the destination on the future level (its Y should represent the floor level).")]
    [SerializeField] private Transform futureTeleportAnchor;

    [Tooltip("Transform that marks the destination on the past level (its Y should represent the floor level).")]
    [SerializeField] private Transform pastTeleportAnchor;

    [Header("Teleport Settings")]
    [Tooltip("Cooldown time (in seconds) between teleports.")]
    [SerializeField] private float teleportCooldown = 0.5f;
    [Tooltip("If the computed horizontal offset is less than this value, a fallback offset is used.")]
    [SerializeField] private float minOffsetThreshold = 0.1f;
    [Tooltip("Fallback offset distance along the player's forward direction if computed offset is too small.")]
    [SerializeField] private float fallbackOffsetDistance = 2.0f;

    // When true, we assume the player is on the past level and wants to teleport to the future level.
    // When false, the reverse applies.
    public bool teleportToFuture = true;
    private bool canTeleport = true;

    /// <summary>
    /// Called by the Player when the teleport key is pressed.
    /// Initiates the teleport process.
    /// </summary>
    public void ActivateTeleport()
    {
        if (!canTeleport)
            return;

        StartCoroutine(DoTeleport());
    }

    private IEnumerator DoTeleport()
    {
        canTeleport = false;
        PhysicsObject physObj = GetComponent<PhysicsObject>();
        if (physObj != null && physObj.IsGrabbed)
        {
            physObj.Drop();

            PlayerInteractor interactor = physObj.GetComponentInParent<PlayerInteractor>();
            if (interactor != null)
            {
                interactor.ClearHands();
            }
        }
        // (Optional) Drop any held object if needed.
        PlayerInteractor playerInteractor = GetComponentInChildren<PlayerInteractor>();
        if (playerInteractor?._objectGrabbable != null)
        {
            ObjectUpdater updater = playerInteractor._objectGrabbable.GetComponent<ObjectUpdater>();
            if (updater != null)
            {
                playerInteractor._objectGrabbable.Drop();
                playerInteractor.ClearHands();
            }
        }

        // Determine the origin and destination anchors.
        // When teleportToFuture is true, assume the player is on the past level:
        // origin = pastTeleportAnchor, destination = futureTeleportAnchor.
        
        Transform originAnchor = teleportToFuture ? pastTeleportAnchor : futureTeleportAnchor;
        Transform destinationAnchor = teleportToFuture ? futureTeleportAnchor : pastTeleportAnchor;

        // Get the Player component (assumed to be on the same GameObject).
        Player player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogWarning("TeleportJumpWeapon: No Player component found!");
            yield break;
        }

        // Compute the horizontal offset relative to the origin anchor.
        Vector3 horizontalOffset = new Vector3(
            player.transform.position.x - originAnchor.position.x,
            0f,
            player.transform.position.z - originAnchor.position.z);
       

        float verticalOffsetY = teleportToFuture ? 50f : -50f;
        // If the horizontal offset is too small, use a fallback offset along the player's forward direction.
        if (horizontalOffset.magnitude < minOffsetThreshold)
        {
            horizontalOffset = player.transform.forward * fallbackOffsetDistance;
        }

        // --- NEW SECTION: Calculate player-to-floor offset on the current level ---
        // --- Calculate player-to-floor offset on the current level ---
        float playerOffsetAboveFloor = 0f;
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 100f, player.whatIsGround))
        {
            // You might adjust the offset depending on your player's height.
            playerOffsetAboveFloor = player.transform.position.y - hit.point.y - 1f;
        }
        else
        {
            playerOffsetAboveFloor = 0f;
        }

        // Use the destination anchor's Y plus the player's offset above the floor
        Vector3 destination = new Vector3(
            destinationAnchor.position.x + horizontalOffset.x,
            destinationAnchor.position.y + playerOffsetAboveFloor,
            destinationAnchor.position.z + horizontalOffset.z);

        // Teleport the player
        player.transform.position = destination;

        // Toggle the teleport state so that next teleport goes to the alternate level.
        teleportToFuture = !teleportToFuture;

        // Wait for the defined cooldown before allowing the next teleport.
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }
}