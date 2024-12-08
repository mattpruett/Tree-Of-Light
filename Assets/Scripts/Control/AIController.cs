using UnityEngine;
using RPG.Core;
using RPG.Combat;
using RPG.Attributes;
using RPG.Movement;
using GameDevTV.Utils;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [Header("Behavior")]
        [Range(0, float.MaxValue)]
        [SerializeField] float chaseDistance = 5f;
        [Range(0, float.MaxValue)]
        [SerializeField] float suspicionDuration = 10f;
        [Range(0, float.MaxValue)]
        [SerializeField] float agroDuration = 5f;
        [Range(0, float.MaxValue)]
        [SerializeField] float agroRange = 50f;

        [Header("Patrolling")]
        [SerializeField] float wayPointTolerance = 2f;
        [SerializeField] float patrolPauseTime = 5f;

        [Range(0, 1)]
        [SerializeField] float patrolSpeedModifer = .2f;
        [SerializeField] PatrolPath patrolPath = null;

        GameObject targetPlayer;
        Health targetPlayerHealth;
        Fighter fighter;

        LazyValue<Vector3> guardPosition;
        int currentWaypointIndex = 0;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceLastWaypointMove = Mathf.Infinity;
        float timeSinceAggravated = Mathf.Infinity;

        #region  Called By Unity
        private void Awake()
        {
            targetPlayer = GameObject.FindWithTag("Player");
            targetPlayerHealth = targetPlayer?.GetComponent<Health>();
            fighter = GetComponent<Fighter>();
            guardPosition = new LazyValue<Vector3>(GetInitialGuardPosition);
        }

        void Start()
        {
            guardPosition.ForceInit();
        }

        private void Update()
        {
            // Calculate distance between myself and the player
            if (targetPlayer == null || !targetPlayerHealth.isAlive) return;

            ChaseAndAttackPlayerIfPossible();
        }

        private Vector3 GetInitialGuardPosition()
        {
            return transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
        #endregion

        private void ChaseAndAttackPlayerIfPossible()
        {
            if (IsAggravated() && fighter.CanAttack(targetPlayer))
            {
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionDuration)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            UpdateTimers();
        }

        public void Aggravate()
        {
            timeSinceAggravated = 0;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceLastWaypointMove += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
        }

        private void AttackBehavior()
        {
            AggravateNearbyEnemies();
            timeSinceLastSawPlayer = 0;
            fighter.Attack(targetPlayer);
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, agroRange, Vector3.up, 0);
            AIController enemyAI;
            // Loop hit
            foreach (RaycastHit hit in hits)
            {
                // find those with enemy components
                enemyAI = hit.transform.gameObject.GetComponent<AIController>();

                if (enemyAI == null) continue;

                // call aggravate
                enemyAI.Aggravate();
            }
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = guardPosition.value;
            if (patrolPath != null)
            {
                if (AtWayPoint())
                {
                    timeSinceLastWaypointMove = 0f;
                    CycleWayPoint();
                }
                nextPosition = GetCurrentWayPoint();
            }

            if (timeSinceLastWaypointMove > patrolPauseTime)
            {
                GetComponent<Mover>().StartMoveAction(nextPosition, patrolSpeedModifer);
            }
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWayPoint()
        {
            currentWaypointIndex = patrolPath.NextWaypoint(currentWaypointIndex);
        }

        private bool AtWayPoint()
        {
            Vector3 waypoint = GetCurrentWayPoint();
            return Vector3.Distance(waypoint, transform.position) < wayPointTolerance;
        }

        private bool IsAggravated()
        {
            // check if within agro time-span
            return
                timeSinceAggravated < agroDuration ||
                Vector3.Distance(transform.position, targetPlayer.transform.position) < chaseDistance;
        }
    }
}