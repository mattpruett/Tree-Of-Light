using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField]
        DamageText damageText = null;

        public void Spawn(float value)
        {
            DamageText dmgText = Instantiate<DamageText>(damageText, transform);
            dmgText.SetValue(value);
        }
    }

}