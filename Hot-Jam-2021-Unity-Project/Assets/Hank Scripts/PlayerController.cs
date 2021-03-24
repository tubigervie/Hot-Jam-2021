using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private float moveSpeed = 15;
    [SerializeField] GameObject stunFX;
    [SerializeField] GameObject slowFX;
    [SerializeField] AudioClip hitSFX;

    Coroutine lastRoutine = null;
    AudioSource audioSource;
    DebugPanelController debugPanel;

    // Higher priority based on order (e.g., STUNNED can overwrite SLOWED, etc..)
    public enum PlayerState
    {
        NORMAL,
        SLOWED,
        STUNNED,
        ACTION,
        INDIALOGUE,
        TELEPORT
    }

    public List<PlayerState> disableMoveStates = new List<PlayerState>();

    PlayerState currentState = PlayerState.NORMAL;

    CharacterController characterController;

    float verticalMove;
    float horizontalMove;
    float gravity;
    float gravitySpeed = 1f;
    Vector3 gravityDirection;

    bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        debugPanel = GameObject.FindObjectOfType<DebugPanelController>();
    }

    // Update is called once per frame
    void Update()
    {
        verticalMove = Input.GetAxisRaw("Vertical");
        horizontalMove = Input.GetAxisRaw("Horizontal");
    }

    void CalculateCameraDirection(ref Vector3 forward, ref Vector3 right)
    {
        Transform camTrans = cam.transform;
        forward = new Vector3(camTrans.forward.x, 0, camTrans.forward.z).normalized;
        right = new Vector3(camTrans.right.x, 0, camTrans.right.z).normalized;
    }

    //private bool OnSlope()
    //{
    //    RaycastHit hit; 
    //    if(Physics.Raycast(transform.position,  Vector3.down, out hit, ))
    //}

    private void FixedUpdate()
    {
        Vector3 camForward = Vector3.zero;
        Vector3 camRight = Vector3.zero;

        CalculateCameraDirection(ref camForward, ref camRight);

        Debug.DrawRay(transform.position, camForward * 5, Color.blue);
        Debug.DrawRay(transform.position, camRight * 5, Color.red);

        Vector3 playerDir = (camForward * verticalMove + camRight * horizontalMove).normalized;
        Vector3 playerMovement = playerDir * moveSpeed;

        bool canMoveFlag = true;
        if (disableMoveStates.Contains(currentState))
        {
            canMoveFlag = false;
            playerMovement = Vector3.zero;
        }
        else if (currentState == PlayerState.SLOWED)
        {
            playerMovement /= 2;
        }

        HandleGravity();

        if (canMoveFlag)
        {
            characterController.Move(playerMovement * Time.deltaTime);
            characterController.Move(gravityDirection * Time.deltaTime);
            if (playerDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(playerDir);
            }
        }

        // DebugPanel Updates
        debugPanel.UpdatePlayerState(currentState);
    }

    private void HandleGravity()
    {
        gravity = Mathf.Lerp(gravity, (!isGrounded) ? (-20f) : (-10), Time.deltaTime);
        gravityDirection = base.transform.up * gravity * gravitySpeed;
        gravityDirection = new Vector3(0, gravityDirection.y, 0);
        bool flag = false;
        if (Physics.SphereCast(base.transform.position + new Vector3(0f, 0.5f, 0f), .15f, -base.transform.up, out RaycastHit hitInfo, .5f, 1 << LayerMask.NameToLayer("Terrain")))
        {
            flag = true;
        }
        if (isGrounded != flag)
        {
            isGrounded = flag;
        }
    }

    // For debuff effects (e.g., stunned, slowed, etc..)
    // For now, if newState has higher priority, then overwrite to newState
    public void ApplyEffect(PlayerState newState, float duration)
    {
        if (newState > currentState)
        {
            currentState = newState;
            if (lastRoutine != null)
            {
                StopCoroutine(lastRoutine);
            }
            lastRoutine = StartCoroutine(EffectDuration(duration));
        }
    }

    public void ApplyEffect(PlayerState newState)
    {
        if (newState != currentState)
        {
            currentState = newState;
        }
    }


    public void DialogueTriggerStartEvent()
    {
        currentState = PlayerState.INDIALOGUE;
    }

    public void DialogueTriggerEndEvent()
    {
        currentState = PlayerState.NORMAL;
    }


    public bool IsStunned()
    {
        return currentState == PlayerState.STUNNED;
    }

    public void PlayHitSFX()
    {
        audioSource.PlayOneShot(hitSFX);
    }

    public PlayerState GetState()
    {
        return currentState;
    }

    IEnumerator EffectDuration(float duration)
    {
        if (currentState == PlayerState.STUNNED)
        {
            Instantiate(stunFX, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        }
        else if (currentState == PlayerState.SLOWED)
        {
            Instantiate(slowFX, transform.position + Vector3.up * 1.5f, Quaternion.identity, transform);
        }
        yield return new WaitForSeconds(duration);
        currentState = PlayerState.NORMAL;
    }
}
