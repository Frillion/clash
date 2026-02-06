using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CrowBodyAnim : MonoBehaviour
{
    [SerializeField] private float animationSpeed;
    [SerializeField] private float animationStrength;
    private CancellationTokenSource _animSource;

    private void Awake()
    {
        if (_animSource != null)
        {
            _animSource.Cancel();
            _animSource.Dispose();
        }

        _animSource =
            CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                CancellationToken.None);
        Animate(_animSource.Token).Forget();
    }

    private async UniTask Animate(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var offset = Mathf.Round(Mathf.Sin(Time.time * Mathf.Deg2Rad * 360 * animationSpeed)) * animationStrength;
            transform.position += new Vector3(0,offset,0);
            await UniTask.NextFrame(cancellationToken: token);
        }
    }

}
