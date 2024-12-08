using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class XPDisplay : MonoBehaviour
    {
        Experience playerXP;
        Text xpText;
        void Awake()
        {
            playerXP = GameObject.FindWithTag("Player").GetComponent<Experience>();
            xpText = GetComponent<Text>();
        }

        void Update()
        {
            if (xpText == null) return;

            xpText.text = string.Format("{0}", playerXP.value);
        }
    }
}
