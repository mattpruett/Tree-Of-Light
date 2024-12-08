using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicsControlRemover : MonoBehaviour
    {
        PlayableDirector director;
        private void Awake()
        {
            director = GetComponent<PlayableDirector>();
        }

        private void OnEnable()
        {
            director.played += DisableControl;
            director.stopped += EnableControl;
        }

        private void OnDisable()
        {
            director.played -= DisableControl;
            director.stopped -= EnableControl;
        }

        private void DisableControl(PlayableDirector director)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        private void EnableControl(PlayableDirector director)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
