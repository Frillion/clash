using System.Threading;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimeManager : SingletonMonoBehaviour<TimeManager>
{
    [HideInInspector] public float totalTime;
    [HideInInspector] public float deltaTime;
    [HideInInspector] public float fixedDeltaTime;
    
    private float _savedTime;
    private bool _paused;
    private float _timeScale;
    private CancellationTokenSource _timeSystemToken;

    private new void Awake()
    {
        base.Awake();
        Init(); // Temporary, Remove When Finishing
    }

    public void Init()
    {
        _timeSystemToken?.Cancel();
        _timeSystemToken?.Dispose();
        _timeSystemToken =
            CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                CancellationToken.None);
        
        _paused = false;
        _timeScale = 1;
        TickTime(_timeSystemToken.Token).Forget();
    }

    public async UniTask PauseFor(int milliseconds, CancellationToken token)
    {
        _savedTime = totalTime;
        _paused = true;
        await UniTask.Delay(milliseconds, DelayType.Realtime, cancellationToken: token);
        _paused = false;
        totalTime = _savedTime;
    }

    public async UniTask SetTimeScaleFor(float timeScale, int milliseconds, CancellationToken? token = null)
    {
        _timeScale = timeScale;
        if (token == null)
        {
            await UniTask.Delay(milliseconds, DelayType.Realtime, cancellationToken: _timeSystemToken.Token);
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
                totalTime = 0;
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
