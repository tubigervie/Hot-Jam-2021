using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanelController : MonoBehaviour
{
    [SerializeField] Text heldObjLabel;
    [SerializeField] Text detectedObjLabel;

    public void UpdateHeldObj(string objName)
    {
        heldObjLabel.text = $"HeldObj: {objName}";
    }

    public void UpdateDetectedObj(string objName)
    {
        detectedObjLabel.text = $"DetectedObj: {objName}";
    }

    public void TogglePanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
