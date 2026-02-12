using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    private TextMeshProUGUI _textComponent;
    private void Awake()
    {
        _textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void FixedUpdate()
    {
        _textComponent.text = ScoreManager.Instance.GetScore().
            ToString(CultureInfo.CurrentCulture);
    }
}
