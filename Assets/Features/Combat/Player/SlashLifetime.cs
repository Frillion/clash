using System;
using System.Collections.Generic;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SlashLifetime : Spawnable
{
    private static readonly int Progress1 = Shader.PropertyToID("_Progress");
    
    [SerializeField] private AnimationCurve slashCurve;
    private readonly List<Material> _mats = new();
    private float _slashDuration;
    private float _currentTime;

    private void Awake()
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in renderers)
        {
            _mats.Add(spriteRenderer.material);
        }
    }

    public void Initialize(float duration)
    {
        _slashDuration = duration;
        Loop().Forget();
    }

    private async UniTask Loop()
    {
        while (_currentTime < _slashDuration)
        {
            _currentTime += Time.deltaTime;

            var timeStep = _currentTime / _slashDuration;
            var currentValue = slashCurve.Evaluate(timeStep);
            currentValue = -1 + currentValue * 2f;
            
            _mats.ForEach(mat => mat.SetFloat(Progress1, currentValue));
        
            await UniTask.NextFrame(PlayerLoopTiming.Update);
        }
        
        Despawn();
    }

    public override void Despawn()
    {
        _currentTime = 0;
        base.Despawn();
    }
}
