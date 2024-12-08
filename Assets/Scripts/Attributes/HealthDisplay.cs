using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        Text healthText;
        void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            healthText = GetComponent<Text>();
        }

        void Update()
        {
            if (healthText == null) return;

            ShowHealthValue();
        }

        void ShowHealthAsPercentage()
        {
            healthText.text = string.Format("{0:0}%", health.GetPercentage());
        }

        void ShowHealthValue()
        {
            healthText.text = string.Format("{0}/{1}", health.value, health.maxValue);
        }
    }
}
