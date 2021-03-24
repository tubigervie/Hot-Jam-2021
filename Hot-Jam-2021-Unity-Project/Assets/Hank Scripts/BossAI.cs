﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [SerializeField] int maxHP = 10;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float minScale = .75f;
    
    [SerializeField] float detectionRange = 30f;
    [SerializeField] float chaseDuration = .5f;
    [SerializeField] float chaseCooldown = .25f;
    [SerializeField] float jumpHeight = 5f;

    [SerializeField] float stunDuration = 1f;

    Vector3 spawnPos;

    bool landedHit = false; // used to change moveDir during BossState.CHASING

    AnimationCurve jumpCurve;
    
    int currentHP;
    float originalScale;
    GameObject player;
    Vector3 moveDir = Vector3.zero;

    float timer = 0f;   // value used to sample AnimationCurve

    float yOffset = 0f; // keep track of y-offset caused by AnimationCurve

    float minMaxRatio;  // used to define min jump height and min moveSpeed of slime boss
    float minJumpHeight;
    float minMoveSpeed;

    float healthRatio;  // used to define characteristics at different sizes

    MeshRenderer meshRender;
    Color regularColor;
    Color deathColor;

    Coroutine lastRoutine = null;

    // State Transitions: NEUTRAL -> (Player nearby) -> CHASING -> (finish 1 hop) -> RESTING -> NEUTRAL
    //      NEUTRAL / CHASING / RESTING -> (hp <= 0) -> DYING
    public enum BossState
    {
        NEUTRAL,    // when boss is ready to sense for player
        CHASING,    // basic hopping towards player
        RESTING,    // the delay between hops
        DYING,      // for death anim
    }

    BossState currentState = BossState.NEUTRAL;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = transform.position;

        meshRender = GetComponent<MeshRenderer>();
        regularColor = meshRender.material.color;
        deathColor = Color.white;
        deathColor.a = 0f;

        currentHP = maxHP;
        originalScale = transform.localScale.x;

        if (minScale >= originalScale)
        {
            Debug.LogWarning("Boss' start scale must be larger than its minimum scale!");
        }
        if (minScale < 0.1f)
        {
            minScale = 0.1f;
        }

        player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogWarning("No player found!");
        }

        // curve to sample from for Boss' hop y-pos
        jumpCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(chaseDuration/2f, jumpHeight), new Keyframe(chaseDuration, 0));

        // initializing min/max values to lerp between
        minMaxRatio = minScale / originalScale;
        minJumpHeight = jumpHeight * minMaxRatio * 2;
        minMoveSpeed = moveSpeed * minMaxRatio * 2;
        healthRatio = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 towardsPlayer = (player.transform.position - transform.position);
        towardsPlayer.y = 0;

        if (currentState == BossState.NEUTRAL && towardsPlayer.magnitude <= detectionRange)
        {
            lastRoutine = StartCoroutine(Chase(chaseDuration, chaseCooldown));
        }
        else if (currentState == BossState.CHASING)
        {
            timer += Time.deltaTime;

            float verticalDisp = jumpCurve.Evaluate(timer);
            float speedMultiplier = Mathf.Lerp(minMoveSpeed, moveSpeed, healthRatio);

            Vector3 verticalChange = (verticalDisp - yOffset) * Vector3.up;
            Vector3 horizontalChange = moveDir * speedMultiplier * Time.deltaTime;

            transform.position += horizontalChange + verticalChange;

            yOffset = verticalDisp; // update new total offset from original y-pos
        }
        else if (currentState == BossState.DYING)
        {
            timer += Time.deltaTime;
            meshRender.material.color = Color.Lerp(regularColor, deathColor, timer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController playerControl = other.gameObject.GetComponent<PlayerController>();
            playerControl.ApplyEffect(PlayerController.PlayerState.STUNNED, stunDuration);
            landedHit = true;
        }
    }

    IEnumerator Chase(float duration, float cooldown)
    {
        currentState = BossState.CHASING;
        timer = 0f;
        Vector3 target = (landedHit) ? spawnPos : player.transform.position;
        CalculateJumpDir(target);
        yield return new WaitForSeconds(duration);
        lastRoutine = StartCoroutine(Rest(cooldown));
    }

    IEnumerator Rest(float duration)
    {
        currentState = BossState.RESTING;
        yield return new WaitForSeconds(duration);
        currentState = BossState.NEUTRAL;
    }

    void CalculateJumpDir(Vector3 targetPos)
    {
        Vector3 towardsTarget = (targetPos - transform.position);
        towardsTarget.y = 0;
        moveDir = towardsTarget.normalized;

        landedHit = false;
    }

    void UpdateSize()
    {
        // Recalculate scale and jumpHeight w/ new healthRatio
        healthRatio = (float)currentHP / maxHP;

        float scaleAmount = Mathf.Lerp(minScale, originalScale, healthRatio);
        transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);

        float jump = Mathf.Lerp(minJumpHeight, jumpHeight, healthRatio);
        jumpCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(chaseDuration / 2f, jump), new Keyframe(chaseDuration, 0));

        // Ensure boss stays on the ground despite scale change (assumes scale only descreases)
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);
            transform.position -= Vector3.up * (hit.distance - transform.localScale.x / 2) - yOffset * Vector3.up;
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentState != BossState.DYING)
        {
            currentHP -= amount;
            if (currentHP <= 0)
            {
                if (lastRoutine != null)
                {
                    StopCoroutine(lastRoutine);
                }
                StartCoroutine(Die());
            }
            UpdateSize();
        }
    }

    IEnumerator Die()
    {
        timer = 0f;
        currentState = BossState.DYING;
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
