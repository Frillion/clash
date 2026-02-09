using System;
using Clash.Features.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Clash.Features.UI
{
    public class HealthUIUpdate : MonoBehaviour
    {
        public PlayerHealthComponent playerHealth;
        private Slider _uiElement;

        private void Awake()
        {
            _uiElement = GetComponent<Slider>();
        }

        private int RemapToSliderRange(float value)
        {
            return (int)((value / playerHealth.health) 
                         * (_uiElement.maxValue - _uiElement.minValue) 
                         + _uiElement.minValue);
        }

        public void Update()
        {
            _uiElement.value = RemapToSliderRange(playerHealth.GetCurrentHp());
        }
    }
}

