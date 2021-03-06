using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDir = cam.transform.forward;

        transform.forward = lookDir;

        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
    }
}
