using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spider : MonoBehaviour, IInteractable
{
    [SerializeField] float moveSpeed = 20f;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float jitterMagnitude = .1f;
    [SerializeField] float jitterSpeedMultiplier = 30f;

    [SerializeField] GameObject spiderLegPickup;

    [SerializeField] GameObject wayPoints;
    Vector3[] wayPointsPos;

    int currentWaypoint = 0;
    int totalWaypoints;

    BoxCollider interactionCollider = null;
    GameObject player = null;
    GameObject spiderBody = null;

    SpiderState currentState = SpiderState.WAITING;

    NavMeshAgent agent = null;

    float timer = 0f;

    public enum SpiderState
    {
        WAITING,    // no movement; waiting for player interaction
        RUNNING,    // detect player and move to next waypoint
        TIRED,      // arrived at last waypoint; waiting for player interaction
        DYING       // after player pulls its legs off
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        interactionCollider = GetComponent<BoxCollider>();
        agent = GetComponent<NavMeshAgent>();

        spiderBody = transform.GetChild(0).gameObject;

        totalWaypoints = wayPoints.transform.childCount;
        wayPointsPos = new Vector3[totalWaypoints];

        for (int i = 0; i < totalWaypoints; ++i)
        {
            wayPointsPos[i] = wayPoints.transform.GetChild(i).transform.position;
        }

        agent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float distToPlayer = (player.transform.position - transform.position).magnitude;
        if (currentState == SpiderState.RUNNING)
        {
            timer += Time.deltaTime;
            float horizontalOffset = Mathf.Sin(timer * jitterSpeedMultiplier) * jitterMagnitude;
            
            if (agent.remainingDistance <= agent.stoppingDistance && agent.velocity.magnitude == 0f)
            {
                if (currentWaypoint == totalWaypoints - 1)  // is the last waypoint
                {
                    currentState = SpiderState.TIRED;
                    interactionCollider.enabled = true;
                }
                else if (distToPlayer <= detectionRange)
                {
                    UpdateWaypoint(currentWaypoint + 1);
                }
                else if (distToPlayer > detectionRange)
                {
                    horizontalOffset = 0f;
                }
            }

            spiderBody.transform.localPosition = transform.InverseTransformDirection(transform.right) * horizontalOffset;
        }
        else if (currentState == SpiderState.DYING)
        {
            // runaway and disappear
        }
    }

    void UpdateWaypoint(int index)
    {
        agent.destination = wayPointsPos[index];
        currentWaypoint = index;
    }

    public void Interact(GameObject player)
    {
        if (currentState == SpiderState.WAITING)
        {
            currentState = SpiderState.RUNNING;
            StartCoroutine(DelayDeactivation(.5f));
            UpdateWaypoint(0);
        }
        else if (currentState == SpiderState.TIRED)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        currentState = SpiderState.DYING;
        Instantiate(spiderLegPickup, transform.position, Quaternion.identity);
        interactionCollider.enabled = false;
        UpdateWaypoint(0);
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }

    IEnumerator DelayDeactivation(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        interactionCollider.enabled = false;
    }
}
