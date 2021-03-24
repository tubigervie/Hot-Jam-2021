﻿using System;
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
    public Action onCorrectIngredientReceived;
    public Action onWrongIngredientReceived;

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
            onWrongIngredientReceived.Invoke();
        }
        else if(!cauldronRecipe.requireInOrder && !cauldronRecipe.CheckAndRemoveIfContains(ref remainingIngredients, ingredient))
        {
            onWrongIngredientReceived.Invoke();
        }
        else
        {
            storedIngredients.Add(ingredient);
            index++;
            if (cauldronRecipe.CheckForCompletion(storedIngredients))
            {
                onCorrectIngredientReceived.Invoke();
                isComplete = true;
                GetComponentInChildren<MeshRenderer>().material = completeMaterial;
                onRecipeComplete();
            }
            else
            {
                onCorrectIngredientReceived.Invoke();
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
