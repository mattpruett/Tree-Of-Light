using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text text = null;

        public void SetValue(string value)
        {
            text.text = value;
        }

        public void SetValue(float value)
        {
            SetValue(value.ToString());
        }

        public void DestroyText()
        {
            Destroy(gameObject);
        }
    }
}