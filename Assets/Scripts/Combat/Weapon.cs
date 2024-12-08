using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        UnityEvent onHit;
#pragma warning restore 649

        public void OnHit()
        {
            onHit.Invoke();
        }
    }
}