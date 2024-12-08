using UnityEngine;

namespace RPG.Control
{
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController callingController, RaycastHit hit);

        CursorType GetCursorType();
    }
}