using System;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        public event Action onExperienceGained;

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void GainExperience(float xp)
        {
            experiencePoints += Mathf.Max(xp, -experiencePoints);
            onExperienceGained();
        }

        public void RestoreState(object state)
        {
            experiencePoints = Mathf.Max(0, (float)state);
        }

        public float value
        {
            get { return experiencePoints; }
        }
    }
}