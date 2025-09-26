using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Avoider : MonoBehaviour
{
   // PoissonDiscSampler poissonDiscSampler;
    public List<Vector3> hidingSpots = new List<Vector3>();

    public NavMeshAgent agent;
    public Transform avoidee;

    public float avoidanceRange;
    public float speed;

    public bool visibleGizmos;
    private bool canAvoideeSeeMe;
    private bool isThereAPlaceToRun;

    private float size_x;
    private float size_y;
    private float cellSize;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            agent.speed = speed;
        }
           
    }

    // Update is called once per frame
    void Update()
    {
        if(agent == null)
        {
            Debug.LogWarning("You NEED to make the object a NavMesh Agent and bake a NavMesh");
        }

        // Always look at avoidee
        transform.LookAt(new Vector3(avoidee.position.x, transform.position.y, avoidee.position.z));

        float position = Vector3.Distance(transform.position, avoidee.position);

        if (canAvoideeSeeMe)
        {
            if (isThereAPlaceToRun)
            {
                if (position < avoidanceRange)
                {
                    FindASpot();
                    //set agent destination 
                }
            }
           /* else
            {
                StartCoroutine(avoidanceTryAgain());
               // FindASpot();
                //set agent destination
            }*/
        }
        else
        {
            StartCoroutine(avoidanceTryAgain());
        }
    }

    public void FindASpot()
    {
        var sampler = new PoissonDiscSampler(size_x, size_y, cellSize);

        foreach (var point in sampler.Samples())
        {
           // Vector3 position = new Vector3(point.x - size_x / 2f, 0, point.y - size_y / 2f);
            if (canAvoideeSeeMe)
            {
                //ignore point
                //Gizmo
            }
            else
            {
                hidingSpots.Add(point);
                //add point to candidate list
                //Gizmo
            }
        }
    }

    public void CheckVisibility()
    {
        Ray rayToPoint = new Ray(transform.position, transform.forward);
        RaycastHit hitVisibility;

        if (Physics.Raycast(rayToPoint, out hitVisibility, 10f))
        {
            Debug.Log("Hit: " + hitVisibility.collider.name);
        }
        else 
        {
            Debug.Log("The point is not visable");
        }
    }

    void OnDrawGizmos()
    {
        if (!visibleGizmos) return;

        // Draw the avoidance range as a circle
        Gizmos.color = Color.red;
    }

    public IEnumerator avoidanceTryAgain()
    {
        yield return new WaitForSeconds(5f);

    }
}


