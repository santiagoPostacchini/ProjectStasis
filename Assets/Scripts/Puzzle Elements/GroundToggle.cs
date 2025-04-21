using UnityEngine;

/// <summary>
/// Desactiva el holograma mientras haya al menos un cubo válido
/// (Tag="Cube", Layer="Ground") dentro del trigger del botón.
/// </summary>
[RequireComponent(typeof(Collider))]
public class HologramToggle : MonoBehaviour
{
    [Tooltip("Objeto holográfico que se activa/ desactiva")]
    [SerializeField] private GameObject hologram;

    private int cubesInTrigger = 0;
    private int groundLayer;

    private void Awake()
    {
        if (hologram == null)
            Debug.LogError($"{name}: asigna el holograma en el inspector.");

        groundLayer = LayerMask.NameToLayer("Ground");

        Collider c = GetComponent<Collider>();
        if (!c.isTrigger)
            Debug.LogWarning($"{name}: marca este collider como IsTrigger = true.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCube(other))
        {
            cubesInTrigger++;
            UpdateHologram();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsCube(other))
        {
            cubesInTrigger = Mathf.Max(cubesInTrigger - 1, 0);
            UpdateHologram();
        }
    }

    private bool IsCube(Collider col) =>
        col.CompareTag("Cube") && col.gameObject.layer == groundLayer;

    private void UpdateHologram()
    {
        bool shouldBeVisible = cubesInTrigger == 0;
        if (hologram.activeSelf != shouldBeVisible)
            hologram.SetActive(shouldBeVisible);
    }
}
