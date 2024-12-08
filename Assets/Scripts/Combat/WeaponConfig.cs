using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        enum handedNess
        {
            rightHanded,
            leftHanded
        }

        const string WEAPON_NAME = "Weapon";

        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float damage = 10f;
        [SerializeField] float percentageModifier = 0f;
        [SerializeField] handedNess handed = handedNess.rightHanded;
        [SerializeField] Projectile projectile = null;

        private Transform HandPosition(Transform rightHand, Transform leftHand)
        {
            return handed == handedNess.rightHanded
                    ? rightHand
                    : leftHand;
        }

        private void DestroyWeapons(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(WEAPON_NAME) ?? leftHand.Find(WEAPON_NAME);

            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYME";
            Destroy(oldWeapon.gameObject);
        }

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyWeapons(rightHand, leftHand);
            Weapon weapon = null;
            if (equippedPrefab != null)
            {
                weapon = Instantiate(equippedPrefab, HandPosition(rightHand, leftHand));
                weapon.gameObject.name = WEAPON_NAME;
            }

            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (animator.runtimeAnimatorController is AnimatorOverrideController)
            {
                animator.runtimeAnimatorController =
                    (animator.runtimeAnimatorController as AnimatorOverrideController).runtimeAnimatorController;
            }

            return weapon;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(GameObject instigator, Transform rightHand, Transform leftHand, Health target, float baseDamage)
        {
            Instantiate(
                projectile,
                HandPosition(rightHand, leftHand).position,
                Quaternion.identity
            )
            .SetTarget(instigator, target, baseDamage);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage) yield return damage;
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage) yield return percentageModifier;
        }

        public float Damage
        {
            get { return damage; }
        }

        public float PercentageModifier
        {
            get { return percentageModifier; }
        }

        public float Range
        {
            get { return weaponRange; }
        }
    }
}