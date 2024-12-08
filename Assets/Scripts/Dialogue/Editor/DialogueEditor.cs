using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Linq;
using System;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        [NonSerialized]
        NodeDraggingProperties nodeDragging = new NodeDraggingProperties();
        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        GUIStyle playerNodeStyle;
        [NonSerialized]
        DialogueNode spawningNode = null;
        [NonSerialized]
        DialogueNode deletingNode = null;
        [NonSerialized]
        DialogueNode linkingParentNode = null;
        [NonSerialized]
        bool isDraggingCanvas = false;
        [NonSerialized]
        Vector2 draggingCanvasOffset;

        [NonSerialized]
        Vector2 bottomRight = Vector2.zero;
        Vector2 scrollPosition;

        const int paddingX = 20;
        const int paddingY = 20;
        const int connectionOffset = 7;
        const float lineThickness = 4f;
        const float backgroundSize = 50f;

        [MenuItem("Window/Dialogue Editor")]
        private static void ShowEditorWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.titleContent = new GUIContent("Dialogue");
            window.Show();
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue asset = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (asset == null) return false;

            ShowEditorWindow();
            return true;
        }

        public void SetSelectedDialogue(Dialogue dialogue)
        {
            selectedDialogue = dialogue;
        }

        private void OnGUI()
        {
            RefreshViewRect();
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No dialogue selected.");
            }
            else
            {
                ProcessEvents();
                DrawGUI();
                AddOrRemoveNodes();
            }
        }

        private void DrawGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawBackground();
            DrawNodesAndLines();
            EditorGUILayout.EndScrollView();
        }

        private void DrawBackground()
        {
            if (bottomRight.x != 0f && bottomRight.y != 0f)
            {
                GUILayoutUtility.GetRect(bottomRight.x, bottomRight.y);
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                Rect textureCoords = new Rect(0, 0, _rect.width / backgroundSize, _rect.height / backgroundSize);
                GUI.DrawTextureWithTexCoords(_rect, backgroundTexture, textureCoords);
            }
        }

        private void AddOrRemoveNodes()
        {
            if (spawningNode != null)
            {
                selectedDialogue.CreateNode(spawningNode);
                spawningNode = null;
            }
            if (deletingNode != null)
            {
                selectedDialogue.DeleteNode(deletingNode);
                deletingNode = null;
            }
        }

        private void DrawNodesAndLines()
        {
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawConnections(node);
            }
            // Clear this out so we get the new bottom right every time.
            bottomRight = Vector2.zero;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawNode(node);
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle nodeStyle = this.nodeStyle;
            if (node.IsPlayerSpeaking())
            {
                nodeStyle = playerNodeStyle;
            }
            Rect nodeRect = node.GetRect();
            bottomRight.x = nodeRect.xMax > bottomRight.x ? nodeRect.xMax : bottomRight.x;
            bottomRight.y = nodeRect.yMax > bottomRight.y ? nodeRect.yMax : bottomRight.y;

            GUILayout.BeginArea(nodeRect, nodeStyle);

            DrawNodeTop(node);
            DrawNodeEditors(node);
            DrawNodeBottom(node);

            GUILayout.EndArea();
        }

        private void DrawNodeTop(DialogueNode node)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("x", GUILayout.Width(23)))
            {
                deletingNode = node;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawNodeEditors(DialogueNode node)
        {
            node.Text = EditorGUILayout.TextField(node.Text);
        }

        private void DrawNodeBottom(DialogueNode node)
        {
            GUILayout.BeginHorizontal();

            DrawLinkChildButtons(node);

            if (GUILayout.Button("+"))
            {
                spawningNode = node;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawLinkChildButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("[link]"))
                {
                    linkingParentNode = node;
                }
            }
            else
            {
                if (node.name == linkingParentNode.name)
                {

                    if (GUILayout.Button("[Cancel]"))
                    {
                        linkingParentNode = null;
                    }
                }
                else if (node.ResponseExists(linkingParentNode.name))
                {
                    if (GUILayout.Button("[unlink]"))
                    {
                        node.RemoveResponseID(linkingParentNode.name);
                        linkingParentNode = null;
                    }
                }
                else if (linkingParentNode.ResponseExists(node.name))
                {
                    if (GUILayout.Button("[unlink]"))
                    {
                        linkingParentNode.RemoveResponseID(node.name);
                        linkingParentNode = null;
                    }
                }
                else
                {
                    if (GUILayout.Button("[child]"))
                    {
                        linkingParentNode.AddResponseID(node.name);
                        linkingParentNode = null;
                    }
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            // center of right side
            Rect nodeRect = node.GetRect();
            Vector3 startPosition = new Vector3(nodeRect.xMax - connectionOffset, nodeRect.center.y);

            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                // center of left side
                Vector3 endPosition = new Vector3(childNode.GetRect().xMin + connectionOffset, childNode.GetRect().center.y);
                Vector3 bezierOffset = endPosition - startPosition;
                bezierOffset.y = 0;
                bezierOffset.x *= .8f;
                Handles.DrawBezier(
                    startPosition,
                    endPosition,
                    startPosition + bezierOffset,
                    endPosition - bezierOffset,
                    Color.white,
                    null,
                    lineThickness
                );

                DrawArrowAtPoint(endPosition);
            }
        }

        private void DrawArrowAtPoint(Vector3 point)
        {
            const int arrowOffset = 10;
            Vector3 endArrowLine1 = new Vector2(point.x - arrowOffset, point.y + arrowOffset);

            Handles.DrawBezier(
                    endArrowLine1,
                    point,
                    endArrowLine1,
                    point,
                    Color.white,
                    null,
                    lineThickness
                );

            Vector3 endArrowLine2 = new Vector2(point.x - arrowOffset, point.y - arrowOffset);
            Handles.DrawBezier(
                    endArrowLine2,
                    point,
                    endArrowLine2,
                    point,
                    Color.white,
                    null,
                    lineThickness
                );
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChange()
        {
            var awesomeNewDailogue = (Selection.activeObject as Dialogue);
            if (awesomeNewDailogue != null)
            {
                selectedDialogue = awesomeNewDailogue;
                Repaint();
            }
        }

        private void ProcessEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    nodeDragging.current = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                    isDraggingCanvas = nodeDragging.current == null ? true : isDraggingCanvas;
                    draggingCanvasOffset = isDraggingCanvas ? Event.current.mousePosition + scrollPosition : draggingCanvasOffset;
                    nodeDragging.CaptureOffset();
                    Selection.activeObject =
                        (UnityEngine.Object)nodeDragging.current ?? selectedDialogue;
                    break;
                case EventType.MouseUp:
                    if (nodeDragging.isDragging)
                    {
                        MoveDialogNode(Event.current.mousePosition);
                    }
                    nodeDragging.StopDragging();
                    isDraggingCanvas = false;
                    break;
                case EventType.MouseDrag:
                    if (nodeDragging.isDragging)
                    {
                        MoveDialogNode(Event.current.mousePosition);
                    }
                    // Update scroll position
                    else if (isDraggingCanvas)
                    {
                        scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                        GUI.changed = true;
                    }
                    break;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            if (selectedDialogue == null) return null;
            var nodes = selectedDialogue.GetAllNodes().ToList();

            int index = nodes.Count() - 1;
            while (index >= 0)
            {
                if (nodes[index].GetRect().Contains(point))
                {
                    return nodes[index];
                }
                index--;
            }
            return null;
        }

        private void MoveDialogNode(Vector2 position)
        {
            if (!nodeDragging.isDragging) return;

            nodeDragging.current.SetPosition(position + nodeDragging.draggingOffset);
            GUI.changed = true;
        }

        // To the bowels of the code these functions go
        [NonSerialized]
        Rect _rect;
        private void RefreshViewRect()
        {
            GUILayout.Label("hack", GUILayout.MaxHeight(0));
            if (Event.current.type == EventType.Repaint)
            {
                // hack to get real view width
                _rect = GUILayoutUtility.GetLastRect();
                _rect.height = Mathf.Max(bottomRight.y, Screen.height);
                _rect.width = Mathf.Max(bottomRight.x, _rect.width);
            }
        }

        private struct NodeDraggingProperties
        {
            public DialogueNode current;
            public Vector2 _draggingOffset;

            public Vector2 draggingOffset
            {
                get
                {
                    return _draggingOffset;
                }
            }
            public bool isDragging
            {
                get { return current != null; }
            }

            public void StopDragging()
            {
                current = null;
            }

            public void CaptureOffset()
            {
                if (current == null) return;
                _draggingOffset = current.GetRect().position - Event.current.mousePosition;
            }
        }
    }
}