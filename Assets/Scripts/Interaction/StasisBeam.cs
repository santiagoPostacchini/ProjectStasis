using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class StasisBeam : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    /// <summary>
    /// Configura y muestra el rayo desde el punto de inicio hasta el punto final.
    /// </summary>
    public void SetBeam(Vector3 start, Vector3 end)
    {
        _lineRenderer.enabled = true;

        if (_lineRenderer.useWorldSpace)
        {
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
        }
        else
        {
            Vector3 localStart = transform.InverseTransformPoint(start);
            Vector3 localEnd = transform.InverseTransformPoint(end);
            _lineRenderer.SetPosition(0, localStart);
            _lineRenderer.SetPosition(1, localEnd);
        }
    }

    /// <summary>
    /// Desactiva la visualización del rayo.
    /// </summary>
    public void DisableBeam()
    {
        _lineRenderer.enabled = false;
    }
}
