using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Sprite Data", menuName = "Character Sprite Data", order = 0)]
[System.Serializable]
public class DialogueSpriteSO : ScriptableObject
{
    [SerializeField] Vector2 dialogueSpriteSize = new Vector2(600, 850);
    [SerializeField] Vector2 dialogueSpritePosition = new Vector2(-366, 114);
    [SerializeField] AudioClip dialogueSFX;
    [SerializeField] float dialogueVolume = .1f;

    public Vector2 GetSpriteSize()
    {
        return dialogueSpriteSize;
    }

    public Vector2 GetSpritePosition()
    {
        return dialogueSpritePosition;
    }

    public AudioClip GetDialogueClip()
    {
        return dialogueSFX;
    }

    public float GetVolume()
    {
        return dialogueVolume;
    }
}
