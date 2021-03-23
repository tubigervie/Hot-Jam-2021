using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class DialogueEditor : EditorWindow
{
    Dialogue selectedDialogue = null;
    [NonSerialized]
    GUIStyle nodeStyle = null;
    [NonSerialized]
    GUIStyle playerNodeStyle = null;
    [NonSerialized]
    DialogueNode draggingNode = null;
    [NonSerialized]
    Vector2 draggingOffset;
    [NonSerialized]
    DialogueNode creatingNode = null;
    [NonSerialized]
    DialogueNode deletingNode = null;
    Vector2 scrollPosition;
    [NonSerialized]
    bool draggingCanvas = false;
    [NonSerialized]
    Vector2 draggingCanvasOffset;

    const float canvasSize = 4000;

    [MenuItem("Window/Dialogue Editor")] //call back, function called when clicking on this menu item
    public static void ShowEditorWindow()
    {
        GetWindow(typeof(DialogueEditor), false, "Dialogue Editor"); //false specifies it's not a utility window. Utility windows are for one-time actions (like generating stuff) and are not dockable
    }

    [OnOpenAssetAttribute(1)] //this should be called when opening an asset the int decides the ordering of callbacks 
    public static bool OnOpenAsset(int instanceID, int line)
    {
        Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
        if (dialogue != null)
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChanged;
        OnSelectionChanged();

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.padding = new RectOffset(20, 20, 20, 20);
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        playerNodeStyle = new GUIStyle();
        playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        playerNodeStyle.normal.textColor = Color.white;
        playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
        playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
    }

    void OnSelectionChanged()
    {
        Dialogue newDialogue = Selection.activeObject as Dialogue;
        if (newDialogue != null)
        {
            selectedDialogue = newDialogue;
            Repaint();
        }
        //selectedDialogue.onValidated += OnModelUpdated;

        OnModelUpdated();
    }

    void OnModelUpdated()
    {
        //ReloadNodes();
        Repaint();
    }

    private void OnGUI()
    {
        if (selectedDialogue == null)
        {
            EditorGUILayout.LabelField("No Dialogue selected.");
        }
        else
        {
            ProcessEvents();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayoutUtility.GetRect(canvasSize, canvasSize);


            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawConnections(node);
            }

            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawNode(node);
            }

            EditorGUILayout.EndScrollView();

            if (creatingNode != null)
            {
                selectedDialogue.CreateNode(creatingNode);
                creatingNode = null;
            }
            if (deletingNode != null)
            {
                selectedDialogue.DeleteNode(deletingNode);
                deletingNode = null;
            }
        }
    }

    private void ProcessEvents()
    {
        if (Event.current.type == EventType.MouseDown && draggingNode == null)
        {
            draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
            if (draggingNode != null)
            {
                draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                Selection.activeObject = draggingNode;
            }
            else
            {
                draggingCanvas = true;
                draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                Selection.activeObject = selectedDialogue;
            }
        }
        else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
        {

            draggingNode.ChangeRect(Event.current.mousePosition + draggingOffset);
            GUI.changed = true;
        }
        else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
        {
            scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
            GUI.changed = true;
        }
        else if (Event.current.type == EventType.MouseUp && draggingNode != null)
        {
            draggingNode = null;
        }
        else if (Event.current.type == EventType.MouseUp && draggingCanvas)
        {
            draggingCanvas = false;
        }

    }

    private DialogueNode GetNodeAtPoint(Vector2 point)
    {
        DialogueNode foundNode = null;
        foreach (DialogueNode node in selectedDialogue.GetAllNodes())
        {
            if (node.GetRect().Contains(point))
            {
                foundNode = node;
            }
        }
        return foundNode;
    }

    private void DrawNode(DialogueNode node)
    {
        GUIStyle style = nodeStyle;
        GUILayout.BeginArea(node.GetRect(), style);


        EditorGUI.PrefixLabel(new Rect(20, 4, 100, 15), 0, new GUIContent("Speaker:"));
        string newSpeaker = EditorGUILayout.TextField(node.GetSpeaker());
        node.ChangeSpeaker(newSpeaker);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUI.PrefixLabel(new Rect(20, 35, 100, 15), 0, new GUIContent("Text:"));
        string newText = EditorGUILayout.TextField(node.GetText());

        node.ChangeText(newText);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Delete"))
        {
            deletingNode = node;
        }
        if (GUILayout.Button("Add child"))
        {
            creatingNode = node;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        Texture2D newTexture = (Texture2D)EditorGUI.ObjectField(new Rect(50, 95, 90, 90),
        "",
        node.GetSpriteTexture(),
        typeof(Texture2D), true);

        node.ChangeSprite(newTexture);
        GUILayout.EndArea();
    }


    private void DrawConnections(DialogueNode node)
    {
        Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
        DialogueNode childNode = selectedDialogue.GetChild(node);
        if (childNode == null) return;
        Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
        Vector3 controlPointOffset = endPosition - startPosition;
        controlPointOffset.y = 0;
        controlPointOffset.x *= .8f;
        Handles.DrawBezier(
            startPosition, endPosition,
            startPosition + controlPointOffset,
            endPosition - controlPointOffset,
            Color.white, null, 4f);
    }
}

