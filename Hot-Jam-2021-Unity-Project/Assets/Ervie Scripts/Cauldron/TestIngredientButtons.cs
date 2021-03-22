using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestIngredientButtons : MonoBehaviour
{
    [SerializeField] Ingredient ingredient;
    [SerializeField] CauldronContainer cauldron;
    Text buttonText;

    private void Awake()
    {
        buttonText = GetComponentInChildren<Text>();
        buttonText.text = ingredient.displayName;
    }

    public void OnButtonClick()
    {
        cauldron.PlaceIngredient(ingredient);
    }
    
}
