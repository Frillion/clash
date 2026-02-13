using System.Threading;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimeManager : SingletonMonoBehaviour<TimeManager>
{
    [HideInInspector] public float totalTime;
    [HideInInspector] public float deltaTime;
    [HideInInspector] public float fixedDeltaTime;
    
    private bool _paused;
    private float _timeScale;

    public void Init()
    {
        TokenSystem.Instance.AddToken(nameof(TimeManager),
            CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                CancellationToken.None));
        
        _paused = false;
        _timeScale = 1;
        totalTime = 0;
        TickTime(TokenSystem.Instance.GetToken(nameof(TimeManager)).Token).Forget();
    }

    public async UniTask PauseFor(int milliseconds, CancellationToken token)
    {
        _paused = true;
        await UniTask.Delay(milliseconds, DelayType.Realtime, cancellationToken: token);
        _paused = false;
    }

    public async UniTask SetTimeScaleFor(float timeScale, int milliseconds, CancellationToken? token = null)
    {
        _timeScale = timeScale;
        if (token == null)
        {
            await UniTask.Delay(milliseconds, DelayType.Realtime, cancellationToken: TokenSystem.Instance.GetToken(nameof(TimeManager)).Token);
        }
        else
        {
            await UniTask.Delay(milliseconds, DelayType.Realtime, cancellationToken:token.Value);
        }

        _timeScale = 1;
    }

    public async UniTask PauseGuard(CancellationToken token)
    {
        while (_paused && !token.IsCancellationRequested)
        {
            await UniTask.NextFrame(cancellationToken: token);
        }
    }

    private async UniTask TickTime(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (_paused)
            {
                deltaTime = 0;
                fixedDeltaTime = 0;
            }
            else
            {
                totalTime += Time.deltaTime * _timeScale;
                deltaTime = Time.deltaTime * _timeScale;
                fixedDeltaTime = Time.fixedDeltaTime * _timeScale;
            }

            await UniTask.NextFrame(cancellationToken: token);
        }
    }
}
