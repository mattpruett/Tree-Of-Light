using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter player;
        Text healthText;
        void Awake()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            healthText = GetComponent<Text>();
        }

        void Update()
        {
            if (healthText == null) return;

            Health health = player.GetTargetHealth();

            healthText.text = health == null
                ? "  -"
                : GetHealthValue(health);
        }

        string GetHealthAsPercentage(Health health)
        {
            return string.Format("{0:0}%", health.GetPercentage());
        }

        string GetHealthValue(Health health)
        {
            return string.Format("{0}/{1}", health.value, health.maxValue);
        }
    }
}
