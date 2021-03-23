using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Teleporter destination;
    [SerializeField] GameObject teleportFX;
    [SerializeField] bool isActive = true;
    [SerializeField] float _cooldown = 1f;
    [SerializeField] float transitionCoolDown = .4f;

    public float cooldown { get { return _cooldown; } }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.gameObject.tag == "Player")
        {
            if (destination == null) return;
            StartCoroutine(TeleportCoroutine(other));
        }
    }

    private IEnumerator TeleportCoroutine(Collider other)
    {
        PlayerController charControl = other.gameObject.GetComponent<PlayerController>();
        charControl.ApplyEffect(PlayerController.PlayerState.TELEPORT);
        charControl.GetComponent<CharacterController>().enabled = false;
        if (teleportFX != null)
            Instantiate(teleportFX, transform.position + Vector3.up * 1.1f, Quaternion.identity);
        StartCooldown(cooldown);
        destination.StartCooldown(destination.cooldown);
        yield return new WaitForSeconds(transitionCoolDown);
        other.transform.position = destination.transform.position + Vector3.up * 1.5f;
        if (teleportFX != null)
            Instantiate(teleportFX, destination.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        yield return new WaitForSeconds(transitionCoolDown);
        charControl.GetComponent<CharacterController>().enabled = true;
        charControl.ApplyEffect(PlayerController.PlayerState.NORMAL);
    }

    public void TurnOn(bool shouldEnable)
    {
        isActive = shouldEnable;
    }

    public void StartCooldown(float seconds)
    {
        StartCoroutine(CooldownDuration(seconds));
    }

    IEnumerator CooldownDuration(float seconds)
    {
        isActive = false;
        yield return new WaitForSeconds(seconds);
        isActive = true;
    }
}
