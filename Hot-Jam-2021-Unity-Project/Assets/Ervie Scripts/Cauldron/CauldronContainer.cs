using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronContainer : MonoBehaviour, IInteractable
{

    [SerializeField] List<Ingredient> storedIngredients = new List<Ingredient>(); //might change this
    [SerializeField] Ingredient tutorialIngredient;
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
        if(cauldronAI.currentState == CauldronAI.CauldronState.Pregame && ingredient.name == "Mandrake")
        {
            FindObjectOfType<LevelManager>().OnTutorialComplete();
            return;
        }
        if (isComplete)
        {
            Debug.Log("Cauldron full!");
            return;
        }
        if (cauldronRecipe.requireInOrder && !cauldronRecipe.CheckForRightIngredientAtIndex(ingredient, index))
        {
            Debug.Log("Here!");
            onWrongIngredientReceived.Invoke();
            StartCoroutine(DropIngredient(ingredient));
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
        else if(cauldronAI.currentState == CauldronAI.CauldronState.Pregame)
        {
            Ingredient currentItem = player.GetComponent<InteractionController>().currentIngredient;
            if (currentItem == null) return;
            PlaceIngredient(currentItem);
            player.GetComponent<InteractionController>().SetCurrentItem(null);
        }
    }

    IEnumerator DropIngredient(Ingredient ingredient)
    {
        Debug.Log("Here we are!");
        AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(.5f, 1f), new Keyframe(1f, -1.25f));
        float timer = 0f;

        GameObject pickup = ingredient.SpawnPickup(transform.position + Vector3.up).gameObject;
        BoxCollider collisionBox = pickup.GetComponent<BoxCollider>();
        collisionBox.enabled = false;
        // Do the animation

        float moveSpeed = 2f;
        float yOffset = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            float newValue = curve.Evaluate(timer);
            pickup.transform.position += transform.forward * moveSpeed * Time.deltaTime + Vector3.up * (newValue - yOffset);
            yield return new WaitForFixedUpdate();
            yOffset = newValue;
        }
        collisionBox.enabled = true;
    }
}
