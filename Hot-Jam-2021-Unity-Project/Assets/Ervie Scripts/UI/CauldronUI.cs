using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CauldronUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Image fillImage;

    CauldronAI cauldron;
    Animator anim;

    [SerializeField] Color startColor;
    [SerializeField] Color endColor;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        cauldron = FindObjectOfType<CauldronAI>();
        if(cauldron != null)
        {
            cauldron.onCauldronStart += ToggleImageElementsOn;
            cauldron.onUpdateTimer += UpdateCauldronUI;
        }
        ToggleImageElements(false);
    }

    private void UpdateCauldronUI()
    {
        float boilTime = cauldron.GetBoilTimer();
        float minutes = Mathf.FloorToInt(boilTime / 60);
        float seconds = Mathf.FloorToInt(boilTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        float ratio = cauldron.GetBoilTimer() / cauldron.GetTotalBoilTimer();
        if (1 - ratio >= .65f)
        {
            anim.SetBool("Rush", true);
            anim.SetFloat("Blend", 1 - ratio);
        }
        else
        {
            anim.SetBool("Rush", false);
            anim.SetFloat("Blend", 0);
        }
        fillImage.fillAmount = 1 - ratio;
        fillImage.color = Color.Lerp(startColor, endColor, 1 - ratio);
    }

    private void ToggleImageElementsOn()
    {
        UpdateCauldronUI();
        ToggleImageElements(true);
    }

    private void ToggleImageElements(bool flag)
    {
        foreach(Transform child in this.transform)
        {
            child.gameObject.SetActive(flag);
        }
    }

}
