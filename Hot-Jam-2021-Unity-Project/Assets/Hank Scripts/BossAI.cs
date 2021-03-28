using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [SerializeField] int maxHP = 10;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float maxMoveSpeed = 12f;
    [SerializeField] float minScale = .75f;
    
    [SerializeField] float detectionRange = 30f;
    [SerializeField] float jumpDuration = .5f;
    [SerializeField] float jumpCooldown = 1.5f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] GameObject disableWhenDie;
    [SerializeField] float stunDuration = 1f;
    [SerializeField] AudioSource audioSource;

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

    float healthRatio;  // used to define characteristics at different sizes

    float speedMultiplier;

    MeshRenderer meshRender;
    Color regularColor;
    Color deathColor;

    Coroutine lastRoutine = null;

    public Action onDie;

    bool increaseDetectionRangeFlag = false;

    // State Transitions: NEUTRAL -> (Player nearby) -> CHASING -> (finish 1 hop) -> RESTING -> NEUTRAL
    //      NEUTRAL / CHASING / RESTING -> (hp <= 0) -> DYING
    public enum BossState
    {
        NEUTRAL,    // when boss is ready to sense for player
        JUMPING,    // a single hop
        RESTING,    // delay between hops
        DYING,      // for death anim
    }

    BossState currentState = BossState.NEUTRAL;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = transform.position;
        audioSource = GetComponent<AudioSource>();
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
        jumpCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(jumpDuration / 2f, jumpHeight), new Keyframe(jumpDuration, 0));

        // initializing min/max values to lerp between
        minMaxRatio = minScale / originalScale;
        minJumpHeight = jumpHeight * minMaxRatio * 2;
        healthRatio = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distToPlayer = (player.transform.position - transform.position);
        distToPlayer.y = 0;

        if (currentState == BossState.NEUTRAL && distToPlayer.magnitude <= detectionRange)
        {
            if (!increaseDetectionRangeFlag)
            {
                increaseDetectionRangeFlag = true;
                detectionRange = 30;
            }
            lastRoutine = StartCoroutine(Jump(jumpDuration, jumpCooldown));
        }
        else if (currentState == BossState.JUMPING)
        {
            timer += Time.deltaTime;

            float verticalDisp = jumpCurve.Evaluate(timer);

            Vector3 verticalChange = (verticalDisp - yOffset) * Vector3.up;
            Vector3 horizontalChange = moveDir * speedMultiplier * Time.deltaTime;

            transform.position += horizontalChange + verticalChange;

            yOffset = verticalDisp; // update new total offset from original y-pos

            int layerMask = LayerMask.GetMask("Terrain");

            // Ensure boss stays on the ground despite scale change (assumes scale only descreases)
            RaycastHit hit;

            if (Physics.SphereCast(transform.position, .15f,-Vector3.up, out hit, 10f, layerMask))
            {
                Debug.Log(hit.collider.gameObject.name);
                transform.position -= Vector3.up * (hit.distance - transform.localScale.x / 2) - yOffset * Vector3.up;
            }
        }
        else if (currentState == BossState.DYING)
        {
            timer += Time.deltaTime;
            meshRender.material.color = Color.Lerp(regularColor, deathColor, timer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && currentState != BossState.DYING)
        {
            PlayerController playerControl = other.gameObject.GetComponent<PlayerController>();
            playerControl.ApplyEffect(PlayerController.PlayerState.STUNNED, stunDuration);
            landedHit = true;
        }
    }

    IEnumerator Jump(float duration, float cooldown)
    {
        currentState = BossState.JUMPING;
        timer = 0f;
        Vector3 target = (landedHit) ? spawnPos : player.transform.position;
        CalculateJumpMovement(target);
        yield return new WaitForSeconds(duration);
        lastRoutine = StartCoroutine(Rest(cooldown));
    }

    IEnumerator Rest(float duration)
    {
        currentState = BossState.RESTING;
        yield return new WaitForSeconds(duration);
        currentState = BossState.NEUTRAL;
    }

    void CalculateJumpMovement(Vector3 targetPos)
    {
        Vector3 toTarget = (targetPos - transform.position);
        toTarget.y = 0;

        speedMultiplier = Mathf.Lerp(maxMoveSpeed, moveSpeed, healthRatio);

        if (toTarget.magnitude * (1 + Mathf.Lerp(healthRatio + .75f, healthRatio, healthRatio)) < speedMultiplier)
        {
            float newValue = toTarget.magnitude * (1 + Mathf.Lerp(healthRatio + .75f, healthRatio, healthRatio));
            speedMultiplier = newValue;
        }
        
        moveDir = toTarget.normalized;

        transform.rotation = Quaternion.LookRotation(moveDir);

        landedHit = false;
    }

    void UpdateSize()
    {
        // Recalculate scale and jumpHeight w/ new healthRatio
        healthRatio = (float)currentHP / maxHP;

        float scaleAmount = Mathf.Lerp(minScale, originalScale, healthRatio);
        transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);

        float jump = Mathf.Lerp(minJumpHeight, jumpHeight, healthRatio);
        jumpCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(jumpDuration / 2f, jump), new Keyframe(jumpDuration, 0));

        int layerMask = LayerMask.GetMask("Terrain");

        // Ensure boss stays on the ground despite scale change (assumes scale only descreases)
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 10f, layerMask))
        {
            Debug.Log(hit.collider.gameObject.name);
            transform.position -= Vector3.up * (hit.distance - transform.localScale.x / 2) - yOffset * Vector3.up;
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentState != BossState.DYING)
        {
            if (!increaseDetectionRangeFlag)
            {
                increaseDetectionRangeFlag = true;
                detectionRange = 30;
            }
            currentHP -= amount;
            audioSource.Play();
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
        onDie.Invoke();
        disableWhenDie.SetActive(false);
        FindObjectOfType<HelpText>().SpawnDeathIcon(true, this.gameObject);
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
