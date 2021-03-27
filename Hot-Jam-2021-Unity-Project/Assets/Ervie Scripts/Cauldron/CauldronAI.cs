using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CauldronAI : MonoBehaviour
{
    public enum CauldronState { Pregame, Idle, Wandering, Carried, Complete, Overflow };
    public CauldronState currentState { get { return _currentState; } }

    public enum BoilState { Normal, Rush};

    NavMeshAgent agent;
    SphereCollider sphereCollider;

    Animator anim;

    [Header("AI Variables")]
    [SerializeField] float maxMoveSpeed = 6f;
    [SerializeField] float wanderIntervalTime = 15;
    [SerializeField] float waypointTolerance = .5f;
    [SerializeField] CauldronState _currentState = CauldronState.Idle;
    [SerializeField] BoilState _boilState = BoilState.Normal;
    [SerializeField] bool shouldWander = false;
    [SerializeField] LayerMask avoidanceMask;
    [SerializeField] CauldronWaypoints waypoints;
    [SerializeField] List<CauldronWaypoints> waypointsList = new List<CauldronWaypoints>();
    [SerializeField] GameObject steamFX;
    float _wanderTimer = 0f;
    float _totalBoilTimer = 0f;
    float _boilTimer = 0f;

    [SerializeField] Vector3 currentWayPoint;
    Vector3 initialPosition;
    Quaternion initialRotation;
    MeshRenderer currentMesh;
    float _currentWanderDistance;

    DebugPanelController debugPanel;

    public Action onCauldronStart;
    public Action onUpdateTimer;

    int wayPointIndex = 0;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentMesh = GetComponentInChildren<MeshRenderer>();
        anim = GetComponent<Animator>();
        sphereCollider = GetComponent<SphereCollider>();
        currentWayPoint = Vector3.zero;
        initialPosition = transform.position;
        SetCauldron();
    }

    private void Start()
    {
        debugPanel = GameObject.FindObjectOfType<DebugPanelController>();
        UpdateLabels();
    }
    // Update is called once per frame
    void Update()
    {
        if (_currentState == CauldronState.Pregame || _currentState == CauldronState.Complete || _currentState == CauldronState.Overflow) return;
        HandleBoilState();
        if (_wanderTimer > wanderIntervalTime && _currentState != CauldronState.Carried && _currentState != CauldronState.Wandering)
        {
            _currentState = CauldronState.Wandering;
            waypoints = waypointsList[UnityEngine.Random.Range(0, waypointsList.Count)];
            anim.SetBool("isWandering", true);
        }
        HandleStateBehaviours();
        float timeDelta = Time.deltaTime;
        if (shouldWander)
        {
            _wanderTimer += timeDelta;
        }
        _boilTimer -= timeDelta;
        if (_boilTimer < 0)
            _boilTimer = 0;
        onUpdateTimer.Invoke();
        UpdateLabels();
    }

    public void OnStartLevel(float boilTime)
    {
        _currentState = CauldronState.Idle;
        _boilTimer = boilTime;
        _totalBoilTimer = boilTime;
         onCauldronStart.Invoke();
    }

    public void SetOnFirePit()
    {
        _wanderTimer = 0;
        Cancel();
        currentWayPoint = Vector3.zero;
        _currentState = CauldronState.Idle;
        anim.SetBool("isWandering", false);
    }

    public float GetBoilTimer()
    {
        return _boilTimer;
    }

    public float GetTotalBoilTimer()
    {
        return _totalBoilTimer;
    }

    public void SetBoilTimer(float time)
    {
        _boilTimer = time;
    }

    public void SetTotalBoilTimer(float time)
    {
        _totalBoilTimer = time;
    }

    public void ToggleCauldronVisibility(bool flag)
    {
        currentMesh.enabled = flag;
        sphereCollider.enabled = flag;
        steamFX.SetActive(flag);
    }

    private void SetCauldron()
    {
        _currentState = CauldronState.Pregame;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        currentWayPoint = Vector3.zero;
        _wanderTimer = wanderIntervalTime;
        _boilTimer = 0;
        Cancel();
    }

    public void Complete()
    {
        Cancel();
        _currentState = CauldronState.Complete;
        debugPanel.UpdateCauldronState(_currentState);
        _boilState = BoilState.Normal;
        _wanderTimer = 0;
        debugPanel.UpdateCauldronTimer(_wanderTimer);
    }

    private void HandleStateBehaviours()
    {
        switch (_currentState)
        {
            case CauldronState.Complete:
                break;
            case CauldronState.Idle:
                break;
            case CauldronState.Carried:
                break;
            case CauldronState.Wandering:
                if (transform.parent != null) 
                {
                    transform.parent.parent.GetComponent<FirePit>().EnableCollider(); //don't judge me....
                    transform.parent = null;
                }
                if(waypoints == null)
                {
                    Debug.Log("cauldron requires a waypoint path!");
                    return;
                }
                if(currentWayPoint == Vector3.zero)
                {
                    currentWayPoint = waypoints.GetWaypoint(wayPointIndex);
                    MoveTo(currentWayPoint, 1);
                }
                if (AtWaypoint())
                {
                    wayPointIndex++;
                    if (wayPointIndex >= waypoints.transform.childCount)
                    {
                        waypoints = waypointsList[UnityEngine.Random.Range(0, waypointsList.Count)];
                        wayPointIndex = 0;
                    }
                    currentWayPoint = waypoints.GetWaypoint(wayPointIndex);
                    MoveTo(currentWayPoint, 1);
                }
                break;
        }
    }

    public void Carry()
    {
        _currentState = CauldronState.Carried;
        sphereCollider.enabled = false;
        Cancel();
        ToggleCauldronVisibility(false);
    }

    public void Drop(Transform playerTransform)
    {
        _currentState = CauldronState.Idle;
        sphereCollider.enabled = true;
        transform.parent = null;
        transform.position = playerTransform.position;
        ToggleCauldronVisibility(true);
        currentWayPoint = waypoints.GetWaypoint(wayPointIndex);
        MoveTo(currentWayPoint, 1);
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

    private void HandleBoilState()
    {
        float boilInterval = GetBoilRatio();
        if(_boilTimer <= 0 && _currentState != CauldronState.Overflow)
        {
            _currentState = CauldronState.Overflow;
            FindObjectOfType<GameManager>().LoadLose();
            Debug.Log("Lost!");
        }
        else if(boilInterval <= .35f && _boilState != BoilState.Rush)
        {
            _boilState = BoilState.Rush;
            FindObjectOfType<AudioManager>().StartLevelRushTheme();
        }
        else if(boilInterval > .35f && _boilState != BoilState.Normal)
        {
            _boilState = BoilState.Normal;
            FindObjectOfType<AudioManager>().StartLevelTheme();
        }
    }

    //private Vector3 RandomNavSphere(Vector3 origin)
    //{
    //    _currentWanderDistance = UnityEngine.Random.Range(wanderRadius.x, wanderRadius.y);
    //    Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _currentWanderDistance;

    //    randomDirection += origin;

    //    NavMeshHit navHit;

    //    NavMesh.SamplePosition(randomDirection, out navHit, _currentWanderDistance, 1 >> 0);
        
    //    while(Vector3.Distance(transform.position, navHit.position) > 100)
    //    {
    //        _currentWanderDistance = UnityEngine.Random.Range(wanderRadius.x, wanderRadius.y);
    //        randomDirection = UnityEngine.Random.insideUnitSphere * _currentWanderDistance;
    //        randomDirection += origin;
    //        NavMesh.SamplePosition(randomDirection, out navHit, _currentWanderDistance, 1 >> 0);
    //    }

    //    Debug.Log(navHit.position);

    //    return navHit.position;
    //}

    private float GetBoilRatio()
    {
        return _boilTimer / _totalBoilTimer;
    }


    private void UpdateTimers()
    {
        _wanderTimer += Time.deltaTime;
        _boilTimer -= Time.deltaTime;
        if (_boilTimer < 0) _boilTimer = 0;
    }

    private void UpdateLabels()
    {
        debugPanel.UpdateCauldronTimer(_wanderTimer);
        debugPanel.UpdateBoilTimer(_boilTimer);
        debugPanel.UpdateCauldronState(_currentState);
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
