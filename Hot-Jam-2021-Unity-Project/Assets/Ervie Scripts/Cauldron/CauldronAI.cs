using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CauldronAI : MonoBehaviour
{
    public enum CauldronState { Idle, Wandering };

    NavMeshAgent agent;
    CauldronContainer container;

    [Header("AI Variables")]
    [SerializeField] float maxMoveSpeed = 6f;
    [SerializeField] float wanderIntervalTime;
    [SerializeField] Vector2 wanderRadius = new Vector2(5f, 5f);
    [SerializeField] float waypointTolerance = .5f;
    [SerializeField] CauldronState currentState = CauldronState.Idle;
    [SerializeField] LayerMask avoidanceMask;
    [SerializeField] CauldronWaypoints waypoints;

    [Header("Test Render Materials - will remove")]
    [SerializeField] Material idleMaterial;
    [SerializeField] Material wanderMaterial;

    float _wanderTimer = 0f;
    Vector3 currentWayPoint;
    Vector3 initialPosition;
    Quaternion initialRotation;
    MeshRenderer currentMesh;
    float _currentWanderDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        container = GetComponent<CauldronContainer>();
        currentMesh = GetComponentInChildren<MeshRenderer>();
        currentWayPoint = Vector3.zero;
    }

    private void Start()
    {
        _wanderTimer = 0;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        currentState = CauldronState.Idle;
        currentMesh.material = idleMaterial;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetCauldron();
            currentState = CauldronState.Idle;
        }
        if (_wanderTimer > wanderIntervalTime)
        {
            currentState = CauldronState.Wandering;
        }
        HandleStateBehaviours();
        UpdateTimers();
    }

    private void SetCauldron()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        currentWayPoint = Vector3.zero;
        _wanderTimer = 0;
        currentMesh.material = idleMaterial;
        Cancel();
    }

    private void HandleStateBehaviours()
    {
        switch (currentState)
        {
            case CauldronState.Idle:
                break;
            case CauldronState.Wandering:
                if(waypoints == null)
                {
                    Debug.Log("cauldron requires a waypoint path!");
                    return;
                }
                if(currentWayPoint == Vector3.zero || AtWaypoint())
                {
                    currentMesh.material = wanderMaterial;
                    Vector3 randomNavWaypoint = waypoints.GetRandomWaypoint();
                    randomNavWaypoint.y = transform.position.y;
                    currentWayPoint = randomNavWaypoint;
                    MoveTo(currentWayPoint, 1);
                }
                break;
        }
    }

    private bool AtWaypoint()
    {
        float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
        return distanceToWaypoint < waypointTolerance;
    }

    private Vector3 GetCurrentWaypoint()
    {
        return currentWayPoint;
    }

    private Vector3 RandomNavSphere(Vector3 origin)
    {
        _currentWanderDistance = UnityEngine.Random.Range(wanderRadius.x, wanderRadius.y);
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _currentWanderDistance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, _currentWanderDistance, 1 >> 0);
        
        while(Vector3.Distance(transform.position, navHit.position) > 100)
        {
            _currentWanderDistance = UnityEngine.Random.Range(wanderRadius.x, wanderRadius.y);
            randomDirection = UnityEngine.Random.insideUnitSphere * _currentWanderDistance;
            randomDirection += origin;
            NavMesh.SamplePosition(randomDirection, out navHit, _currentWanderDistance, 1 >> 0);
        }

        Debug.Log(navHit.position);

        return navHit.position;
    }


    private void UpdateTimers()
    {
        _wanderTimer += Time.deltaTime;
    }

    public void MoveTo(Vector3 destination, float speedFraction)
    {
        agent.destination = destination;
        agent.speed = maxMoveSpeed * Mathf.Clamp01(speedFraction);
        agent.isStopped = false;
    }

    public void Cancel()
    {
        agent.destination = initialPosition;
        agent.speed = 0;
        agent.isStopped = true;
    }
}
