using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] bool visible = true;
        [SerializeField] RectTransform foregroundBar = null;
        [SerializeField] Canvas canvas = null;
        [SerializeField] Health health = null;

        void Update()
        {
            float healthPercentage = health.GetFractionalPercentage();

            bool notZero = !Mathf.Approximately(healthPercentage, 0f);
            bool notOne = !Mathf.Approximately(healthPercentage, 1f);

            foregroundBar.localScale = new Vector3(healthPercentage, 1, 1);
            canvas.enabled = visible && notZero && notOne;
        }
    }
}
