using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();

        [SerializeField]
        Vector2 newNodeOffset = new Vector2(215, 0);

        Dictionary<string, DialogueNode> nodeMap = new Dictionary<string, DialogueNode>();

        private void OnValidate()
        {
            nodeMap.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeMap[node.name] = node;
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

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parent)
        {
            if (parent == null) yield break;

            foreach (string childId in parent.GetResponseIDs())
            {
                if (nodeMap.ContainsKey(childId))
                {
                    yield return nodeMap[childId];
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode parent)
        {
            if (parent == null) yield break;

            foreach (DialogueNode child in GetAllChildren(parent))
            {
                if (child.IsPlayerSpeaking())
                {
                    yield return child;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode parent)
        {
            if (parent == null) yield break;

            foreach (DialogueNode child in GetAllChildren(parent))
            {
                if (!child.IsPlayerSpeaking())
                {
                    yield return child;
                }
            }
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateNodeObject(parent);
            Undo.RegisterCompleteObjectUndo(newNode, "Created Dalog Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode node)
        {
            Undo.RecordObject(this, "Deleting Dialogue Node");
            nodes.Remove(node);
            nodeMap.Remove(node.name);
            // Remove id for it's parent
            RemoveAsChild(node);
            Undo.DestroyObjectImmediate(node);
        }

        private void RemoveAsChild(DialogueNode child)
        {
            foreach (DialogueNode node in nodes)
            {
                node.RemoveResponseID(child.name);
            }
        }

        private DialogueNode CreateNodeObject(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parent != null)
            {
                parent.AddResponseID(newNode.name);
                newNode.SetIsPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            nodeMap[newNode.name] = newNode;
        }
#endif
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // Create the root node if it doesn't exist.
            if (nodes.Count == 0)
            {
                AddNode(CreateNodeObject(null));
            }

            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(node)))
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
            // Needed for the interface implementation
        }
    }
}