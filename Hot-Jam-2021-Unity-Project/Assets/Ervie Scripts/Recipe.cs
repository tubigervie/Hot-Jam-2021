﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Recipe", menuName = "Resources/New Recipe")]
public class Recipe : ScriptableObject
{
    public List<Ingredient> ingredients;

    public bool requireInOrder { get { return _requireInOrder; } } 

    [SerializeField] bool _requireInOrder = true;
    [SerializeField] Ingredient wildcardIngredient;

    public bool CheckForCompletion(List<Ingredient> currentIngredients)
    {
        if (currentIngredients.Count < ingredients.Count) return false;
        if (_requireInOrder)
        {
            for (int i = 0; i < ingredients.Count; i++)
            {
                if (currentIngredients[i] != ingredients[i] && currentIngredients[i] != wildcardIngredient)
                    return false;
            }
            return true;
        }
        else
        {
            return SequenceEqualsIgnoreOrder(ingredients, currentIngredients);
        }
    }

    public bool CheckForRightIngredientAtIndex(Ingredient currentIngredient, int index)
    {
        return (ingredients[index] == currentIngredient || ingredients[index] == wildcardIngredient);
    }

    public bool CheckAndRemoveIfContains(ref List<Ingredient> remainingIngredients, Ingredient currentIngredient)
    {
        if (!remainingIngredients.Contains(currentIngredient))
            return false;
        remainingIngredients.Remove(currentIngredient);
        return true;
    }

    private static bool SequenceEqualsIgnoreOrder<T>(IEnumerable<T> list1, IEnumerable<T> list2, IEqualityComparer<T> comparer = null, Ingredient wildcardIngredient = null)
    {
        int wildcardCount = 0;
        if (list1 is ICollection<T> ilist1 && list2 is ICollection<T> ilist2 && ilist1.Count != ilist2.Count)
            return false;

        if (comparer == null)
            comparer = EqualityComparer<T>.Default;

        var itemCounts = new Dictionary<T, int>(comparer);
        foreach (T s in list1)
        {
            if (itemCounts.ContainsKey(s))
            {
                itemCounts[s]++;
            }
            else
            {
                itemCounts.Add(s, 1);
            }
        }
        foreach (T s in list2)
        {
            if (itemCounts.ContainsKey(s))
            {
                itemCounts[s]--;
            }
            else if (wildcardIngredient != null && s.GetType().Equals(typeof(Ingredient)))
            {
                if (s as Ingredient == wildcardIngredient)
                {
                    wildcardCount++;    // count the number of wildcards
                }
            }
            else
            {
                return false;
            }
        }
        foreach (T s in list1)  // remove all extraneous items until wildcards run out
        {
            if (itemCounts[s] > 0 && wildcardCount > 0 && itemCounts[s] <= wildcardCount)
            {
                wildcardCount -= itemCounts[s];
                itemCounts[s] = 0;
            }
            else
            {
                return false;
            }
        }
        return itemCounts.Values.All(c => c == 0);
    }
}
