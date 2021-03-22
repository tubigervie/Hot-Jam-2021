using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private float moveSpeed = 15;

    Rigidbody rb;

    float verticalMove;
    float horizontalMove;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody on Player!");
        }
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

    private void FixedUpdate()
    {
        Vector3 camForward = Vector3.zero;
        Vector3 camRight = Vector3.zero;

        CalculateCameraDirection(ref camForward, ref camRight);

        Debug.DrawRay(transform.position, camForward * 5, Color.blue);
        Debug.DrawRay(transform.position, camRight * 5, Color.red);

        Vector3 playerDir = (camForward * verticalMove + camRight * horizontalMove).normalized;
        Vector3 playerMovement = playerDir * moveSpeed;

        rb.velocity = new Vector3(playerMovement.x, rb.velocity.y, playerMovement.z);

        if (playerDir != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(playerDir);
        }
    }
}
