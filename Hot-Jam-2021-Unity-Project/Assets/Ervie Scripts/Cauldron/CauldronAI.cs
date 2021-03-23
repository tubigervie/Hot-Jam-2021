using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CauldronAI : MonoBehaviour
{
    public enum CauldronState { Idle, Wandering, Carried, Complete };
    public CauldronState currentState { get { return _currentState; } }

    NavMeshAgent agent;
    BoxCollider boxCollider;


    [Header("AI Variables")]
    [SerializeField] float maxMoveSpeed = 6f;
    [SerializeField] float wanderIntervalTime;
    [SerializeField] Vector2 wanderRadius = new Vector2(5f, 5f);
    [SerializeField] float waypointTolerance = .5f;
    [SerializeField] CauldronState _currentState = CauldronState.Idle;

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
        currentMesh = GetComponentInChildren<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        currentWayPoint = Vector3.zero;
    }

    private void Start()
    {
        initialPosition = transform.position;
        _wanderTimer = 0;
        SetCauldron();
    }
    // Update is called once per frame
    void Update()
    {
        if (_currentState == CauldronState.Complete) return;
        if (_wanderTimer > wanderIntervalTime && _currentState != CauldronState.Carried)
        {
            _currentState = CauldronState.Wandering;
            currentMesh.material = wanderMaterial;
        }
        HandleStateBehaviours();
        UpdateTimers();
    }

    public void SetOnFirePit()
    {
        _wanderTimer = 0;
        Cancel();
        currentWayPoint = Vector3.zero;
        _currentState = CauldronState.Idle;
        currentMesh.material = idleMaterial;
    }

    public void ToggleCauldronVisibility(bool flag)
    {
        currentMesh.enabled = flag;
        boxCollider.enabled = flag;
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

    public void Complete()
    {
        _currentState = CauldronState.Complete;
    }

    private void HandleStateBehaviours()
    {
        switch (_currentState)
        {
            case CauldronState.Complete:
                Cancel();
                break;
            case CauldronState.Idle:
                break;
            case CauldronState.Carried:
                break;
            case CauldronState.Wandering:
                if (transform.parent != null) transform.parent = null;
                if(waypoints == null)
                {
                    Debug.Log("cauldron requires a waypoint path!");
                    return;
                }
                if(currentWayPoint == Vector3.zero || AtWaypoint())
                {
                    Vector3 randomNavWaypoint = waypoints.GetRandomWaypoint();
                    currentWayPoint = randomNavWaypoint;
                    MoveTo(currentWayPoint, 1);
                }
                break;
        }
    }

    public void Carry()
    {
        _currentState = CauldronState.Carried;
        boxCollider.enabled = false;
        Cancel();
        ToggleCauldronVisibility(false);
    }

    public void Drop(Transform playerTransform)
    {
        _currentState = CauldronState.Idle;
        boxCollider.enabled = true;
        transform.parent = null;
        transform.position = playerTransform.position;
        ToggleCauldronVisibility(true);
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
        agent.speed = 0;
        agent.isStopped = true;
    }
}
