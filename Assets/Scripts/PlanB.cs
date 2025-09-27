using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlanB : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform avoidee;
    public Transform wall;
    public Transform wall1;


    public float avoidanceRange;
    public float speed;
    public bool visibleGizmos = true;

    private float size_x = 10f;
    private float size_y = 10f;
    private float cellSize = 10f;

    private List<Vector3> candidateSpots = new List<Vector3>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    void Update()
    {
        agent.speed = speed;

        if (avoidee == null) return;

        if (agent == null)
        {
            Debug.LogWarning("You NEED to make the object a NavMesh Agent and bake a NavMesh");
        }

        // Always look at avoidee
        transform.LookAt(new Vector3(avoidee.position.x, transform.position.y, avoidee.position.z));

        float distance = Vector3.Distance(transform.position, avoidee.position);

        // If player is close, try to escape
        if (distance < avoidanceRange)
        {
            FindASpot();
        }
    }

    /// <summary>
    /// Find a hiding spot using Poisson-disc sampling
    /// </summary>
    public void FindASpot()
    {
        candidateSpots.Clear();
        var sampler = new PoissonDiscSampler(size_x, size_y, cellSize);

        foreach (var point in sampler.Samples())
        {
            // Convert Poisson point into world position centered around Avoider
            Vector3 poissonPoint = transform.position + new Vector3(point.x - size_x / 2f, 0, point.y - size_y / 2f);

            if (!CheckVisibility(poissonPoint)) // good hiding spot
            {
                candidateSpots.Add(poissonPoint);

                if (visibleGizmos)
                {
                    Debug.DrawLine(avoidee.position, poissonPoint, Color.green, 1f);
                }

            }
            else if (visibleGizmos)
            {
                Debug.DrawLine(avoidee.position, poissonPoint, Color.red, 1f);
            }
        }

        if (candidateSpots.Count > 0)
        {
            // Pick the closest spot to the Avoider
            Vector3 closest = candidateSpots[0];
            float minDist = Vector3.Distance(transform.position, closest);

            foreach (var spot in candidateSpots)
            {
                float d = Vector3.Distance(transform.position, spot);
                if (d < minDist)
                {
                    minDist = d;
                    closest = spot;
                }
            }

            agent.SetDestination(closest);
        }
    }

    /// <summary>
    /// Checks if the avoidee can see a given point
    /// </summary>
    public bool CheckVisibility(Vector3 point)
    {
        Vector3 dirToPoint = (point - avoidee.position).normalized;
        float distToPoint = Vector3.Distance(avoidee.position, point);

        // If the ray hits an obstacle before the point, then the point is hidden
        if (Physics.Raycast(avoidee.position, dirToPoint, out RaycastHit hit, distToPoint))
        {
            if (hit.collider.transform != this.transform && hit.collider.transform != avoidee)
            {
                return false; // blocked to not visible
            }

            /* if (hit.collider.transform == wall || hit.collider.transform == wall2)
             {
                 Debug.Log("wall hit");
                 return false; // blocked to not visible
             }*/

        }

       // Debug.Log("wall not hit");
        return true; // nothing blocked to visible
    }

    void OnDrawGizmos()
    {
        if (avoidee != null && visibleGizmos)
        {
            // Show the avoidance range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, avoidanceRange);
        }
    }
}