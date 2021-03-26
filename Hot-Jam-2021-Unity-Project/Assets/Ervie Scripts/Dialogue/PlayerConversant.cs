using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerConversant : MonoBehaviour
{
    Dialogue currentDialogue;
    //AIConversant currentConversant = null;
    DialogueNode currentNode = null;

    public UnityAction onConversationUpdated;

    public bool IsActive()
    {
        return currentDialogue != null;
    }

    public void StartDialogue(Dialogue newDialogue)
    {
        if (newDialogue == null) return;
        if (currentDialogue == newDialogue) return;
        currentDialogue = newDialogue;
        if (currentDialogue != null)
        {
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();
        }
    }

    public void QueueDialogue(Dialogue newDialogue, float delayTimer)
    {
        if (newDialogue == null) return;
        StartCoroutine(DelayDialogueCoroutine(newDialogue, delayTimer));
    }

    private IEnumerator DelayDialogueCoroutine(Dialogue newDialogue, float delayTimer)
    {
        yield return new WaitForSeconds(delayTimer);
        StartDialogue(newDialogue);
    }

    public void Quit()
    {
        currentDialogue = null;
        TriggerExitAction();
        currentNode = null;
        onConversationUpdated();
    }

    // Start is called before the first frame update
    public string GetText()
    {
        if (currentNode == null)
            return "";
        return currentNode.GetText();
    }

    public string GetCurrentConversantName()
    {
        return currentNode.GetSpeaker();
    }

    public Texture2D GetConversantSprite()
    {
        return currentNode.GetSpriteTexture();
    }

    public AudioClip GetDialogueFX()
    {
        if (currentNode == null) return null;
        return currentNode.dialogueSpriteSO.GetDialogueClip();
    }

    public float GetDialogueVolume()
    {
        return currentNode.dialogueSpriteSO.GetVolume();
    }

    public Vector2 GetSpritePosition()
    {
        return currentNode.dialogueSpriteSO.GetSpritePosition();
    }

    public Vector2 GetDialogueSpriteSize()
    {
        return currentNode.dialogueSpriteSO.GetSpriteSize();
    }

    public void Next()
    {
        DialogueNode child = currentDialogue.GetChild(currentNode);
        if (child != null)
        {
            TriggerExitAction();
            currentNode = child;
            TriggerEnterAction();
            onConversationUpdated();
        }
        else
        {
            Quit();
        }
    }

    public bool HasNext()
    {
        return (currentDialogue.GetChild(currentNode) != null);
    }

    private void TriggerEnterAction()
    {
        if(currentNode != null && currentNode.GetOnEnterActions().Count > 0)
        {
            foreach(string action in currentNode.GetOnEnterActions())
                TriggerAction(action);
        }
    }

    private void TriggerExitAction()
    {
        if (currentNode != null && currentNode.GetOnExitActions().Count > 0)
        {
            foreach (string action in currentNode.GetOnExitActions())
                TriggerAction(action);
        }
    }

    private void TriggerAction(string action)
    {
        if (action == "") return;
        foreach(DialogueTrigger trigger in this.GetComponents<DialogueTrigger>())
        {
            trigger.Trigger(action);
        }
    }

    //private IEnumerator<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
    //{
    //    foreach(var node in inputNode)
    //    {
    //        ifnode.
    //    }
    //}
}

