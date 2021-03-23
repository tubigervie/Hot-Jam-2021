using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomProjectile : MonoBehaviour
{
    [SerializeField] float lifetime = 3f;
    [SerializeField] float moveSpeed = 3f;

    [SerializeField] float stunDuration = 1f;
    Vector3 moveDir;

    float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetMoveDir(Vector3 dir)
    {
        moveDir = dir.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().Stun(stunDuration);
            Debug.Log("Hit Player!");
            Destroy(this.gameObject);
        }
    }
}
