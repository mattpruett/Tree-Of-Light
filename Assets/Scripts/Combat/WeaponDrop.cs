using System.Collections;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponDrop : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float respawnTime = 5f;
        [SerializeField] float healthRestorationValue = 0f;
        [SerializeField] CursorType cursor = CursorType.WeaponDrop;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject other)
        {
            if (weapon != null)
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
            }

            if (healthRestorationValue > 0)
            {
                other.GetComponent<Health>().Heal(healthRestorationValue);
            }
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            HideDrop();
            yield return new WaitForSeconds(seconds);
            ShowDrop();
        }

        private void HideDrop()
        {
            ToggleDropEnabled(false);
        }

        private void ShowDrop()
        {
            ToggleDropEnabled(true);
        }

        private void ToggleDropEnabled(bool enabled)
        {
            GetComponent<Collider>().enabled = enabled;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(enabled);
            }
        }

        public bool HandleRaycast(PlayerController callingController, RaycastHit hit)
        {
            if (Input.GetMouseButton(0))
            {
                callingController.MovePlayerTo(hit.point);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return cursor;
        }
    }
}
