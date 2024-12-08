using System;
using RPG.Core;
using GameDevTV.Saving;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.AI;


namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 6f;

        [SerializeField]
        float maxPathLength = 50f;

        NavMeshAgent agent;
        Health health;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            health = health ?? GetComponent<Health>();
            agent.enabled = health.isAlive;
            UpdateAnimator();
        }

        void UpdateAnimator()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(GetComponent<NavMeshAgent>().velocity);
            GetComponent<Animator>().SetFloat("forwardSpeed", localVelocity.z);
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

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();

            if (!NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path) || path.status != NavMeshPathStatus.PathComplete)
                return false;

            float pathLength = GetPathLength(path);
            return pathLength <= maxPathLength;
        }

        public void MoveTo(Vector3 destination, float speedModifer)
        {
            if (agent.enabled)
            {
                agent.destination = destination;
                agent.speed = maxSpeed * Mathf.Clamp01(speedModifer);
                agent.isStopped = false;
            }
        }

        public void Cancel()
        {
            if (agent.enabled)
            {
                agent.isStopped = true;
            }
        }

        public void StartMoveAction(Vector3 destination, float speedModifer)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedModifer);
        }

        public object CaptureState()
        {
            return new MoverSaveData()
            {
                position = new SerializableVector3(transform.position),
                eulerAngles = new SerializableVector3(transform.eulerAngles)
            };
        }

        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;
            var agent = GetComponent<NavMeshAgent>();
            agent.enabled = false;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.eulerAngles.ToVector();
            agent.enabled = true;
        }

        #region Structs, enums, other
        [Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 eulerAngles;
        }
        #endregion 
    }
}