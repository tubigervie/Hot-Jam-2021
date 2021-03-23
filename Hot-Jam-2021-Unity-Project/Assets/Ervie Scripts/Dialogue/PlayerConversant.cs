using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConversant : MonoBehaviour
{
    [SerializeField] Dialogue testDialogue;
    Dialogue currentDialogue;
    //AIConversant currentConversant = null;
    DialogueNode currentNode = null;

    public event Action onConversationUpdated;

    public bool IsActive()
    {
        return currentDialogue != null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartDialogue(testDialogue);
        }
    }

    public void StartDialogue(Dialogue newDialogue)
    {
        if (currentDialogue == newDialogue) return;
        currentDialogue = newDialogue;
        if (currentDialogue != null)
        {
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();
        }
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
        if(currentNode != null && currentNode.GetOnEnterAction() != "")
        {
            TriggerAction(currentNode.GetOnEnterAction());
        }
    }

    private void TriggerExitAction()
    {
        if (currentNode != null && currentNode.GetOnExitAction() != "")
        {
            TriggerAction(currentNode.GetOnExitAction());
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

