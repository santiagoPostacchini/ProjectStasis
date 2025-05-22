using UnityEngine;

public class TrajectoryCube : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int pointsCount = 50;
    public float timeStep = 0.1f;
    public LayerMask collisionMask;
    public void DrawTrajectory(Vector3 startPos, Vector3 startVelocity, float drag)
    {
        
        Vector3[] points = new Vector3[pointsCount];
        Vector3 currentPosition = startPos;
        Vector3 velocity = startVelocity;

        Vector3 gravity = Physics.gravity;
        points[0] = currentPosition;
        int i = 1;

        for (; i < pointsCount; i++)
        {
            velocity *= 1f / (1f + drag * timeStep);
            
            Vector3 nextVelocity = velocity + gravity * timeStep;
            
            Vector3 nextPosition = currentPosition + velocity * timeStep + gravity * (0.5f * (timeStep * timeStep));
            Vector3 segment = nextPosition - currentPosition;
            
            if (Physics.Raycast(currentPosition, segment.normalized, out RaycastHit hit, segment.magnitude, collisionMask))
            {
                points[i] = hit.point;
                i++;
                break;
            }

            points[i] = nextPosition;
            currentPosition = nextPosition;
            velocity = nextVelocity;
        }

        lineRenderer.positionCount = i;
        lineRenderer.SetPositions(points);
    }
}
