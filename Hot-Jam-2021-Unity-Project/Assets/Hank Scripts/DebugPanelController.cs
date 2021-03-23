using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanelController : MonoBehaviour
{
    [SerializeField] Text heldObjLabel;
    [SerializeField] Text detectedObjLabel;
    [SerializeField] Text playerStateLabel;

    public void UpdateHeldObj(string objName)
    {
        heldObjLabel.text = $"HeldObj: {objName}";
    }

    public void UpdateDetectedObj(string objName)
    {
        detectedObjLabel.text = $"DetectedObj: {objName}";
    }

    public void UpdatePlayerState(PlayerController.PlayerState state)
    {
        string text = "";
        switch(state)
        {
            case PlayerController.PlayerState.NORMAL:
                text = "NORMAL";
                break;
            case PlayerController.PlayerState.SLOWED:
                text = "SLOWED";
                break;
            case PlayerController.PlayerState.STUNNED:
                text = "STUNNED";
                break;
            case PlayerController.PlayerState.ACTION:
                text = "ACTION";
                break;
            default:
                text = "N/A";
                break;
        }
        playerStateLabel.text = $"PlayerState: {text}";
    }

    public void TogglePanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
