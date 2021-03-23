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

    public enum PlayerState
    {
        NORMAL,
        STUNNED,
        SLOWED,
        ACTION,
    }

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
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
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

        if (currentState == PlayerState.STUNNED)
        {
            playerMovement = Vector3.zero;
        }
        else if (currentState == PlayerState.SLOWED)
        {
            playerMovement /= 2;
        }

        HandleGravity();
        characterController.Move(playerMovement * Time.deltaTime);
        characterController.Move(gravityDirection * Time.deltaTime);
        if (playerDir != Vector3.zero && currentState != PlayerState.STUNNED) {
            transform.rotation = Quaternion.LookRotation(playerDir);
        }
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
    // For now, can only occur when player is perfectly normal
    public void ApplyEffect(PlayerState newState, float duration)
    {
        if (currentState == PlayerState.NORMAL)
        {
            currentState = newState;
            StartCoroutine(EffectDuration(duration));
        }
    }

    IEnumerator EffectDuration(float duration)
    {
        if (currentState == PlayerState.STUNNED)
        {
            Instantiate(stunFX, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        }
        yield return new WaitForSeconds(duration);
        currentState = PlayerState.NORMAL;
    }
}
