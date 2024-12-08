using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Control
{
    public class CollisionPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] CursorType cursor = CursorType.WeaponDrop;

        Pickup pickup;

        public CursorType GetCursorType()
        {
            return cursor;
        }

        public bool HandleRaycast(PlayerController callingController, RaycastHit hit)
        {
            if (Input.GetMouseButton(0))
            {
                callingController.MovePlayerTo(hit.point);
            }
            return true;
        }

        void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup();
            }
        }

        private void Pickup()
        {
            pickup.PickupItem();
        }
    }
}