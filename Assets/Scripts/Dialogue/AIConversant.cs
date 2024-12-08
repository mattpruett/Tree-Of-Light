using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue dialogue = null;
        [SerializeField] string conversantName;

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController, RaycastHit hit)
        {
            if (dialogue == null) return false;

            // TODO: Don't allow player to click on guard from far away
            // Make the player move to the conversant before a conversation
            // can begin.
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);
            }
            return true;
        }

        public string GetConversantName()
        {
            return conversantName;
        }
    }
}
