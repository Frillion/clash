using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clash.Features.UI
{
    public interface IFadable
    {
        void SetOpacity(float alpha);
    }

    public class GameOverFade : MonoBehaviour
    {
        private List<IFadable> _fadableUIElements = new();
        [SerializeField] private float maxAlpha;
        private float _alpha;

        private void Start()
        {
            TokenSystem.Instance.AddToken(nameof(GameOverFade),
                CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                    CancellationToken.None));
            
            _alpha = 0;
        }

        public void Trigger()
        {
            FadeIn(TokenSystem.Instance.GetToken(nameof(GameOverFade)).Token).Forget();
        }

        private async UniTask FadeIn(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _fadableUIElements.ForEach(element => element.SetOpacity(_alpha));
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

