using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target = null;

        void LateUpdate()
        {
            if (target != null)
                transform.position = target.position;
        }
    }
}