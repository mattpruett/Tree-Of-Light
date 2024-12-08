using UnityEngine;

namespace RPG.Movement
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] float gizmoRaidus = 1f;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            int count = transform.childCount;
            Transform currentChild;
            Transform lastChild = count == 0 ? null : transform.GetChild(transform.childCount - 1);

            for (int i = 0; i < transform.childCount; i++)
            {
                currentChild = transform.GetChild(i);
                Gizmos.DrawSphere(currentChild.transform.position, gizmoRaidus);
                Gizmos.DrawLine(lastChild.position, currentChild.position);
                lastChild = currentChild;
            }
        }

        public Vector3 GetWaypoint(int index)
        {
            return transform.GetChild(index).position;
        }

        public int NextWaypoint(int currentIndex)
        {
            return (currentIndex + 1) % transform.childCount;
        }
    }
}
