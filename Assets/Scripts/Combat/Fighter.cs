using RPG.Movement;
using RPG.Core;
using GameDevTV.Saving;
using RPG.Attributes;
using UnityEngine;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using GameDevTV.Inventories;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandPosition = null;
        [SerializeField] Transform LeftHandPosition = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        Health target;
        Health fighterHealth;
        Animator animator;
        Mover mover;
        Equipment equipment;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;
        float timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            fighterHealth = GetComponent<Health>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(GetInitialWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (!CanAttack()) return;

            if (TargetIsInRange(target))
            {
                AttackBehavior();
                mover.Cancel();
            }
            else
            {
                mover.MoveTo(target.transform.position, 1f);
            }
        }

        Weapon GetInitialWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            EquipWeapon(weapon ?? defaultWeapon);
        }

        public bool CanAttack()
        {

            return target != null && target.isAlive && fighterHealth.isAlive;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;

            bool canMoveTo = GetComponent<Mover>().CanMoveTo(combatTarget.transform.position);
            bool targetIsInRange = TargetIsInRange(combatTarget.transform);
            if (!canMoveTo && !targetIsInRange) return false;

            // CombatTarget requires the health component. So we don't need to check for its existence.
            return combatTarget.GetComponent<Health>().isAlive;
        }

        private void AttackBehavior()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                PerformAttack();
            }
        }

        private void PerformAttack()
        {
            // This will trigger the Hit() event
            TriggerAttack();
            timeSinceLastAttack = 0f;
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        private void TriggerStopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        private void HitTargetWithWeapon()
        {
            float baseDamage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            currentWeapon.value?.OnHit();
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(
                    gameObject,
                    rightHandPosition,
                    LeftHandPosition,
                    target,
                    baseDamage
                );
            }
            else
            {
                target.TakeDamage(gameObject, baseDamage);
            }
        }

        private bool TargetIsInRange(Health target)
        {
            return Vector3.Distance(target.transform.position, transform.position) < currentWeaponConfig.Range;
        }

        private bool TargetIsInRange(Transform target)
        {
            return Vector3.Distance(transform.position, target.position) < currentWeaponConfig.Range;
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandPosition, LeftHandPosition, GetComponent<Animator>());
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            print($"Equipping {weapon.name}");
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        public Health GetTargetHealth()
        {
            return target;
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            target = null;
            TriggerStopAttack();
            mover.Cancel();
        }

        #region Saving
        public object CaptureState()
        {
            return currentWeaponConfig.name;
            //return currentWeaponConfig?.name ?? "Unarmed";
        }

        public void RestoreState(object state)
        {
            EquipWeapon(Resources.Load<WeaponConfig>(state as string));
        }
        #endregion

        #region Animation Events
        private void Hit()
        {
            if (target == null || currentWeaponConfig == null) return;

            HitTargetWithWeapon();
        }

        private void Shoot()
        {
            Hit();
        }
        #endregion
    }
}