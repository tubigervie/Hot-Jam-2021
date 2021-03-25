using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalDeposit : MonoBehaviour, IInteractable
{
    [SerializeField] bool isReal;
    [SerializeField] float explodeDelay = 3f;
    [SerializeField] float stunRange = 5f;
    [SerializeField] float stunDuration = 1.5f;
    [SerializeField] GameObject explosionFX;
    [SerializeField] GameObject crystalPickup;

    [SerializeField] float shakeMagnitude = .05f;

    GameObject player;

    bool isShaking = false;

    float timer = 0f;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        ShowStunRangeInScene();
        
        if (isShaking)
        {
            timer += Time.deltaTime;
            float magnitude = Mathf.Sin(timer * 50) * shakeMagnitude;
            Vector3 horizontalOffset = transform.right * magnitude + transform.forward * magnitude;
            transform.position += horizontalOffset;
        }
    }

    void ShowStunRangeInScene()
    {
        Debug.DrawRay(transform.position, Vector3.forward * stunRange, Color.red);
        Debug.DrawRay(transform.position, Vector3.up * stunRange, Color.red);
        Debug.DrawRay(transform.position, Vector3.right * stunRange, Color.red);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, stunRange);
    }

    public void Interact(GameObject player)
    {
        StartCoroutine(SetFuse(explodeDelay));
        if (isReal)
        {
            Instantiate(crystalPickup, transform.position, Quaternion.identity);
        }
    }

    IEnumerator SetFuse(float fuseTime)
    {
        Debug.Log("Fuse set!");
        isShaking = true;
        yield return new WaitForSeconds(fuseTime);
        isShaking = false;
        Explode();
    }

    void Explode()
    {
        Debug.Log("Explode!");
        Instantiate(explosionFX, transform.position, Quaternion.identity);

        foreach(Collider c in Physics.OverlapSphere(transform.position, stunRange))
        {
            if (c.gameObject.tag == "Player")
            {
                PlayerController playerControl = player.GetComponent<PlayerController>();
                playerControl.ApplyEffect(PlayerController.PlayerState.STUNNED, stunDuration);
            }
        }
        Destroy(this.gameObject);
    }
}
