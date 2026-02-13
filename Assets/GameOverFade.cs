using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Clash.Features.Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Clash.Features.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameOverFade : MonoBehaviour
    {
        [SerializeField] private float maxAlpha;
        private CanvasGroup _fadeGroup;
        private float _alpha;

        private void Awake()
        {
            GameManager.Instance.OnGameStart += Refresh;
            GameManager.Instance.OnGameEnd += Trigger;
            _fadeGroup = GetComponent<CanvasGroup>();
        }

        private void Refresh()
        {
            _alpha = 0;
            _fadeGroup.alpha = _alpha;
        }

        private void Trigger()
        {
            TokenSystem.Instance.AddToken(nameof(GameOverFade),
                CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                    CancellationToken.None));
            
            FadeIn(TokenSystem.Instance.GetToken(nameof(GameOverFade)).Token).Forget();
        }

        private async UniTask FadeIn(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _fadeGroup.alpha = _alpha;
                _alpha += Time.deltaTime;
                
                if (_alpha >= maxAlpha)
                {
                    TokenSystem.Instance.Cancel(nameof(GameOverFade));
                }

                await UniTask.NextFrame(cancellationToken: token);
            }
        }
    }
}

