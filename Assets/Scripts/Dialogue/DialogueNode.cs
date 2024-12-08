using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField]
        bool isPlayerSpeaker = false;

        [SerializeField]
        string text;

        [SerializeField]
        List<string> responseIDs = new List<string>();

        [SerializeField]
        Rect rect = new Rect(0, 0, 200, 100);

        [SerializeField]
        string onEnterAction;

        [SerializeField]
        string onExitAction;

        public string Text
        {
            get { return text; }
            set
            {
                SetText(value);
            }
        }

        public IEnumerable<string> GetResponseIDs()
        {
            return responseIDs;
        }

        public bool ResponseExists(string responseId)
        {
            return responseIDs.Contains(responseId);
        }

        public Rect GetRect()
        {
            return rect;
        }

        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaker;
        }

        public bool HasResponses()
        {
            return responseIDs.Count > 0;
        }

        public string GetOnEnterAction()
        {
            return onEnterAction;
        }

        public string GetOnExitAction()
        {
            return onExitAction;
        }

#if UNITY_EDITOR
        public void SetPosition(Vector2 position)
        {
            Undo.RecordObject(this, "Move Dialog Node");
            rect.position = position;
            EditorUtility.SetDirty(this);
        }

        private void SetText(string newText)
        {
            if (text == newText) return;
            Undo.RecordObject(this, "Update Dialogue Text");
            text = newText;
            EditorUtility.SetDirty(this);
        }

        public void AddResponseID(string responseId)
        {
            Undo.RecordObject(this, "Add dialog link");
            responseIDs.Add(responseId);
            EditorUtility.SetDirty(this);
        }

        public void RemoveResponseID(string responseId)
        {
            Undo.RecordObject(this, "Unlink dialog");
            responseIDs.Remove(responseId);
            EditorUtility.SetDirty(this);
        }

        public void SetIsPlayerSpeaking(bool isPlayerSpeaking)
        {
            if (isPlayerSpeaking == isPlayerSpeaker) return;
            Undo.RecordObject(this, "Update Is Player Speaking");
            isPlayerSpeaker = isPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}