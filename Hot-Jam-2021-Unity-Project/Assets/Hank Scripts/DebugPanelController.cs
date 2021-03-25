using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanelController : MonoBehaviour
{
    [SerializeField] Text heldObjLabel;
    [SerializeField] Text detectedObjLabel;
    [SerializeField] Text playerStateLabel;
    [SerializeField] Text cauldronStateLabel;
    [SerializeField] Text cauldronTimerLabel;
    [SerializeField] Text boilTimerLabel;

    private void Start()
    {
    }

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
        switch (state)
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
            case PlayerController.PlayerState.INDIALOGUE:
                text = "INDIALOGUE";
                break;
            case PlayerController.PlayerState.TELEPORT:
                text = "TELEPORT";
                break;
            default:
                text = "N/A";
                break;
        }
        playerStateLabel.text = $"PlayerState: {text}";
    }

    public void UpdateCauldronState(CauldronAI.CauldronState state)
    {
        string text = "";
        switch (state)
        {
            case CauldronAI.CauldronState.Idle:
                text = "IDLE";
                break;
            case CauldronAI.CauldronState.Wandering:
                text = "WANDERING";
                break;
            case CauldronAI.CauldronState.Carried:
                text = "CARRIED";
                break;
            case CauldronAI.CauldronState.Complete:
                text = "COMPLETE";
                break;
        }
        cauldronStateLabel.text = $"CauldronState: {text}";
    }

    public void UpdateCauldronTimer(float time)
    {
        cauldronTimerLabel.text = $"WanderTimer: {time}";
    }

    public void UpdateBoilTimer(float time)
    {
        boilTimerLabel.text = $"BoilTimer: {time}";
    }

    public void TogglePanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
