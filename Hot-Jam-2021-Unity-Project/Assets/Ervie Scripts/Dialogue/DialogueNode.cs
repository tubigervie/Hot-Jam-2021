using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueNode : ScriptableObject
{
    [SerializeField]
    private string speaker;

    [TextArea]
    [SerializeField]
    private string text;

    [SerializeField]
    public string child;

    [SerializeField]
    private Texture2D spriteTexture;

    public DialogueSpriteSO dialogueSpriteSO;

    [SerializeField]
    private Rect rect = new Rect(10, 10, 200, 200);

    [SerializeField]
    private List<string> onEnterActions = new List<string>();

    [SerializeField]
    private List<string> onExitActions = new List<string>();

    public string GetText()
    {
        return text;
        //-353, 32, 500, 750
    }
    public string GetSpeaker()
    {
        return speaker;
    }
    public Rect GetRect()
    {
        return rect;
    }

    public string GetChildId()
    {
        return child;
    }

    public Texture2D GetSpriteTexture()
    {
        return spriteTexture;
    }

    public List<string> GetOnEnterActions()
    {
        return onEnterActions;
    }

    public List<string> GetOnExitActions()
    {
        return onExitActions;
    }

#if UNITY_EDITOR
    public void ChangeRect(Vector2 newPosition)
    {

        Undo.RecordObject(this, "Move Dialogue Node");
        rect.position = newPosition;
        EditorUtility.SetDirty(this);

    }
    public void ChangeText(string newText)
    {
        if (newText != text)
        {
            Undo.RecordObject(this, "Update Dialogue Text");
            text = newText;
            EditorUtility.SetDirty(this);
        }
    }

    public void ChangeSpeaker(string newSpeaker)
    {
        if (newSpeaker != speaker)
        {
            Undo.RecordObject(this, "Update Speaker Text");
            speaker = newSpeaker;
            EditorUtility.SetDirty(this);
        }
    }

    public void ChangeSprite(Texture2D newSpriteTexture)
    {
        if(newSpriteTexture != spriteTexture)
        {
            Undo.RecordObject(this, "Update Sprite Texture");
            spriteTexture = newSpriteTexture;
            EditorUtility.SetDirty(this);
        }
    }

    public void RemoveChild(string name)
    {
        Undo.RecordObject(this, "Remove Dialogue Link");
        this.child = "";
        EditorUtility.SetDirty(this);
    }

    public void AddChild(string name)
    {
        Undo.RecordObject(this, "Add Dialogue Link");
        this.child = name;
        EditorUtility.SetDirty(this);
    }

#endif
}
