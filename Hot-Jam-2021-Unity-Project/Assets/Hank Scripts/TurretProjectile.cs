using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    [SerializeField] float lifetime = 3f;
    [SerializeField] float moveSpeed = 3f;
    
    int dmg = 1;   // only for dealing dmg to Boss
    Vector3 moveDir = Vector3.zero;
    float timer = 0f;

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

    public void SetDmg(int amount)
    {
        this.dmg = amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Boss")
        {
            BossAI boss = other.gameObject.GetComponent<BossAI>();
            boss.TakeDamage(dmg);
            Destroy(this.gameObject);

            Debug.Log("Hit Boss!");
        }
    }
}
