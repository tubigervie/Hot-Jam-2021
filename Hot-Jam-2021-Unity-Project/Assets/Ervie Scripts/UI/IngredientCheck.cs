using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCheck : MonoBehaviour
{
    public Ingredient ingredient;
    [SerializeField] GameObject checkMark;

    private void Start()
    {
        checkMark.SetActive(false);
    }

    public void EnableCheck()
    {
        checkMark.SetActive(true);
    }
}
