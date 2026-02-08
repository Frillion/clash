using System;
using UnityEngine;
using UnityEngine.UI;

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
