using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats playerStats;
        Text levelText;
        void Awake()
        {
            playerStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            levelText = GetComponent<Text>();
        }

        void Update()
        {
            if (levelText == null) return;

            levelText.text = string.Format("{0}", playerStats.Level);
        }
    }
}
