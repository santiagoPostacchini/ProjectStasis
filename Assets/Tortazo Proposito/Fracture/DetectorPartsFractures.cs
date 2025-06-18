using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

public class DetectorPartsFractures : MonoBehaviour
{
    [SerializeField] private StasisGunEntity diosa1;
    [SerializeField] private StasisGunEntity diosa2;

    [Header("Parámetros de detección")]
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 boxCenterOffset = new Vector3(0f, 0f, 2f);
    [SerializeField] private LayerMask detectionLayer;

    private Collider[] hits = new Collider[10];
    private HashSet<DestroyedPieceController> alreadyDetected = new HashSet<DestroyedPieceController>();

    void Update()
    {
        Vector3 boxCenter = transform.position + transform.TransformDirection(boxCenterOffset);
        int count = Physics.OverlapBoxNonAlloc(boxCenter, boxHalfExtents, hits, transform.rotation, detectionLayer);

        for (int i = 0; i < count; i++)
        {
            DestroyedPieceController part = hits[i].GetComponent<DestroyedPieceController>();
            if (part != null && !alreadyDetected.Contains(part))
            {
                ChooseGod(part);
                alreadyDetected.Add(part); // Solo se llama una vez
            }
        }
    }

    public void ChooseGod(DestroyedPieceController part)
    {
        int randomValue = Random.Range(0, 2);
        int a = Random.Range(0, 6); // 0 es falso
        if (a < 2) return;

        if (randomValue == 0)
        {
            diosa1.TryStasis(part.transform, part);
        }
        else
        {
            diosa2.TryStasis(part.transform, part);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // verde translúcido

        Vector3 boxCenter = transform.position + transform.TransformDirection(boxCenterOffset);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, boxHalfExtents * 2);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);
    }
}