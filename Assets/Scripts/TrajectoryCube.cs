using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryCube : MonoBehaviour
{
    public LineRenderer _lineRenderer;
    public int _pointsCount = 50;
    public float _timeStep = 0.1f;
    public LayerMask _collisionMask;
    public void DrawTrajectory(Vector3 startPos, Vector3 startVelocity, float drag)
    {
        
        Vector3[] points = new Vector3[_pointsCount];
        Vector3 currentPosition = startPos;
        Vector3 velocity = startVelocity;

        Vector3 gravity = Physics.gravity;
        points[0] = currentPosition;
        int i = 1;

        for (; i < _pointsCount; i++)
        {
            // Aplicar drag: reducci�n exponencial por unidad de tiempo
            velocity *= 1f / (1f + drag * _timeStep);

            // Aplicar gravedad
            Vector3 nextVelocity = velocity + gravity * _timeStep;

            // Posici�n estimada con gravedad y drag
            Vector3 nextPosition = currentPosition + velocity * _timeStep + 0.5f * gravity * (_timeStep * _timeStep);
            Vector3 segment = nextPosition - currentPosition;

            // Comprobar colisi�n
            if (Physics.Raycast(currentPosition, segment.normalized, out RaycastHit hit, segment.magnitude, _collisionMask))
            {
                points[i] = hit.point;
                i++;
                break;
            }

            points[i] = nextPosition;
            currentPosition = nextPosition;
            velocity = nextVelocity;
        }

        _lineRenderer.positionCount = i;
        _lineRenderer.SetPositions(points);
    }
}
