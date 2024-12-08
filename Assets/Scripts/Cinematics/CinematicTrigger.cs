using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                GetComponent<PlayableDirector>().Play();
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
