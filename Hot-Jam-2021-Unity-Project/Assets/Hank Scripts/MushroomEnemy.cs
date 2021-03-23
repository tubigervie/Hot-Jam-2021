﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomEnemy : MonoBehaviour
{
    [SerializeField] float cooldown = 1f;
    [SerializeField] GameObject bulletPrefab;

    float timer = 0f;
    [SerializeField] bool isLooping = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLooping)
        {
            timer += Time.deltaTime;
            if (timer >= cooldown)
            {
                Fire();
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
        }
    }

    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<MushroomProjectile>().SetMoveDir(transform.forward);
    }

    public void SetLoopFiring(bool shouldFire)
    {
        isLooping = shouldFire;
    }
}
