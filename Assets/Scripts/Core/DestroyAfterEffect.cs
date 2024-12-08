using UnityEngine;

namespace RGP.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField]
        GameObject targetToDestory = null;

        void Update()
        {
            if (!GetComponent<ParticleSystem>().IsAlive())
            {
                /*
                print("Destroying particle effect");
                print($"targetToDestory == null: {targetToDestory == null}");
                print($"targetToDestory == null: {gameObject == null}");
                */
                // TODO: this is called infinitely for some reason.
                Destroy(targetToDestory ?? gameObject);
            }
        }
    }
}