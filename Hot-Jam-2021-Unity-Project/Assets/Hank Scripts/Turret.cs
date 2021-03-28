using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject turretBullet;
    [SerializeField] int bulletDmg = 1; // only for Boss
    [SerializeField] float cooldown = 1f;
    AudioSource audioSource;

    bool isReady = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact(GameObject player)
    {
        if (isReady) {
            Fire();
        }
    }

    void Fire()
    {
        TurretProjectile bullet = Instantiate(turretBullet, transform.position, Quaternion.identity).GetComponent<TurretProjectile>();
        audioSource.Play();
        bullet.SetMoveDir(transform.forward);
        bullet.SetDmg(bulletDmg);
        StartCoroutine(StartCooldown(cooldown));
    }

    IEnumerator StartCooldown(float seconds)
    {
        isReady = false;
        yield return new WaitForSeconds(seconds);
        isReady = true;
    }
}
