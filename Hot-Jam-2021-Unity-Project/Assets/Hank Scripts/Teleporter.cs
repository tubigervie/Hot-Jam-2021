using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Teleporter destination;
    [SerializeField] GameObject teleportFX;
    [SerializeField] bool isActive = true;
    [SerializeField] float _cooldown = 1f;

    public float cooldown { get { return _cooldown; } }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.gameObject.tag == "Player")
        {
            destination.StartCooldown(destination.cooldown);
            StartCooldown(cooldown);

            CharacterController charControl = other.gameObject.GetComponent<CharacterController>();
            charControl.enabled = false;
            other.transform.position = destination.transform.position + Vector3.up * 1.5f;
            charControl.enabled = true;

            Instantiate(teleportFX, destination.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        }
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
