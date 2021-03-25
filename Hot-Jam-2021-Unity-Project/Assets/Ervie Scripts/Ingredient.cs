using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "Resources/New Ingredient")]
[System.Serializable]
public class Ingredient : ScriptableObject
{
    [SerializeField] string _displayName;
    [SerializeField] Sprite _sprite;
    [SerializeField] IngredientPickup _pickup;

    public string displayName { get { return _displayName; } }

    public Sprite sprite { get { return _sprite; } }

    public IngredientPickup SpawnPickup(Vector3 position)
    {
        var pickup = Instantiate(_pickup);
        pickup.ingredient = this;
        pickup.transform.position = position;
        return pickup;
    }
}
