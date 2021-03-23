using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue", order = 0)]
public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();
    [SerializeField] Vector2 newNodeOffset = new Vector2(250, 0);

    Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

    private void OnValidate()
    {
        nodeLookup.Clear();
        foreach(DialogueNode node in GetAllNodes())
        {
            nodeLookup[node.name] = node;
        }
    }


    public IEnumerable<DialogueNode> GetAllNodes()
    {
        return nodes;
    }

    public DialogueNode GetRootNode()
    {
        return nodes[0];
    }

    public DialogueNode GetChild(DialogueNode parentNode)
    {
        string childNodeID = parentNode.GetChildId();
        if (string.IsNullOrEmpty(childNodeID)) return null;
        if (nodeLookup.ContainsKey(childNodeID))
            return nodeLookup[childNodeID];
        return null;
    }

#if UNITY_EDITOR  //preprocessor directiv
    public void CreateNode(DialogueNode parent)
    {
        DialogueNode newChild = MakeNode(parent);
        Undo.RegisterCreatedObjectUndo(newChild, "Created Dialogue Node");
        Undo.RecordObject(this, "Added Dialogue Node");
        AddNode(newChild);
    }

    private void AddNode(DialogueNode newChild)
    {
        nodes.Add(newChild);
        OnValidate();
    }

    private DialogueNode MakeNode(DialogueNode parent)
    {
        DialogueNode newChild = ScriptableObject.CreateInstance<DialogueNode>();
        newChild.name = Guid.NewGuid().ToString();
        if (parent != null)
        {
            string currentChildId = parent.GetChildId();
            DialogueNode currentChild = null;
            if (!string.IsNullOrEmpty(currentChildId))
            {
                if (nodeLookup.ContainsKey(currentChildId))
                    currentChild = nodeLookup[currentChildId];
            }
            parent.AddChild(newChild.name);
            if(currentChild != null)
                newChild.child = currentChild.name;
            newChild.ChangeRect(parent.GetRect().position + newNodeOffset);
        }
        return newChild;
    }

    public void DeleteNode(DialogueNode nodeToDelete)
    {
        Undo.RecordObject(this, "Deleted Dialogue Node");
        nodes.Remove(nodeToDelete);
        OnValidate();
        CleanDanglingChildren(nodeToDelete);
        Undo.DestroyObjectImmediate(nodeToDelete);
    }

    private void CleanDanglingChildren(DialogueNode nodeToDelete)
    {
        foreach (DialogueNode node in GetAllNodes())
        {
            if (node.child == nodeToDelete.name)
            {
                string nodeToDeleteChildId = nodeToDelete.GetChildId();
                DialogueNode nodeToDeleteChild = null;
                if (!string.IsNullOrEmpty(nodeToDeleteChildId))
                {
                    if (nodeLookup.ContainsKey(nodeToDeleteChildId))
                        nodeToDeleteChild = nodeLookup[nodeToDeleteChildId];
                }
                node.RemoveChild(nodeToDelete.name);
                node.child = nodeToDeleteChildId;
            }
        }
    }

    public void OnBeforeSerialize()
    {
        if (nodes.Count == 0)
        {
            DialogueNode newChild = MakeNode(null);
            AddNode(newChild);
        }

        string assetPath = AssetDatabase.GetAssetPath(this);
        if (!String.IsNullOrEmpty(assetPath))
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                if (AssetDatabase.GetAssetPath(node) == "")
                    AssetDatabase.AddObjectToAsset(node, this);
            }
        }
#endif
    }

    public void OnAfterDeserialize()
    {
    }
}
