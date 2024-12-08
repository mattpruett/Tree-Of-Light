using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string conversantName;
        Dialogue currentDialogue;

        DialogueNode currentNode = null;

        AIConversant currentConversant;
        bool isChoosing = false;

        public event Action onConversationUpdated;

        public void StartDialogue(AIConversant conversant, Dialogue newDialogue)
        {
            currentConversant = conversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue?.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();
        }

        public void Quit()
        {
            TriggerExitAction();
            isChoosing = false;
            StartDialogue(null, null);
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            return currentNode?.Text ?? string.Empty;
        }

        public void Next()
        {
            int playerResponseCount = currentDialogue.GetPlayerChildren(currentNode).Count();
            if (playerResponseCount > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }
            TriggerExitAction();
            DialogueNode[] children = currentDialogue.GetAIChildren(currentNode).ToArray();
            currentNode = children == null || children.Length == 0
                ? null
                : children[UnityEngine.Random.Range(0, children.Length)];

            TriggerEnterAction();
            onConversationUpdated();
        }

        public void SelectResponse(DialogueNode selectedReponse)
        {
            currentNode = selectedReponse;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public bool HasNext()
        {
            return currentNode != null && currentNode.HasResponses();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetPlayerChildren(currentNode);
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                TriggerDialogueActions(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                TriggerDialogueActions(currentNode.GetOnExitAction());
            }
        }

        private void TriggerDialogueActions(string action)
        {
            if (currentConversant == null || string.IsNullOrEmpty(action)) return;

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

        public string GetCurrentConversantName()
        {
            return isChoosing ? conversantName : currentConversant.GetConversantName();
        }
    }
}
