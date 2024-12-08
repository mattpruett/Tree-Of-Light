using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using UnityEngine.Rendering;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        //Warning about no possible default value or not initialized.
#pragma warning disable 649
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }
#pragma warning restore 649

        [SerializeField]
        CursorMapping[] cursorMappings = null;

        [SerializeField]
        float sphereCastRadius = 1f;

        [SerializeField]
        float maxNavMeshToleranceThreshold = 1f;

#pragma warning disable 649
        [SerializeField]
        float maxPathLength = 50f;
#pragma warning restore 649

        bool isDraggingUI = false;

        Health playerHealth;
        private void Awake()
        {
            playerHealth = GetComponent<Health>();
        }

        private void Update()
        {
            if (InteractWithUI()) return;
            if (PlayerIsDead()) return;
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private bool PlayerIsDead()
        {
            if (playerHealth == null || !playerHealth.isAlive)
            {
                SetCursor(CursorType.None);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = SpherecastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this, hit))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private bool InteractWithUI()
        {
            isDraggingUI = Input.GetMouseButtonUp(0) ? false : isDraggingUI;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                isDraggingUI = Input.GetMouseButtonDown(0) ? true : isDraggingUI;
                SetCursor(CursorType.UI);
                return true;
            }
            return isDraggingUI;
        }

        private bool InteractWithMovement()
        {
            bool hasHit = RaycastNavMesh(out Vector3 target);

            if (Input.GetMouseButtonDown(1))
            {
                bool navmeshHit = InteractWithMovement();
                print("right-click");
            }

            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;
                if (Input.GetMouseButton(0))
                {
                    MovePlayerTo(target);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = Vector3.zero;

            // Raycast to terrain
            if (!Physics.Raycast(GetMouseRay(), out RaycastHit raycastHit)) return false;

            // Find nearest navmesh point
            if (!NavMesh.SamplePosition(
                raycastHit.point,
                out NavMeshHit hit,
                maxNavMeshToleranceThreshold,
                NavMesh.AllAreas))
            {
                return false;
            }

            // return true if found.
            target = hit.position;

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float totalDistance = 0;
            if (path.corners.Length < 2) return totalDistance;
            // Here's how the course did it incase my solution presents problems.
            /*
            for (int i = 0; i < path.corners.Length-1; i++)
                totalDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            */
            Vector3 lastPosition = transform.position;

            foreach (Vector3 corner in path.corners)
            {
                totalDistance += Vector3.Distance(lastPosition, corner);
                lastPosition = corner;
            }
            return totalDistance;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            return cursorMappings[(int)type];
        }

        private static RaycastHit[] RaycastAll()
        {
            return Physics.RaycastAll(GetMouseRay());
        }

        private RaycastHit[] SpherecastAll()
        {
            return Physics.SphereCastAll(GetMouseRay(), sphereCastRadius);
        }

        private RaycastHit[] SpherecastAllSorted()
        {
            RaycastHit[] hits = SpherecastAll();
            SortRaycastHitArray(hits);
            return hits;
        }

        private static RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = RaycastAll();
            SortRaycastHitArray(hits);
            return hits;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        public void MovePlayerTo(Vector3 point)
        {
            GetComponent<Mover>().StartMoveAction(point, 1f);
        }

        private static void SortRaycastHitArray(RaycastHit[] a)
        {
            if (a.Length == 0) return;
            SortRaycastHitArray(a, 0, a.Length - 1);
        }

        private static void SortRaycastHitArray(RaycastHit[] a, int min, int max)
        {
            float pvt = a[(min + max) / 2].distance;
            RaycastHit swp;
            int lo = min, hi = max;
            while (lo <= hi)
            {
                while (a[lo].distance < pvt) lo++;
                while (a[hi].distance > pvt) hi--;
                if (lo > hi) break;

                swp = a[lo]; a[lo] = a[hi]; a[hi] = swp;
                lo++; hi--;
            }
            if (min < hi) SortRaycastHitArray(a, min, hi);
            if (lo < max) SortRaycastHitArray(a, lo, max);
        }
    }
}