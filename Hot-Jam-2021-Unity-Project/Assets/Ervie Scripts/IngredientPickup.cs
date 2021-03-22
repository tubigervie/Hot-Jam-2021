using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientPickup : MonoBehaviour, IPickable
{
    public Ingredient ingredient = null;

    public void Hide()
    {
        ShowPickup(false);
    }

    public void HideForSeconds(float time)
    {
        StartCoroutine(HideForSecondsCoroutine(time));
    }

    private IEnumerator HideForSecondsCoroutine(float time)
    {
        ShowPickup(false);
        yield return new WaitForSeconds(time);
        ShowPickup(true);
    }

    public Ingredient PickUp()
    {
        HideForSeconds(5f);
        return ingredient;
    }

    private void ShowPickup(bool shouldShow)
    {
        GetComponent<Collider>().enabled = shouldShow;
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(shouldShow);
        }
    }
}
