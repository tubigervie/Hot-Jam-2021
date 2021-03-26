using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spider : MonoBehaviour, IInteractable
{
    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float minSpeed = 5f;
    [SerializeField] float detectionRange = 10f;

    [SerializeField] float jitterMagnitude = .1f;
    [SerializeField] float jitterSpeedMultiplier = 30f;
    [SerializeField] float slowdownPerWaypoint = 1f;
    [SerializeField] AudioClip critterSFX;

    [SerializeField] GameObject spiderLegPickup;

    [SerializeField] GameObject wayPoints;
    Vector3[] wayPointsPos;

    int currentWaypoint = 0;
    int totalWaypoints;

    GameObject player = null;
    GameObject spiderBody = null;

    MeshRenderer[] bodyParts;
    Color startColor;
    Color deathColor;

    SpiderState currentState = SpiderState.WAITING;

    NavMeshAgent agent = null;

    AudioSource audiosource;

    float timer = 0f;
    bool canInteract = true;

    public enum SpiderState
    {
        WAITING,    // no movement; waiting for player interaction
        RUNNING,    // detect player and move to next waypoint
        DYING       // after player pulls its legs off
    }

    void Awake()
    {
        audiosource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();

        spiderBody = transform.GetChild(0).gameObject;
        bodyParts = new MeshRenderer[spiderBody.transform.childCount];
        for (int i = 0; i < spiderBody.transform.childCount; ++i)
        {
            bodyParts[i] = spiderBody.transform.GetChild(i).GetComponent<MeshRenderer>();
        }
        startColor = bodyParts[0].material.color;
        deathColor = startColor;
        deathColor.a = 0f;

        totalWaypoints = wayPoints.transform.childCount;
        wayPointsPos = new Vector3[totalWaypoints];

        for (int i = 0; i < totalWaypoints; ++i)
        {
            wayPointsPos[i] = wayPoints.transform.GetChild(i).transform.position;
        }

        agent.speed = maxSpeed;
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
                if (distToPlayer <= detectionRange)
                {
                    audiosource.PlayOneShot(critterSFX);
                    int nextWaypoint = (currentWaypoint + 1) % totalWaypoints;
                    UpdateWaypoint(nextWaypoint);
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
            timer += Time.deltaTime;
            for (int i = 0; i < bodyParts.Length; ++i)
            {
                bodyParts[i].material.color = Color.Lerp(startColor, deathColor, timer);
            }
        }
    }

    void UpdateWaypoint(int index)
    {
        agent.destination = wayPointsPos[index];
        currentWaypoint = index;
        agent.speed -= slowdownPerWaypoint;
    }

    public void Interact(GameObject player)
    {
        if (currentState == SpiderState.WAITING)
        {
            currentState = SpiderState.RUNNING;
            StartCoroutine(DelayReactivation(.5f));
            UpdateWaypoint(0);
        }
        else if (canInteract && currentState == SpiderState.RUNNING)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        currentState = SpiderState.DYING;
        Instantiate(spiderLegPickup, transform.position, Quaternion.identity);
        timer = 0f;
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }

    IEnumerator DelayReactivation(float seconds)
    {
        canInteract = false;
        yield return new WaitForSeconds(seconds);
        canInteract = true;
    }
}
