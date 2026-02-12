using System;
using System.Threading;
using Clash.Features.Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    private static readonly int Alpha = Shader.PropertyToID("_alpha");
    private Image _renderer;
    private float _currentAlpha;
    
    private void Awake()
    {
        _renderer = GetComponent<Image>();
    }

    private void Start()
    {
        TokenSystem.Instance.AddToken(nameof(DamageIndicator),
            CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(), CancellationToken.None));
        
        _currentAlpha = 0;
        _renderer.material.SetFloat(Alpha, _currentAlpha);

        PlayerHealthComponent.OnDamaged += ShowIndicator;
        TickAlpha(TokenSystem.Instance.GetToken(nameof(DamageIndicator)).Token).Forget();
    }

    private void ShowIndicator()
    {
        _currentAlpha = 0.5f;
    }

    private async UniTask TickAlpha(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            _renderer.material.SetFloat(Alpha, _currentAlpha);
            _currentAlpha -= Time.deltaTime;
            _currentAlpha = Mathf.Clamp(_currentAlpha, 0, 1);
            await UniTask.NextFrame(cancellationToken: token);
        }

        PlayerHealthComponent.OnDamaged -= ShowIndicator;
    }
}
