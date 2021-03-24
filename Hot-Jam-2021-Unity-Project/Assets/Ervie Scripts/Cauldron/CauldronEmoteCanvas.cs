using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CauldronEmoteCanvas : MonoBehaviour
{
    [SerializeField] Sprite angryIcon;
    [SerializeField] Sprite happyIcon;
    [SerializeField] Image emoteIconImage;
    [SerializeField] CauldronContainer container;
    [SerializeField] float displayTime = 3f;

    private void Awake()
    {
        emoteIconImage.enabled = false;
    }

    private void Start()
    {
        container.onCorrectIngredientReceived += DisplayHappyIcon;
        container.onWrongIngredientReceived += DisplayAngryIcon;
    }

    private void Update()
    {
        if (emoteIconImage.enabled)
        {
            emoteIconImage.transform.position = Camera.main.WorldToScreenPoint(container.transform.position + Vector3.up * 2.5f);
        }
    }

    public void DisplayHappyIcon()
    {
        StartCoroutine(DisplayIconCoroutine(happyIcon));
    }

    public void DisplayAngryIcon()
    {
        StartCoroutine(DisplayIconCoroutine(angryIcon));
    }

    private IEnumerator DisplayIconCoroutine(Sprite iconToDisplay)
    {
        emoteIconImage.sprite = iconToDisplay;
        emoteIconImage.transform.position = Camera.main.WorldToScreenPoint(container.transform.position + Vector3.up * 2.5f);
        emoteIconImage.enabled = true;
        yield return new WaitForSeconds(displayTime);
        emoteIconImage.enabled = false;
    }

}
