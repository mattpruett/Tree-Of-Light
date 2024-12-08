using System.Collections;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        #region  Destination IDs
        public enum PortalDestination
        {
            Aurek,
            Besh,
            Cresh,
            Cherek,
            Dorn,
            Esk,
            Enth,
            Onith,
            Forn,
            Grek,
            Herf,
            Isk,
            Jenth,
            Krill,
            Krenth,
            Leth,
            Mern,
            Nern,
            Nen,
            Osk,
            Orenth,
            Peth,
            Qek,
            Resh,
            Senth,
            Shen,
            Trill,
            Thesh,
            Usk,
            Vev,
            Wesk,
            Xesh,
            Yirt,
            Zerek
        }
        #endregion

        [Header("Portal")]
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint = default;
        [SerializeField] PortalDestination destination = default;

        [Header("Fader")]
        [Min(0f)]
        [SerializeField] float fadeOutDuration = 2f;
        [Min(0f)]
        [SerializeField] float fadeInDuration = 2f;
        [Min(0f)]
        [SerializeField] float transitionWaitDuration = 2f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }


            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            // Save level/scene
            SavingWrapper saver = FindObjectOfType<SavingWrapper>();

            // Remove player Controls
            DisableControl();

            yield return fader.FadeOut(fadeOutDuration);

            saver.Save();

            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneToLoad);
            // Remove control
            DisableControl();

            // Load level/scene
            saver.Load();

            SpawnPlayerAt(GetOtherPortal());

            saver.Save();

            yield return new WaitForSeconds(transitionWaitDuration);
            fader.FadeIn(fadeInDuration);

            // Restore Control
            EnableControl();

            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal != this && portal.destination == destination)
                {
                    return portal;
                }
            }
            return null;
        }

        private void SpawnPlayerAt(Portal otherPortal)
        {
            if (otherPortal == null) return;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var agent = player.GetComponent<NavMeshAgent>();
                agent.enabled = false;
                player.transform.position = otherPortal.spawnPoint.position;
                player.transform.rotation = otherPortal.spawnPoint.rotation;
                agent.enabled = true;
            }
        }

        private void DisableControl()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        private void EnableControl()
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
