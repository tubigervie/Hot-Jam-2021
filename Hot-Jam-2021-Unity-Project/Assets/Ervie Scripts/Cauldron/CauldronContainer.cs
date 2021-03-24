using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronContainer : MonoBehaviour, IInteractable
{

    [SerializeField] List<Ingredient> storedIngredients = new List<Ingredient>(); //might change this
    [SerializeField] Recipe cauldronRecipe;
    [SerializeField] int index = 0;

    bool isComplete = false;
    List<Ingredient> remainingIngredients = new List<Ingredient>();
    CauldronAI cauldronAI;

    [Header("Test Render Materials - will remove")]
    [SerializeField] Material completeMaterial;

    public Action onRecipeComplete;

    void Awake()
    {
        cauldronAI = GetComponent<CauldronAI>();
        onRecipeComplete += cauldronAI.Complete;
    }

    private void Start()
    {
        PopulateRequiredIngredients();
    }

    private void PopulateRequiredIngredients()
    {
        foreach (Ingredient ingredient in cauldronRecipe.ingredients)
        {
            remainingIngredients.Add(ingredient);
        }
    }

    public void PlaceIngredient(Ingredient ingredient)
    {
        if (isComplete)
        {
            Debug.Log("Cauldron full!");
            return;
        }
        if (cauldronRecipe.requireInOrder && !cauldronRecipe.CheckForRightIngredientAtIndex(ingredient, index))
        {
            Debug.Log("Wrong ingredient or not right order!");
        }
        else if(!cauldronRecipe.requireInOrder && !cauldronRecipe.CheckAndRemoveIfContains(ref remainingIngredients, ingredient))
        {
            Debug.Log("Wrong ingredient!");
        }
        else
        {
            storedIngredients.Add(ingredient);
            index++;
            if (cauldronRecipe.CheckForCompletion(storedIngredients))
            {
                Debug.Log("Recipe complete!");
                isComplete = true;
                GetComponentInChildren<MeshRenderer>().material = completeMaterial;
                onRecipeComplete();
            }
            else
            {
                Debug.Log($"Added ingredient: {ingredient}!");
            }
        }
    }

    public void Interact(GameObject player)
    {
        //Ingredient currentItem = player.GetComponent<TestPickUpController>().currentPickedUpItem;
        if (cauldronAI.currentState == CauldronAI.CauldronState.Wandering)
        {
            InteractionController interactionController = player.GetComponent<InteractionController>();
            Ingredient currentItem = interactionController.currentIngredient;
            if (currentItem != null)
            {
                interactionController.DropIngredient();
            }
            interactionController.SetCauldronSlot(this);
            interactionController.interactable = null;
            cauldronAI.Carry();
        }
        else if (cauldronAI.currentState == CauldronAI.CauldronState.Idle)
        {
            Ingredient currentItem = player.GetComponent<InteractionController>().currentIngredient;
            if (currentItem == null) return;
            PlaceIngredient(currentItem);
            //player.GetComponent<TestPickUpController>().SetCurrentPickedItem(null);
            player.GetComponent<InteractionController>().SetCurrentItem(null);
        }
    }
}
