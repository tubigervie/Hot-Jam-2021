using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconRotator : MonoBehaviour
{
    [SerializeField] float magnitude = 1f;
    [SerializeField] float frequency = 1f;
    [SerializeField] float degreeOffset = 0f;

    RectTransform rectTrans;

    Vector3 rotation = Vector3.zero;

    float timer = 0f;

    private void Start()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        rectTrans.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Sin(timer * frequency) * magnitude + degreeOffset));
    }
}
