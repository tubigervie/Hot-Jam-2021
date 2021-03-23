using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private float moveSpeed = 15;

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

        HandleGravity();
        characterController.Move(playerMovement * Time.deltaTime);
        characterController.Move(gravityDirection * Time.deltaTime);
        if (playerDir != Vector3.zero) {
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
}
