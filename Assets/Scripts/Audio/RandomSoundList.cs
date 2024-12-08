using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Audio
{
    public class RandomSoundList : MonoBehaviour
    {
        [SerializeField] AudioClip[] clips = null;

        public void PlayRandomClip()
        {
            AudioSource audioSource = GetComponent<AudioSource>();

            if (audioSource == null) return;
            AudioClip clip = SelectRandomClip();

            if (clip == null) return;

            audioSource.clip = clip;
            audioSource.Play();
        }

        private AudioClip SelectRandomClip()
        {
            return clips == null || clips.Length == 0
                ? null
                : clips[Random.Range(0, clips.Length)];
        }
    }
}
